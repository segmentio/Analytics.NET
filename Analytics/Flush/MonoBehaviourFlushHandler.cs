
#if UNITY_5_3_OR_NEWER

using System.Collections.Generic;
using Segment.Request;
using Segment.Model;
using UnityEngine;

namespace Segment.Flush
{
    internal class MonoBehaviourFlushHandler : MonoBehaviour, IFlushHandler
    {
        /// <summary>
        /// Internal message queue
        /// </summary>
        private BlockingQueue<BaseAction> queue;

        /// <summary>
        /// Creates a series of actions into a batch that we can send to the server
        /// </summary>
        private IBatchFactory batchFactory;

        /// <summary>
        /// Performs the actual HTTP request to our server
        /// </summary>
        private IRequestHandler requestHandler;

        /// <summary>
        /// The max size of the queue to allow
        /// This condition prevents high performance condition causing
        /// this library to eat memory. 
        /// </summary>
		internal int MaxQueueSize { get; private set; }

        internal bool Async { get; private set; }

        internal void Initialize(IBatchFactory batchFactory, IRequestHandler requestHandler, int maxQueueSize, bool async)
        {
            this.queue = new BlockingQueue<BaseAction>();
            this.batchFactory = batchFactory;
            this.requestHandler = requestHandler;
            this.MaxQueueSize = maxQueueSize;
            this.Async = async;
        }

        public void Process(BaseAction action)
        {
            if (this.Async == false)
            {
                this.queue.Enqueue(action);
                this.Flush();
                return;
            }

            int size = queue.Count;

            if (size > MaxQueueSize)
            {
                Logger.Warn("Dropped message because queue is too full.", new Dict
                {
                    { "message id", action.MessageId },
                    { "queue size", queue.Count },
                    { "max queue size", MaxQueueSize }
                });
            }
            else
            {
                queue.Enqueue(action);
            }
        }

        /// <summary>
        /// Blocks until all the messages are flushed
        /// </summary>
        public void Flush()
        {
            if (queue.Count == 0)
            {
                return;
            }

            List<BaseAction> current = new List<BaseAction>();

            while (queue.Count > 0 && current.Count < this.MaxQueueSize)
            {
                BaseAction action = queue.Dequeue();

                if (action != null)
                {
                    current.Add(action);
                }
            }

            // we have a batch that we're trying to send
            Batch batch = batchFactory.Create(current);

            Logger.Debug("Created flush batch.", new Dict
            {
                { "batch size", current.Count }
            });

            // make the request here
            requestHandler.MakeRequest(batch);
        }

        /// <summary>
        /// Loops on the flushing thread and processes the message queue
        /// </summary>
        private void Update()
        {
            if (queue.Count > this.MaxQueueSize)
            {
                this.Flush();
            }
        }

        private void OnDestroy()
        {
            // TODO [bgish] - save off to disk?
        }

        public void Dispose()
        {
            this.OnDestroy();
        }

        private void Awake()
        {
            GameObject.DontDestroyOnLoad(this.gameObject);
        }
    }
}

#endif
