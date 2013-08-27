using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Threading;
using System.Web;

using Newtonsoft.Json;

using Segmentio.Model;
using Segmentio.Exception;

namespace Segmentio.Request
{
	internal class BlockingRequestHandler : IRequestHandler
	{
		/// <summary>
		/// JSON serialization settings
		/// </summary>
		private JsonSerializerSettings settings = new JsonSerializerSettings() {
			Converters = new List<JsonConverter> { new ContextSerializer() }
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
			try
			{
				Uri uri = new Uri(_client.Options.Host + "/v1/import");
				
				string json = JsonConvert.SerializeObject(batch, settings);
				
				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);
				
				request.Timeout = (int)Timeout.TotalMilliseconds;
				request.ContentType = "application/json";
				request.Method = "POST";

				// do not use the expect 100-continue behavior
				request.ServicePoint.Expect100Continue = false;
				// buffer the data before sending, ok since we send all in one shot
				request.AllowWriteStreamBuffering = true;

				using (var requestStream = request.GetRequestStream())
				{
					using (StreamWriter writer = new StreamWriter(requestStream))
					{
						writer.Write(json);
					}
				}

				using (var response = (HttpWebResponse)request.GetResponse())
				{
					
					if (response.StatusCode == HttpStatusCode.OK)
					{
						Succeed(batch);
					}
					else
					{
						string responseStr = String.Format("Status Code {0}. ", response.StatusCode);
						
						responseStr += ReadResponse(response);
						
						Fail(batch, new APIException("Unexpected Status Code", responseStr));
					}
				}
			}
			catch (WebException e) 
			{
				Fail(batch, ParseException(e));
			}
			catch (System.Exception e)
			{
				Fail(batch, e);
			}
		}

		private void Fail(Batch batch, System.Exception e) 
		{
			foreach (BaseAction action in batch.batch)
			{
				_client.Statistics.Failed += 1;
				_client.RaiseFailure(action, e);
			}
		}

		
		private void Succeed(Batch batch) 
		{
			foreach (BaseAction action in batch.batch)
			{
				_client.Statistics.Succeeded += 1;
				_client.RaiseSuccess(action);
			}
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
	}
}

