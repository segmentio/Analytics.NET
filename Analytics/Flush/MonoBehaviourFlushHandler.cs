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
        /// The context for this device.
        /// </summary>
        private Context context;

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
            batch.Context = this.context;

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
            this.context = this.GetContext();
        }
        
        private void Update()
        {
            if (this.queue.Count > this.MaxQueueSize)
            {
                this.Flush();
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            // TODO [bgish] - save off to disk?
        }

        private void OnApplicationPause(bool pause)
        {
            // TODO [bgish] - save off to disk?
        }

        private void OnDestroy()
        {
            // TODO [bgish] - save off to disk?
        }

        private void Awake()
        {
            GameObject.DontDestroyOnLoad(this.gameObject);
        }

        private Context GetContext()
        {
            var app = new Dictionary<string, object>();
            app.Add("name", Application.productName);  
            app.Add("version", Application.version);                                                     

            var device = new Dictionary<string, object>();
            device.Add("id", SystemInfo.deviceUniqueIdentifier);
            device.Add("model", SystemInfo.deviceModel);
            device.Add("name", SystemInfo.deviceName);
            device.Add("type", Application.platform.ToString());

            var os = new Dictionary<string, object>();  
            os.Add("name", SystemInfo.operatingSystem);

            var screen = new Dictionary<string, object>();
            screen.Add("width", UnityEngine.Screen.width);
            screen.Add("height", UnityEngine.Screen.height);
            screen.Add("density", UnityEngine.Screen.dpi);

            var context = new Context();
            context.Add("app", app);
            context.Add("device", device);
            context.Add("os", os);
            context.Add("screen", screen);

            return context;
        }
    }
}

#endif
