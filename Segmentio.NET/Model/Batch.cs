using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Segmentio.Model
{
    internal class Batch
    {

        [JsonProperty(PropertyName = "apiKey")]
        internal string ApiKey { get; set; }

        [JsonProperty(PropertyName = "batch")]
        internal List<BaseAction> batch { get; set; }

        internal Batch() { }

        internal Batch(string apiKey, List<BaseAction> batch)
        {
            this.ApiKey = apiKey;
            this.batch = batch;
        }
    }
}
