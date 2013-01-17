using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Analytics.Model
{
    internal class Batch
    {

        [JsonProperty(PropertyName = "secret")]
        internal string Secret { get; set; }

        [JsonProperty(PropertyName = "batch")]
        internal List<BaseAction> batch { get; set; }

        internal Batch() { }

        internal Batch(string secret, List<BaseAction> batch)
        {
            this.Secret = secret;
            this.batch = batch;
        }
    }
}
