using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Segmentio.Trigger
{
    /// <summary>
    /// Trigger that causes the queue to flush once the amount of milliseconds has passed since the last flush
    /// </summary>
    internal class FlushAfterTrigger : IFlushTrigger
    {

        private TimeSpan timespan;

        internal FlushAfterTrigger(TimeSpan timespan)
        {
            this.timespan = timespan;
        }

        public bool shouldFlush(DateTime lastFlush, int queueSize)
        {
            if (lastFlush == null)
            {
                return queueSize > 0;
            }
            else
            {
                return DateTime.Now.Subtract(lastFlush).CompareTo(timespan) > 0;
            }
        }

    }
}
