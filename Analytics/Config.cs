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

        internal string UserAgentHeader { get; set; }

        internal string Proxy { get; set; }

        internal int MaxQueueSize { get; set; }

        internal int MaxBatchSize { get; set; }

        internal bool Async { get; set; }

        internal bool CompressRequest { get; set; }

        internal TimeSpan Timeout { get; set; }

        internal int FlushIntervalInMillis { get; private set; }
        internal int Threads { get; set; }

        public Config()
        {
            this.Host = Defaults.Host;
            this.Proxy = "";
            this.Timeout = Defaults.Timeout;
            this.MaxQueueSize = Defaults.MaxQueueCapacity;
            this.MaxBatchSize = Defaults.MaxBatchSize;
            this.Async = Defaults.Async;
            this.FlushIntervalInMillis = Defaults.FlushIntervalInMillis;
            this.Threads = Defaults.Threads;
            this.UserAgentHeader = Defaults.UserAgentHeader;
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
        /// Sets the maximum amount of messages to send per batch
        /// </summary>
        /// <param name="maxBatchSize"></param>
        /// <returns></returns>
        public Config SetMaxBatchSize(int maxBatchSize)
        {
            this.MaxBatchSize = maxBatchSize;
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

        /// <summary>
        /// Sets the API request header uses GZip option.
        /// Enable this when the network is the bottleneck for your application (typically in client side applications).
        /// If useGZip is set, it compresses request content with GZip algorithm
        /// </summary>
        /// <param name="bCompress">True to compress request header, false for no compression</param>
        /// <returns></returns>
        public Config SetRequestCompression(bool bCompress)
        {
            this.CompressRequest = bCompress;
            return this;
        }


        /// <summary>
        /// Set the interval at which the client should flush events. 
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public Config SetFlushIntervalInMillis(int interval)
        {
            this.FlushIntervalInMillis = interval;
            return this;
        }
    }
}
