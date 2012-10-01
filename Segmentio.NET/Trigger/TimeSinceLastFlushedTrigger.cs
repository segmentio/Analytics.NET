using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Segmentio.Trigger
{
    internal class TimeSinceLastFlushedTrigger : IFlushTrigger
    {

        private TimeSpan timespan;

        internal TimeSinceLastFlushedTrigger(TimeSpan timespan)
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
