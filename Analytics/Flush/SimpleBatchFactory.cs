//-----------------------------------------------------------------------
// <copyright file="SimpleBatchFactory.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment.Flush
{
    using System.Collections.Generic;
    using Segment.Model;

    internal class SimpleBatchFactory : IBatchFactory
    {
        private string writeKey;

        internal SimpleBatchFactory(string writeKey)
        {
            this.writeKey = writeKey;
        }

        public Batch Create(List<BaseAction> actions) 
        {
            return new Batch(this.writeKey, actions);
        }
    }
}
