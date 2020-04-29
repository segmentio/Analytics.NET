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

        internal int FlushAt { get; set; }

        internal bool SyncMode { get; set; }

        internal bool Gzip { get; set; }

        internal TimeSpan Timeout { get; set; }

        internal int FlushIntervalInMillis { get; private set; }

        internal int Threads { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host">Endpoint for tracking api for Proxy Service</param>
        /// <param name="proxy"></param>
        /// <param name="timeout"></param>
        /// <param name="maxQueueSize">Queue size</param>
        /// <param name="flushAt">Number of items in a batch to upload</param>
        /// <param name="syncMode">Disable threading and send requests sync</param>
        /// <param name="threads">Count of concurrent internal threads to post data from queue</param>
        /// <param name="flushInterval">The frequency, in seconds, to send data to Segment</param>
        /// <param name="gzip">Compress data w/ gzip before dispatch</param>
        public Config(
            string host = null,
            string proxy = null,
            TimeSpan? timeout = null,
            int? maxQueueSize = null,
            int? flushAt = null,
            bool? syncMode = null,
#if !NET35
            int? threads = null,
#endif
            double? flushInterval = null,
            bool? gzip = null
            )
        {
            this.Host = host ?? Defaults.Host;
            this.Proxy = proxy ?? "";
            this.Timeout = timeout ?? Defaults.Timeout;
            this.MaxQueueSize = maxQueueSize ?? Defaults.MaxQueueCapacity;
            this.FlushAt = flushAt ?? Defaults.FlushAt;
            this.SyncMode = syncMode ?? Defaults.SyncMode;
            this.FlushIntervalInMillis = (int)((flushInterval ?? Defaults.FlushInterval) * 1000);
            this.Gzip = gzip ?? Defaults.Gzip;
#if !NET35
            this.Threads = threads ?? Defaults.Threads;
#endif
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
        [Obsolete("Use the new method SetFlushAt")]
        public Config SetMaxBatchSize(int maxBatchSize)
        {
            return SetFlushAt(maxBatchSize);
        }

        /// <summary>
        /// Sets the maximum amount of messages to send per batch
        /// </summary>
        /// <param name="flushAt"></param>
        /// <returns></returns>
        public Config SetFlushAt(int flushAt)
        {
            this.FlushAt = flushAt;
            return this;
        }

#if !NET35
        /// <summary>
        /// Count of concurrent internal threads to post data from queue
        /// </summary>
        /// <param name="threads"></param>
        /// <returns></returns>
        public Config SetThreads(int threads)
        {
            Threads = threads;
            return this;
        }
#endif

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
        /// <param name="async">True for syncMode flushing, false for blocking flushing</param>
        /// <returns></returns>
        [Obsolete("Use the new method SetSyncMode")]
        public Config SetAsync(bool async)
        {
            return SetSyncMode(!async);
        }

        /// <summary>
        /// Disable threading and send requests sync
        /// </summary>
        /// <param name="syncMode"></param>
        /// <returns></returns>
        public Config SetSyncMode(bool syncMode)
        {
            this.SyncMode = syncMode;
            return this;
        }

        /// <summary>
        /// Sets the API request header uses GZip option.
        /// Enable this when the network is the bottleneck for your application (typically in client side applications).
        /// If useGZip is set, it compresses request content with GZip algorithm
        /// </summary>
        /// <param name="bCompress">True to compress request header, false for no compression</param>
        /// <returns></returns>
        [Obsolete("Use the new method SetGzip")]
        public Config SetRequestCompression(bool bCompress)
        {
            return SetGzip(bCompress);
        }

        /// <summary>
        /// Sets the API request header uses GZip option.
        /// Enable this when the network is the bottleneck for your application (typically in client side applications).
        /// If useGZip is set, it compresses request content with GZip algorithm
        /// </summary>
        /// <param name="gzip">True to compress request header, false for no compression</param>
        /// <returns></returns>
        public Config SetGzip(bool gzip)
        {
            this.Gzip = gzip;
            return this;
        }

#if NET35
        /// <summary>
        /// Set the interval in seconds at which the client should flush events. 
        /// This is relative to the last flush
        /// </summary>
        /// <param name="interval">Time in milliseconds</param>
        /// <returns></returns>
#else
        /// <summary>
        /// Set the interval in seconds at which the client should flush events. 
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
#endif
        public Config SetFlushInterval(double interval)
        {
            this.FlushIntervalInMillis = (int)(interval * 1000);
            return this;
        }
    }
}
