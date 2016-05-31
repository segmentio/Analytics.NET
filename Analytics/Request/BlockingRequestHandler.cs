using System;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Web;

using Newtonsoft.Json;

using Segment.Model;
using Segment.Exception;

namespace Segment.Request
{
    using Dict = System.Collections.Generic.Dictionary<string, object>;
    internal class BlockingRequestHandler : IRequestHandler
	{
		/// <summary>
		/// JSON serialization settings
		/// </summary>
		private JsonSerializerSettings settings = new JsonSerializerSettings() {
			// Converters = new List<JsonConverter> { new ContextSerializer() }
		};

		/// <summary>
		/// Segment.io client to mark statistics
		/// </summary>
		private Client _client;

		/// <summary>
		/// The maximum amount of time to wait before calling
		/// the HTTP flush a timeout failure.
		/// </summary>
		public TimeSpan Timeout { get; set; }

		internal BlockingRequestHandler (Client client, TimeSpan timeout)
		{
			this._client = client;
			this.Timeout = timeout;
		}

		public void MakeRequest(Batch batch)
        {
            Stopwatch watch = new Stopwatch();

			try
			{
				Uri uri = new Uri(_client.Config.Host + "/v1/import");

				// set the current request time
				batch.SentAt = DateTime.Now.ToString("o");

				string json = JsonConvert.SerializeObject(batch, settings);

				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);

				// Basic Authentication
				// https://segment.io/docs/tracking-api/reference/#authentication
				request.Headers["Authorization"] = BasicAuthHeader(batch.WriteKey, "");

				request.Timeout = (int)Timeout.TotalMilliseconds;
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
                    { "batch size", batch.batch.Count }
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
                        Succeed(batch, watch.ElapsedMilliseconds);
					}
					else
					{
						string responseStr = String.Format("Status Code {0}. ", response.StatusCode);
						responseStr += ReadResponse(response);
                        Fail(batch, new APIException("Unexpected Status Code", responseStr), watch.ElapsedMilliseconds);
					}
				}
			}
			catch (WebException e) 
			{
                watch.Stop();
                Fail(batch, ParseException(e), watch.ElapsedMilliseconds);
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
				_client.Statistics.Failed += 1;
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
				_client.Statistics.Succeeded += 1;
				_client.RaiseSuccess(action);
            }

            Logger.Info("Segment.io request successful.", new Dict
            {
                { "batch id", batch.MessageId },
                { "duration (ms)", duration }
            });
		}
		
		private System.Exception ParseException(WebException e) {
			
			if (e.Response != null && ((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.BadRequest)
			{
				return new APIException("Bad Request", ReadResponse(e.Response));
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
			return  "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(val));
		}
	}
}

