using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analytics.Trigger
{
    internal interface IFlushTrigger
    {

        bool shouldFlush(DateTime lastFlush, int queueSize);

    }
}
