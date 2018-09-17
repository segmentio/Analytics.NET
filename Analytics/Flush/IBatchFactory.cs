using System;
using System.Collections.Generic;

using Segment.Model;

namespace Segment.Flush
{
    internal interface IBatchFactory
    {
        Batch Create(List<BaseAction> actions);
    }
}
