//-----------------------------------------------------------------------
// <copyright file="Config.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment
{
    using System;

    /// <summary>
    /// Config required to initialize the client.
    /// </summary>
    public class Config
    {
        public Config()
        {
            this.Host = Defaults.Host;
            this.Timeout = Defaults.Timeout;
            this.MaxQueueSize = Defaults.MaxQueueCapacity;
            this.Async = Defaults.Async;
        }

        /// <summary>
        /// Gets or sets the REST API endpoint.
        /// </summary>
        internal string Host { get; set; }

        internal int MaxQueueSize { get; set; }

        internal bool Async { get; set; }

        internal TimeSpan Timeout { get; set; }
        
        /// <summary>
        /// Sets the maximum amount of timeout on the HTTP request flushes to the server.
        /// </summary>
        /// <param name="timeout">The timeout period.</param>
        /// <returns>This Config.</returns>
        public Config SetTimeout(TimeSpan timeout)
        {
            this.Timeout = timeout;
            return this;
        }
        
        /// <summary>
        /// Sets the maximum amount of items that can be in the queue before no more are accepted.
        /// </summary>
        /// <param name="maxQueueSize">The maximum queue size.</param>
        /// <returns>This Config.</returns>
        public Config SetMaxQueueSize(int maxQueueSize)
        {
            this.MaxQueueSize = maxQueueSize;
            return this;
        }

        /// <summary>
        /// Sets whether the flushing to the server is synchronous or asynchronous.
        /// <para/>
        /// True is the default and will allow your calls to Analytics.Client.Identify(...), Track(...), etc
        /// to return immediately and to be queued to be flushed on a different thread.
        /// <para/>
        /// False is convenient for testing but should not be used in production. False will cause the 
        /// HTTP requests to happen immediately.
        /// </summary>
        /// <param name="async">True for async flushing, false for blocking flushing.</param>
        /// <returns>This Config.</returns>
        public Config SetAsync(bool async)
        {
            this.Async = async;
            return this;
        }
    }
}
