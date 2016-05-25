//-----------------------------------------------------------------------
// <copyright file="BlockingRequestHandler.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment.Request
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;
    using Exception;
    using Newtonsoft.Json;
    using Segment.Model;

    internal class BlockingRequestHandler : IRequestHandler
    {
        #if !UNITY_5_3_OR_NEWER

        /// <summary>
        /// JSON serialization settings.
        /// </summary>
        private JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            // Converters = new List<JsonConverter> { new ContextSerializer() }
        };
        
        #endif

        /// <summary>
        /// Segment.io client to mark statistics.
        /// </summary>
        private Client client;

        internal BlockingRequestHandler(Client client, TimeSpan timeout)
        {
            this.client = client;
            this.Timeout = timeout;
        }

        /// <summary>
        /// Gets or sets the maximum amount of time to wait before calling the HTTP flush a timeout failure.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        #if UNITY_5_3_OR_NEWER
        public System.Collections.IEnumerator MakeRequest(Batch batch)
        #else
        public void MakeRequest(Batch batch)
        #endif
        {
            Stopwatch watch = new Stopwatch();

            #if UNITY_5_3_OR_NEWER

            string url = this.client.Config.Host + "/v1/import";

            // set the current request time
            batch.SentAt = DateTime.Now.ToString("o");

            string json = JsonConvert.SerializeObject(batch);
            byte[] jsonBytes = System.Text.UTF8Encoding.UTF8.GetBytes(json);

            var headers = new System.Collections.Generic.Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            headers.Add("Authorization", this.BasicAuthHeader(batch.WriteKey, string.Empty));

            var www = new UnityEngine.WWW(url, jsonBytes, headers);
            
            yield return www;
            
            watch.Stop();
            
            if (string.IsNullOrEmpty(www.error) == false)
            {
                this.Fail(batch, new System.Exception(www.error), watch.ElapsedMilliseconds);
            }
            else
            {
                this.Succeed(batch, watch.ElapsedMilliseconds);
            }

            #else
            
            try
            {
                Uri uri = new Uri(this.client.Config.Host + "/v1/import");

                // set the current request time
                batch.SentAt = DateTime.Now.ToString("o");

                string json = JsonConvert.SerializeObject(batch, this.settings);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

                // Basic Authentication
                // https://segment.io/docs/tracking-api/reference/#authentication
                request.Headers["Authorization"] = this.BasicAuthHeader(batch.WriteKey, string.Empty);

                request.Timeout = (int)this.Timeout.TotalMilliseconds;
                request.ContentType = "application/json";
                request.Method = "POST";

                // do not use the expect 100-continue behavior
                request.ServicePoint.Expect100Continue = false;
                
                // buffer the data before sending, ok since we send all in one shot
                request.AllowWriteStreamBuffering = true;

                Logger.Info("Sending analytics request to Segment.io ..", new Dict
                {
                    { "batch id", batch.MessageId },
                    { "json size", json.Length },
                    { "batch size", batch.Actions.Count }
                });

                watch.Start();

                using (var requestStream = request.GetRequestStream())
                {
                    using (StreamWriter writer = new StreamWriter(requestStream))
                    {
                        writer.Write(json);
                    }
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    watch.Stop();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        this.Succeed(batch, watch.ElapsedMilliseconds);
                    }
                    else
                    {
                        string responseStr = string.Format("Status Code {0}. ", response.StatusCode);
                        responseStr += this.ReadResponse(response);
                        this.Fail(batch, new APIException("Unexpected Status Code", responseStr), watch.ElapsedMilliseconds);
                    }
                }
            }
            catch (WebException e) 
            {
                watch.Stop();
                this.Fail(batch, this.ParseException(e), watch.ElapsedMilliseconds);
            }
            catch (System.Exception e)
            {
                watch.Stop();
                this.Fail(batch, e, watch.ElapsedMilliseconds);
            }

            #endif
        }

        private void Fail(Batch batch, System.Exception e, long duration)
        {
            foreach (BaseAction action in batch.Actions)
            {
                this.client.Statistics.Failed += 1;
                this.client.RaiseFailure(action, e);
            }

            var args = new Dict { { "batch id", batch.MessageId }, { "reason", e.Message }, { "duration (ms)", duration } };
            Logger.Info("Segment.io request failed.", args);
        }

        private void Succeed(Batch batch, long duration)
        {
            foreach (BaseAction action in batch.Actions)
            {
                this.client.Statistics.Succeeded += 1;
                this.client.RaiseSuccess(action);
            }

            Logger.Info("Segment.io request successful.", new Dict { { "batch id", batch.MessageId }, { "duration (ms)", duration } });
        }

        private System.Exception ParseException(WebException e)
        {
            if (e.Response != null && ((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.BadRequest)
            {
                return new APIException("Bad Request", this.ReadResponse(e.Response));
            }
            else
            {
                return e;
            }
        }

        private string ReadResponse(WebResponse response)
        {
            if (response != null)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            else
            {
                return null;
            }
        }

        private string BasicAuthHeader(string user, string pass)
        {
            string val = user + ":" + pass;
            return "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(val));
        }
    }
}
