using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Segment
{
    /// <summary>
    /// Config required to initialize the client
    /// </summary>
	public class Config
    {

        /// <summary>
        /// The REST API endpoint
        /// </summary>
        internal string Host { get; set; }

		internal string Proxy { get; set; }

        internal int MaxQueueSize { get; set; }

		internal bool Async { get; set; }

		internal TimeSpan Timeout { get; set; }

		public Config()
        {
            this.Host = Defaults.Host;
			this.Proxy = "";
			this.Timeout = Defaults.Timeout;
            this.MaxQueueSize = Defaults.MaxQueueCapacity;
			this.Async = Defaults.Async;
        }

		/// <summary>
		/// Set the API host server address, instead of default server "https://api.segment.io"
		/// </summary>
		/// <param name="host">Host server url</param>
		/// <returns></returns>
		public Config SetHost(string host)
		{
			this.Host = host;
			return this;
		}

		/// <summary>
		/// Set the proxy server Uri
		/// </summary>
		/// <param name="proxy">Proxy server Uri</param>
		/// <returns></returns>
		public Config SetProxy(string proxy)
		{
			this.Proxy = proxy;
			return this;
		}

		/// <summary>
		/// Sets the maximum amount of timeout on the HTTP request flushes to the server.
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public Config SetTimeout(TimeSpan timeout)
		{
			this.Timeout = timeout;
			return this;
		}
		
		/// <summary>
		/// Sets the maximum amount of items that can be in the queue before no more are accepted.
		/// </summary>
		/// <param name="maxQueueSize"></param>
		/// <returns></returns>
		public Config SetMaxQueueSize(int maxQueueSize)
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
		public Config SetAsync(bool async)
        {
			this.Async = async;
            return this;
        }
    }
}
