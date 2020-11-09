using System;
using System.Collections.Generic;

using RudderStack.Model;

namespace RudderStack.Flush
{
    internal interface IBatchFactory
    {
        Batch Create(List<BaseAction> actions);
    }
}

