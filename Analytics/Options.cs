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

        /// <summary>
        /// The amount of items that can be added to the queue without flushing
        /// </summary>
        internal int FlushAt { get; set; }

        /// <summary>
        /// The amount of time that can pass since the last flush
        /// </summary>
        internal TimeSpan FlushAfter { get; set; }

        internal int MaxQueueSize { get; set; }

        public Options()
        {
            this.Host = Defaults.Host;
            this.FlushAt = Defaults.FlushAt;
            this.FlushAfter = Defaults.FlushAfter;
            this.MaxQueueSize = Defaults.MaxQueueCapacity;
        }

        /// <summary>
        /// Sets the amount of items that can be added to the queue before it flushes
        /// </summary>
        /// <param name="flushAt"></param>
        /// <returns></returns>
        public Options SetFlushAt(int flushAt)
        {
            this.FlushAt = flushAt;
            return this;
        }

        /// <summary>
        /// Sets the amount of milliseconds that can pass before the next flush
        /// </summary>
        /// <param name="flushAfter"></param>
        /// <returns></returns>
        public Options SetFlushAfter(TimeSpan flushAfter)
        {
            this.FlushAfter = flushAfter;
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
    }
}
