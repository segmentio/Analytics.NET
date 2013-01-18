using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Segmentio.Trigger
{
    /// <summary>
    /// Causes the queue to flush once the threshold amount of capacity has been reached
    /// </summary>
    internal class FlushAtTrigger : IFlushTrigger { 
        
        private int threshold;

        internal FlushAtTrigger(int threshold)
        {
            this.threshold = threshold;
        }

        public bool shouldFlush(DateTime lastFlush, int queueSize)
        {
            return queueSize >= this.threshold;
        }
    }
}
