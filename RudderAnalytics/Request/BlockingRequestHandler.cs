using System;
using System.Diagnostics;
using System.Net;
#if NET35
#else
using System.Net.Http;
using System.Net.Http.Headers;
#endif
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RudderStack.Exception;
using RudderStack.Model;
using RudderStack.Stats;
using System.IO;
using System.IO.Compression;

namespace RudderStack.Request
{
#if NET35
    internal interface IHttpClient
    {
        WebHeaderCollection Headers { get; set; }
        IWebProxy Proxy { get; set; }
        byte[] UploadData(Uri address, string method, byte[] data);
    }

    internal class HttpClient : WebClient, IHttpClient
    {
        public TimeSpan Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest w = base.GetWebRequest(address);
            if (Timeout.Milliseconds != 0)
                w.Timeout = Convert.ToInt32(Timeout.Milliseconds);
            return w;
        }
    }
#else
    class WebProxy : System.Net.IWebProxy
    {
        private string _proxy;

        public WebProxy(string proxy)
        {
            _proxy = proxy;
            GetProxy(new Uri(proxy)); // ** What does this do?
        }

        public System.Net.ICredentials Credentials
        {
            get; set;
        }

        public Uri GetProxy(Uri destination)
        {
            if (!String.IsNullOrWhiteSpace(destination.ToString()))
                return destination;
            else
                return new Uri("");
        }

        public bool IsBypassed(Uri host)
        {
            if (!String.IsNullOrWhiteSpace(host.ToString()))
                return true;
            else
                return false;
        }
    }
