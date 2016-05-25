//-----------------------------------------------------------------------
// <copyright file="MonoBehaviourFlushHandler.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_5_3_OR_NEWER

namespace Segment.Flush
{
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;
    using Segment.Model;
    using Segment.Request;
    using UnityEngine;
    using System;

    internal class MonoBehaviourFlushHandler : MonoBehaviour, IFlushHandler
    {
        private const int FlushAmount = 100;

        /// <summary>
        /// Internal message queue.
        /// </summary>
        private LocalStore<BaseAction> dataStore;

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
        
        private DateTime lastFlushTime = DateTime.Now;
        private int actionsSinceLastFlush;

        /// <summary>
        /// Gets the max size of the queue to allow.  This condition prevents high performance 
        /// condition causing this library to eat memory. 
        /// </summary>
        internal int MaxQueueSize { get; private set; }

        internal bool Async { get; private set; }

        public void Process(BaseAction action)
        {
            if (this.dataStore.Count > this.MaxQueueSize)
            {
                var args = new Dict
                {
                    { "message id", action.MessageId },
                    { "queue size", this.dataStore.Count },
                    { "max queue size", this.MaxQueueSize }
                };

                Segment.Logger.Error("Dropped message because queue is too full.", args);
                return;
            }

            this.dataStore.Add(action);

            if (this.Async == false)
            {
                this.Flush();
                return;
            }

            this.actionsSinceLastFlush++;

            if (this.actionsSinceLastFlush >= FlushAmount)
            {
                this.Flush(true);
            }
        }
        
        public void Flush()
        {
            this.Flush(this.Async);
        }

        private void Flush(bool asyc)
        {
            this.lastFlushTime = DateTime.Now;
            this.actionsSinceLastFlush = 0;

            if (this.dataStore.Count == 0)
            {
                // DO NOT CHECKIN
                Debug.Log("Nothing to Flush!");
                return;
            }

            List<BaseAction> current = this.dataStore.PeakTop(FlushAmount);

            // DO NOT CHECKIN
            Debug.LogFormat("Flushing {0} events", current.Count);

            // we have a batch that we're trying to send
            Batch batch = this.batchFactory.Create(current);
            batch.Context = this.context;

            Segment.Logger.Debug("Created flush batch.", new Dict { { "batch size", current.Count } });

            var enumerator = this.requestHandler.MakeRequest(batch);

            if (asyc)
            {
                this.StartCoroutine(enumerator);
            }
            else
            {
                // blocking for the coroutine to finish
                while (enumerator.MoveNext())
                {
                }
            }
        }

        public void Dispose()
        {
            this.OnDestroy();
        }

        public void ClientSuccess(BaseAction action)
        {
            this.dataStore.Remove(action);
        }

        private void Update()
        {
            // if we have a lot to flush, then check every 20 seconds, else wait a minute and a half
            float flushWaitTime = this.dataStore.Count > FlushAmount ? 20 : 90;

            // flush every minute
            if (DateTime.Now.Subtract(this.lastFlushTime).TotalSeconds > flushWaitTime)
            {
                if (Application.internetReachability != NetworkReachability.NotReachable)
                {
                    this.Flush(true);
                }

                this.lastFlushTime = DateTime.Now;
            }
        }

        internal void Initialize(IBatchFactory batchFactory, IRequestHandler requestHandler, int maxQueueSize, bool async)
        {
            this.dataStore = new LocalStore<BaseAction>("analytics");
            this.batchFactory = batchFactory;
            this.requestHandler = requestHandler;
            this.MaxQueueSize = maxQueueSize;
            this.Async = async;
            this.context = this.GetContext();
        }
        
        private void OnApplicationFocus(bool focus)
        {
            // if the app is losing focus then do a non-async flush
            if (focus == false)
            {
                this.FlushAndSave();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            // if the app is being paused then do a non-async flush
            if (pause)
            {
                this.FlushAndSave();
            }
        }

        private void OnDestroy()
        {
            this.FlushAndSave();
        }

        private void FlushAndSave()
        {
            this.Flush(false);
            this.dataStore.Save();
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
