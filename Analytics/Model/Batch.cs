//-----------------------------------------------------------------------
// <copyright file="Batch.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment.Model
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    internal class Batch
    {
        internal Batch()
        {
            this.MessageId = Guid.NewGuid().ToString();
        }

        internal Batch(string writeKey, List<BaseAction> batch) : this()
        {
            this.WriteKey = writeKey;
            this.Actions = batch;
        }

        internal string WriteKey { get; set; }

        [JsonProperty(PropertyName = "messageId")]
        internal string MessageId { get; private set; }

        [JsonProperty(PropertyName = "sentAt")]
        internal string SentAt { get; set; }

        [JsonProperty(PropertyName = "context")]
        internal Context Context { get; set; }

        [JsonProperty(PropertyName = "batch")]
        internal List<BaseAction> Actions { get; set; }
    }
}