#endif

    internal class BlockingRequestHandler : IRequestHandler
    {
        /// <summary>
        /// RudderStack client to mark statistics
        /// </summary>
        private readonly RudderClient _client;

        private readonly Backo _backo;

        private readonly int _maxBackOffDuration;

#if NET35
        private readonly IHttpClient _httpClient;
#else
        private readonly HttpClient _httpClient;
#endif
        /// <summary>
        /// The maximum amount of time to wait before calling
        /// the HTTP flush a timeout failure.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        internal BlockingRequestHandler(RudderClient client, TimeSpan timeout) : this(client, timeout, null, new Backo(max: 10000, jitter: 5000)) // Set maximum waiting limit to 10s and jitter to 5s
        {
        }
        internal BlockingRequestHandler(RudderClient client, TimeSpan timeout, Backo backo) : this(client, timeout, null, backo)
        {
        }
#if NET35

        internal BlockingRequestHandler(RudderClient client, TimeSpan timeout, IHttpClient httpClient, Backo backo)
#else
        internal BlockingRequestHandler(RudderClient client, TimeSpan timeout, HttpClient httpClient, Backo backo)
#endif
        {
            this._client = client;
            _backo = backo;

            this.Timeout = timeout;

            if (httpClient != null)
            {
                _httpClient = httpClient;
            }
            // Create HttpClient instance in .Net 3.5
#if NET35
            if (httpClient == null)
                _httpClient = new HttpClient { Timeout = Timeout };
#else
            var handler = new HttpClientHandler();
#endif

            // Set proxy information
            if (!string.IsNullOrEmpty(_client.Config.Proxy))
            {
#if NET35
                _httpClient.Proxy = new WebProxy(_client.Config.Proxy);
#else
                handler.Proxy = new WebProxy(_client.Config.Proxy);
                handler.UseProxy = true;
#endif
            }

            // Initialize HttpClient instance with given configuration
#if !NET35
            if (httpClient == null)
                _httpClient = new HttpClient(handler) { Timeout = Timeout };
#endif
            // Send user agent in the form of {library_name}/{library_version} as per RFC 7231.
            var szUserAgent = _client.Config.UserAgent;
#if NET35
            _httpClient.Headers.Set("User-Agent", szUserAgent);
#else
            _httpClient.DefaultRequestHeaders.Add("User-Agent", szUserAgent);
#endif
        }

        public async Task MakeRequest(Batch batch)
        {
            Stopwatch watch = new Stopwatch();
            _backo.Reset();
            try
            {
                Uri uri = new Uri(_client.Config.DataPlaneUrl + "/v1/batch");

                // set the current request time
                batch.SentAt = DateTime.UtcNow.ToString("o");

                string json = JsonConvert.SerializeObject(batch);

                // Basic Authentication
#if NET35
                _httpClient.Headers.Set("Authorization", "Basic " + BasicAuthHeader(batch.WriteKey, string.Empty));
                _httpClient.Headers.Set("Content-Type", "application/json; charset=utf-8");
#else
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", BasicAuthHeader(batch.WriteKey, string.Empty));
#endif
                // Prepare request data;
                var requestData = Encoding.UTF8.GetBytes(json);

                // Compress request data if compression is set
                if (_client.Config.Gzip)
                {
#if NET35
                     _httpClient.Headers.Set(HttpRequestHeader.ContentEncoding, "gzip");
#else
                    // _httpClient.DefaultRequestHeaders.Add("Content-Encoding", "gzip");
#endif

                    // Compress request data with GZip
                    using (MemoryStream memory = new MemoryStream())
                    {
                        using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true))
                        {
                            gzip.Write(requestData, 0, requestData.Length);
                        }
                        requestData = memory.ToArray();
                    }
                }

                Logger.Info("Sending analytics request to RudderStack ..", new Dict
                {
                    { "batch id", batch.MessageId },
                    { "json size", json.Length },
                    { "batch size", batch.batch.Count }
                });

                // Retries with exponential backoff
                int statusCode = (int)HttpStatusCode.OK;
                string responseStr = "";

                while (!_backo.HasReachedMax)
                {
#if NET35
                    watch.Start();

                    try
                    {
                        /*
                         Tls = 0xc0
                         Tls11 = 0x300
                         Tls12 = 0xc00
                          The default security protocol on the .NET version 3.5 is Tls, but Tls12 is the established standard now, hence setting it
                          manually using the below statement.
                        */
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                        var response = Encoding.UTF8.GetString(_httpClient.UploadData(uri, "POST", requestData));
                        watch.Stop();
                        Succeed(batch, watch.ElapsedMilliseconds);
                        statusCode = 200;
                        break;
                    }
                    catch (WebException ex)
                    {
                        watch.Stop();

                        var response = (HttpWebResponse)ex.Response;
                        if(response != null && response.GetResponseStream() != null) {
                        using (var reader = new System.IO.StreamReader(response.GetResponseStream(), ASCIIEncoding.ASCII)) {
                            responseStr = reader.ReadToEnd(); 
                        }
                        }
                        statusCode = (response != null) ? (int)response.StatusCode : 0;
                      if ((statusCode >= 500 && statusCode <= 600) || statusCode == 429 || statusCode == 0)
                        {
                            // If status code is greater than 500 and less than 600, it indicates server error
                            // Error code 429 indicates rate limited.
                            // Retry uploading in these cases.
                            Thread.Sleep(_backo.AttemptTime());
                            if (statusCode == 429)
                            {
                                Logger.Info($"Too many request at the moment CurrentAttempt:{_backo.CurrentAttempt} Retrying to send request", new Dict
                                {
                                    { "batch id", batch.MessageId },
                                    { "statusCode", statusCode },
                                    { "duration (ms)", watch.ElapsedMilliseconds }
                                });
                            }
                            else
                            {
                                Logger.Info($"Internal RudderStack Server error CurrentAttempt:{_backo.CurrentAttempt} Retrying to send request", new Dict
                                {
                                    { "batch id", batch.MessageId },
                                    { "statusCode", statusCode },
                                    { "duration (ms)", watch.ElapsedMilliseconds }
                                });
                            }
                            continue;
                        }
                        else
                        {
                            //If status code is greater or equal than 400 but not 429 should indicate is client error.
                            //All other types of HTTP Status code are not errors (Between 100 and 399)
                            break;
                        }

                    }

#else
                    watch.Start();

                    ByteArrayContent content = new ByteArrayContent(requestData);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    if (_client.Config.Gzip)
                    {
                        content.Headers.ContentEncoding.Add("gzip");
                    }

                    HttpResponseMessage response = null;
                    bool retry = false;
                    try
                    {
                        response = await _httpClient.PostAsync(uri, content).ConfigureAwait(false);
                        if (response != null && response.Content != null)
                        {
                            responseStr = await response.Content.ReadAsStringAsync();
                        }

                    }
                    catch (TaskCanceledException e)
                    {
                        Logger.Info("HTTP Post failed with exception of type TaskCanceledException", new Dict
                        {
                            { "batch id", batch.MessageId },
                            { "reason", e.Message },
                            { "duration (ms)", watch.ElapsedMilliseconds }
                        });
                        retry = true;
                    }
                    catch (OperationCanceledException e)
                    {
                        Logger.Info("HTTP Post failed with exception of type OperationCanceledException", new Dict
                        {
                            { "batch id", batch.MessageId },
                            { "reason", e.Message },
                            { "duration (ms)", watch.ElapsedMilliseconds }
                        });
                        retry = true;
                    }
                    catch (HttpRequestException e)
                    {
                        Logger.Info("HTTP Post failed with exception of type HttpRequestException", new Dict
                        {
                            { "batch id", batch.MessageId },
                            { "reason", e.Message },
                            { "duration (ms)", watch.ElapsedMilliseconds }
                        });
                        retry = true;
                    }
                    catch (System.Exception e)
                    {
                        Logger.Info("HTTP Post failed with exception of type Exception", new Dict
                        {
                            { "batch id", batch.MessageId },
                            { "reason", e.Message },
                            { "duration (ms)", watch.ElapsedMilliseconds }
                        });
                        retry = true;
                    }

                    watch.Stop();
                    statusCode = response != null ? (int)response.StatusCode : 0;

                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        Succeed(batch, watch.ElapsedMilliseconds);
                        break;
                    }
                    else
                    {
                        if ((statusCode >= 500 && statusCode <= 600) || statusCode == 429 || retry)
                        {
                            // If status code is greater than 500 and less than 600, it indicates server error
                            // Error code 429 indicates rate limited.
                            // Retry uploading in these cases.
                            await _backo.AttemptAsync();
                            if (statusCode == 429)
                            {
                                Logger.Info($"Too many request at the moment CurrentAttempt:{_backo.CurrentAttempt} Retrying to send request", new Dict
                                {
                                    { "batch id", batch.MessageId },
                                    { "statusCode", statusCode },
                                    { "duration (ms)", watch.ElapsedMilliseconds }
                                });
                            }
                            else
                            {
                                Logger.Info($"Internal RudderStack Server error CurrentAttempt:{_backo.CurrentAttempt} Retrying to send request", new Dict
                                {
                                    { "batch id", batch.MessageId },
                                    { "statusCode", statusCode },
                                    { "duration (ms)", watch.ElapsedMilliseconds }
                                });
                            }
                        }
                        else
                        {
                            //HTTP status codes smaller than 500 or greater than 600 except for 429 are either Client errors or a correct status
                            //This means it should not retry 
                            break;
                        }
                    }
#endif
                }

                if (responseStr.Contains("Invalid JSON"))
                {
                    var message = $"Received Invalid JSON as response back from the dataPlane with status code: {statusCode}, Please verify if the current version of your dataplane supports gzip compression of the request body";
                    Logger.Error(message);
                    Fail(batch, new APIException(statusCode.ToString(), message), watch.ElapsedMilliseconds);
                }
                else if (_backo.HasReachedMax || statusCode != (int)HttpStatusCode.OK)
                {
                    var message = $"Has Backoff reached max: {_backo.HasReachedMax} with number of Attempts:{_backo.CurrentAttempt},\n Status Code: {statusCode}\n, response message: {responseStr}";
                    Fail(batch, new APIException(statusCode.ToString(), message), watch.ElapsedMilliseconds);
                }
            }
            catch (System.Exception e)
            {
                watch.Stop();
                Fail(batch, e, watch.ElapsedMilliseconds);
            }
        }

        private void Fail(Batch batch, System.Exception e, long duration)
        {
            foreach (BaseAction action in batch.batch)
            {
                _client.Statistics.IncrementFailed();
                _client.RaiseFailure(action, e);
            }

            Logger.Info("RudderStack request failed.", new Dict
            {
                { "batch id", batch.MessageId },
                { "reason", e.Message },
                { "duration (ms)", duration }
            });
        }

        private void Succeed(Batch batch, long duration)
        {
            foreach (BaseAction action in batch.batch)
            {
                _client.Statistics.IncrementSucceeded();
                _client.RaiseSuccess(action);
            }

            Logger.Info("RudderStack request successful.", new Dict
            {
                { "batch id", batch.MessageId },
                { "duration (ms)", duration }
            });
        }

        private string BasicAuthHeader(string user, string pass)
        {
            string val = user + ":" + pass;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(val));
        }
    }
}
