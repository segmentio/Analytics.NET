using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Segmentio.Model
{
    internal class Batch
    {

        [JsonProperty(PropertyName = "secret")]
        internal string Secret { get; set; }

        [JsonProperty(PropertyName = "batch")]
        internal List<BaseAction> batch { get; set; }
	
	[JsonProperty(PropertyName = "context")]
	internal Context context { get; set; }

        internal Batch() { 
		this.context = new Context ();
		this.context.Add ("library", "analytics-.NET");
	}

        internal Batch(string secret, List<BaseAction> batch) : this()
        {
            this.Secret = secret;
            this.batch = batch;
        }
    }
}
