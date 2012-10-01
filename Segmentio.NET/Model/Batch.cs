using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Segmentio.Model
{
    [DataContract]
    internal class Batch
    {
        [DataMember]
        internal string apiKey { get; set; }

        [DataMember]
        internal List<BaseAction> batch { get; set; }

        internal Batch() { }

        internal Batch(string apiKey, List<BaseAction> batch)
        {
            this.apiKey = apiKey;
            this.batch = batch;
        }
    }
}
