using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Segmentio.Model
{
    internal class Batch
    {

        [JsonProperty(PropertyName = "writeKey")]
        internal string WriteKey { get; set; }

        [JsonProperty(PropertyName = "batch")]
        internal List<BaseAction> batch { get; set; }
	
	[JsonProperty(PropertyName = "context")]
	internal Context context { get; set; }

        internal Batch() { 
		this.context = new Context ();
		this.context.Add ("library", "analytics-.NET");
	}

        internal Batch(string writeKey, List<BaseAction> batch) : this()
        {
            this.WriteKey = writeKey;
            this.batch = batch;
        }
    }
}
