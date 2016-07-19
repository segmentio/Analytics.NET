//-----------------------------------------------------------------------
// <copyright file="IBatchFactory.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment.Flush
{
    using System.Collections.Generic;
    using Segment.Model;

    internal interface IBatchFactory
    {
        Batch Create(List<BaseAction> actions);
    }
}
