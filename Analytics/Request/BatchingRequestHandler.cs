using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Threading;

using Newtonsoft.Json;

using Segmentio.Model;
using Segmentio.Trigger;
using Segmentio.Exception;

namespace Segmentio.Request
{
    internal class BatchingRequestHandler : IRequestHandler
    {
        
        private string secret;
        private Queue<BaseAction> queue;
        private DateTime lastFlush;
        
		private JsonSerializerSettings settings = new JsonSerializerSettings() {

		};

        private volatile bool flushActive;

        /// <summary>
        /// The amount of actions to flush to the server at a time.
        /// </summary>
        public int BatchIncrement { get; set; }
        
        /// <summary>
        /// The max size of the queue to allow
        /// This condition prevents high performance condition causing
        /// this library to eat memory. 
        /// </summary>
        public int MaxSize { get; set; }

        /// <summary>
        /// The maximum amount of milliseconds to wait before calling
        /// the HTTP flush a timeout failure.
        /// </summary>
        public int Timeout { get; set; }

        private Client client;
        private IFlushTrigger[] triggers;

        public BatchingRequestHandler(IFlushTrigger[] triggers)
        {
            queue = new Queue<BaseAction>();

            this.triggers = triggers;
            
            this.BatchIncrement = 50;
            this.MaxSize = 1000000;

            this.Timeout = 3000;
        }

        public void Initialize(Client client, string secret)
        {
            this.client = client;
            this.secret = secret;

            this.BatchIncrement = client.Options.FlushAt;
            this.MaxSize = client.Options.MaxQueueSize;
        }

        public void Process(BaseAction action)
        {
            int size = queue.Count;

            if (size > MaxSize)
            {
                // drop the message
                // TODO: log it
            }
            else
            {
                lock (queue)
                {
                    queue.Enqueue(action);
                    size += 1;
                }

                client.Statistics.Submitted += 1;
            }

            foreach (IFlushTrigger trigger in triggers)
            {
                if (trigger.shouldFlush(lastFlush, size))
                {
                    AsyncFlush();
                    break;
                }
            }
        }

        /// <summary>
        /// Queues a Flush on the Thread Pool
        /// </summary>
        public void AsyncFlush()
        {
            ThreadPool.QueueUserWorkItem(o => this.Flush());
        }

        /// <summary>
        /// Synchronizes over the queue and dequeues to create a batch JSON request.
        /// These operations happen on the calling thread, but the actual HTTP request
        /// is performed asynchronously on another thread.
        /// </summary>
        public void Flush()
        {
            flushActive = true;

            List<BaseAction> actions = new List<BaseAction>();

            lock (queue) 
            {
                for (int i = 0; i < BatchIncrement; i += 1)
                {
                    if (queue.Count == 0) break;

                    BaseAction action = queue.Dequeue();
                    actions.Add(action);
                }
            }

            if (actions.Count > 0)
            {
                Batch batch = new Batch(secret, actions);
                MakeRequest(batch);

                lastFlush = DateTime.Now;
                client.Statistics.Flushed += 1;
            }

            flushActive = false;
        }

        private void MakeRequest(Batch batch)
        {
            try
            {
                Uri uri = new Uri(client.Options.Host + "/v1/import");

				string json = JsonConvert.SerializeObject(batch, settings);

                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);

                request.Timeout = Timeout;
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

                BatchState state = new BatchState(request, batch);

                request.BeginGetResponse(FinishWebRequest, state);
            }
            catch (System.Exception e)
            {
                foreach (BaseAction action in batch.batch)
                {
                    client.Statistics.Failed += 1;
                    client.RaiseFailure(action, e);
                }
            }
        }

        private void FinishWebRequest(IAsyncResult result)
        {
            BatchState state = (BatchState) result.AsyncState;
            HttpWebRequest request = state.Request;
            try
            {
                using (var response = (HttpWebResponse)request.EndGetResponse(result))
                {

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // log success
                        foreach (BaseAction action in state.Batch.batch)
                        {
                            client.Statistics.Succeeded += 1;
                            client.RaiseSuccess(action);
                        }
                    }
                    else
                    {
                        string responseStr = String.Format("Status Code {0}. ", response.StatusCode);

                        responseStr += ReadResponse(response);

                        foreach (BaseAction action in state.Batch.batch)
                        {
                            client.Statistics.Failed += 1;
                            client.RaiseFailure(action, new APIException("Unexpected Status Code", responseStr));
                        }
                    }
                }
            }
            catch (WebException e)
            {
                foreach (BaseAction action in state.Batch.batch)
                {
                    client.Statistics.Failed += 1;
                    client.RaiseFailure(action, ParseException(e));
                }
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

