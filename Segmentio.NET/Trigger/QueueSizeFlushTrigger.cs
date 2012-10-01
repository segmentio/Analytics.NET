using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Segmentio.Trigger
{
    internal class QueueSizeFlushTrigger : IFlushTrigger { 
        
        private int threshold;

        internal QueueSizeFlushTrigger(int threshold)
        {
            this.threshold = threshold;
        }

        public bool shouldFlush(DateTime lastFlush, int queueSize)
        {
            return queueSize >= this.threshold;
        }
    }
}
