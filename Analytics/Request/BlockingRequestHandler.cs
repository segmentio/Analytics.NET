using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Segment.Exception;
using Segment.Model;
using Segment.Stats;

namespace Segment.Request
{
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

			_httpClient = new HttpClient { Timeout = Timeout };
			_httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
		}

		public async Task MakeRequest(Batch batch)
		{
			Stopwatch watch = new Stopwatch();

			try
			{
				Uri uri = new Uri(_client.Config.Host + "/v1/import");

				// set the current request time
				batch.SentAt = DateTime.Now.ToString("o");

				string json = JsonConvert.SerializeObject(batch);

				// Basic Authentication
				// https://segment.io/docs/tracking-api/reference/#authentication
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", BasicAuthHeader(batch.WriteKey, string.Empty));

				Logger.Info("Sending analytics request to Segment.io ..", new Dict
				{
					{ "batch id", batch.MessageId },
					{ "json size", json.Length },
					{ "batch size", batch.batch.Count }
				});

				watch.Start();

				var response = await _httpClient.PostAsync(uri, new StringContent(json)).ConfigureAwait(false);

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
