using System;
using System.Diagnostics;
using System.Net;
#if NET35
#else
using System.Net.Http;
using System.Net.Http.Headers;
#endif
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Segment.Exception;
using Segment.Model;
using Segment.Stats;

namespace Segment.Request
{
#if NET35
    internal class HttpClient : WebClient
    {
        public TimeSpan Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest w = base.GetWebRequest(address);
            if (Timeout.Milliseconds != 0)
                w.Timeout = Timeout.Milliseconds;
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
		/// Segment.io client to mark statistics
		/// </summary>
		private readonly Client _client;

		private readonly HttpClient _httpClient;

		/// <summary>
		/// The maximum amount of time to wait before calling
		/// the HTTP flush a timeout failure.
		/// </summary>
		public TimeSpan Timeout { get; set; }

		internal BlockingRequestHandler(Client client, TimeSpan timeout)
		{
			this._client = client;
			this.Timeout = timeout;

#if NET35
			_httpClient = new HttpClient { Timeout = Timeout };
			// set proxy
			if (!string.IsNullOrEmpty(_client.Config.Proxy))
				_httpClient.Proxy = new WebProxy(_client.Config.Proxy);
#else
			if (!string.IsNullOrEmpty(_client.Config.Proxy))
			{
				var handler = new HttpClientHandler
				{
					Proxy = new WebProxy(_client.Config.Proxy),
					UseProxy = true
				};
				_httpClient = new HttpClient(handler) { Timeout = Timeout };
			}
			else
				_httpClient = new HttpClient() { Timeout = Timeout };
#endif
		}

		public async Task MakeRequest(Batch batch)
		{
			Stopwatch watch = new Stopwatch();

			try
			{
				Uri uri = new Uri(Defaults.Host + "/v1/import");

				// set the current request time
				batch.SentAt = DateTime.Now.ToString("o");

				string json = JsonConvert.SerializeObject(batch);

				// Basic Authentication
				// https://segment.io/docs/tracking-api/reference/#authentication
#if NET35
				_httpClient.Headers.Add("Authorization", "Basic " + BasicAuthHeader(batch.WriteKey, string.Empty));
                _httpClient.Headers.Add("Content-Type", "application/json; charset=utf-8");
#else
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", BasicAuthHeader(batch.WriteKey, string.Empty));
#endif

				Logger.Info("Sending analytics request to Segment.io ..", new Dict
				{
					{ "batch id", batch.MessageId },
					{ "json size", json.Length },
					{ "batch size", batch.batch.Count }
				});

#if NET35
                watch.Start();

                try
                {
                    var response = Encoding.UTF8.GetString(_httpClient.UploadData(uri, "POST", Encoding.UTF8.GetBytes(json)));
                    watch.Stop();

                    Succeed(batch, watch.ElapsedMilliseconds);
                }
                catch (WebException ex)
                {
                    watch.Stop();

                    string responseStr = ex.Message;
                    Fail(batch, new APIException("Unexpected Response", responseStr), watch.ElapsedMilliseconds);
                }
#else
				watch.Start();

				var response = await _httpClient.PostAsync(uri, new StringContent(json, Encoding.UTF8, "application/json")).ConfigureAwait(false);

				watch.Stop();

				if (response.StatusCode == HttpStatusCode.OK)
				{               
					Succeed(batch, watch.ElapsedMilliseconds);
				}
				else
				{
					string responseStr = String.Format("Status Code {0}. ", response.StatusCode);
					responseStr += await response.Content.ReadAsStringAsync().ConfigureAwait(false);
					Fail(batch, new APIException("Unexpected Status Code", responseStr), watch.ElapsedMilliseconds);
				}
#endif
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
				_client.Statistics.Failed = Statistics.Increment(_client.Statistics.Failed);
				_client.RaiseFailure(action, e);
			}

			Logger.Info("Segment.io request failed.", new Dict
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
				_client.Statistics.Succeeded = Statistics.Increment(_client.Statistics.Succeeded);
				_client.RaiseSuccess(action);
			}

			Logger.Info("Segment.io request successful.", new Dict
			{
				{ "batch id", batch.MessageId },
				{ "duration (ms)", duration }
			});
		}

		private string BasicAuthHeader(string user, string pass)
		{
			string val = user + ":" + pass;
			return Convert.ToBase64String(Encoding.GetEncoding(0).GetBytes(val));
		}
	}
}
