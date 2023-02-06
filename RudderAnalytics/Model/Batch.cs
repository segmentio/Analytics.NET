using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace RudderStack.Model
{
    internal class Batch
    {
        internal string WriteKey { get; set; }

        [JsonProperty(PropertyName = "messageId")]
        internal string MessageId { get; private set; }

        [JsonProperty(PropertyName = "sentAt")]
        internal string SentAt { get; set; }

        [JsonProperty(PropertyName = "batch")]
        internal List<BaseAction> batch { get; set; }

        internal Batch()
        {
            this.MessageId = Guid.NewGuid().ToString();
        }

        internal Batch(string writeKey, List<BaseAction> batch) : this()
        {
            this.WriteKey = writeKey;
            this.batch = batch;
        }
    }
}
