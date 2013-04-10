using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Segmentio
{
    /// <summary>
    /// Options required to initialize the client
    /// </summary>
    public class Options
    {

        /// <summary>
        /// The REST API endpoint
        /// </summary>
        internal string Host { get; set; }

        internal int MaxQueueSize { get; set; }

		internal bool Async { get; set; }

		internal TimeSpan Timeout { get; set; }

        public Options()
        {
            this.Host = Defaults.Host;
			this.Timeout = Defaults.Timeout;
            this.MaxQueueSize = Defaults.MaxQueueCapacity;
			this.Async = Defaults.Async;
        }

        /// <summary>
        /// Sets the amount of items that can be added to the queue before it flushes
        /// </summary>
        /// <param name="flushAt"></param>
        /// <returns></returns>
		[Obsolete("SetFlushAt is no longer needed, async flush will now happen continuously in the background.")]
        public Options SetFlushAt(int flushAt)
        {
            return this;
        }

        /// <summary>
        /// Sets the amount of milliseconds that can pass before the next flush
        /// </summary>
        /// <param name="flushAfter"></param>
        /// <returns></returns>
		[Obsolete("SetFlushAfter is no longer needed, async flush will now happen continuously in the background.")]
        public Options SetFlushAfter(TimeSpan flushAfter)
        {
            return this;
        }

		/// <summary>
		/// Sets the maximum amount of timeout on the HTTP request flushes to the server.
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public Options SetTimeout(TimeSpan timeout)
		{
			this.Timeout = timeout;
			return this;
		}
		
		/// <summary>
		/// Sets the maximum amount of items that can be in the queue before no more are accepted.
		/// </summary>
		/// <param name="maxQueueSize"></param>
		/// <returns></returns>
		public Options SetMaxQueueSize(int maxQueueSize)
		{
			this.MaxQueueSize = maxQueueSize;
			return this;
		}

        /// <summary>
        /// Sets whether the flushing to the server is synchronous or asynchronous.
		/// 
		/// True is the default and will allow your calls to Analytics.Client.Identify(...), Track(...), etc
		/// to return immediately and to be queued to be flushed on a different thread.
		/// 
		/// False is convenient for testing but should not be used in production. False will cause the 
		/// HTTP requests to happen immediately.
		/// 
        /// </summary>
		/// <param name="async">True for async flushing, false for blocking flushing</param>
        /// <returns></returns>
        public Options SetAsync(bool async)
        {
			this.Async = async;
            return this;
        }
    }
}
