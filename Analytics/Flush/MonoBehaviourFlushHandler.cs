//-----------------------------------------------------------------------
// <copyright file="MonoBehaviourFlushHandler.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_5_3_OR_NEWER

namespace Segment.Flush
{
    using System.Collections.Generic;
    using Segment.Model;
    using Segment.Request;
    using UnityEngine;

    internal class MonoBehaviourFlushHandler : MonoBehaviour, IFlushHandler
    {
        /// <summary>
        /// Internal message queue.
        /// </summary>
        private BlockingQueue<BaseAction> queue;

        /// <summary>
        /// Creates a series of actions into a batch that we can send to the server.
        /// </summary>
        private IBatchFactory batchFactory;

        /// <summary>
        /// Performs the actual HTTP request to our server.
        /// </summary>
        private IRequestHandler requestHandler;

        /// <summary>
        /// Gets the max size of the queue to allow.  This condition prevents high performance 
        /// condition causing this library to eat memory. 
        /// </summary>
        internal int MaxQueueSize { get; private set; }

        internal bool Async { get; private set; }

        public void Process(BaseAction action)
        {
            if (this.Async == false)
            {
                this.queue.Enqueue(action);
                this.Flush();
                return;
            }

            int size = this.queue.Count;

            if (size > this.MaxQueueSize)
            {
                var args = new Dict { { "message id", action.MessageId }, { "queue size", this.queue.Count }, { "max queue size", this.MaxQueueSize } };
                Segment.Logger.Warn("Dropped message because queue is too full.", args);
            }
            else
            {
                this.queue.Enqueue(action);
            }
        }

        /// <summary>
        /// Blocks until all the messages are flushed.
        /// </summary>
        public void Flush()
        {
            if (this.queue.Count == 0)
            {
                return;
            }

            List<BaseAction> current = new List<BaseAction>();

            while (this.queue.Count > 0 && current.Count < this.MaxQueueSize)
            {
                BaseAction action = this.queue.Dequeue();

                if (action != null)
                {
                    current.Add(action);
                }
            }

            // we have a batch that we're trying to send
            Batch batch = this.batchFactory.Create(current);

            Segment.Logger.Debug("Created flush batch.", new Dict { { "batch size", current.Count } });

            // make the request here
            this.requestHandler.MakeRequest(batch);
        }

        public void Dispose()
        {
            this.OnDestroy();
        }

        internal void Initialize(IBatchFactory batchFactory, IRequestHandler requestHandler, int maxQueueSize, bool async)
        {
            this.queue = new BlockingQueue<BaseAction>();
            this.batchFactory = batchFactory;
            this.requestHandler = requestHandler;
            this.MaxQueueSize = maxQueueSize;
            this.Async = async;
        }

        /// <summary>
        /// Loops on the flushing thread and processes the message queue.
        /// </summary>
        private void Update()
        {
            if (this.queue.Count > this.MaxQueueSize)
            {
                this.Flush();
            }
        }

        private void OnDestroy()
        {
            // TODO [bgish] - save off to disk?
        }

        private void Awake()
        {
            GameObject.DontDestroyOnLoad(this.gameObject);
        }
    }
}

#endif
