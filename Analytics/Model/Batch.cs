using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Segment.Model
{
    internal class Batch
    {
        internal string WriteKey { get; set; }

		[JsonProperty(PropertyName="messageId")]
		public string MessageId { get; private set; }

		[JsonProperty(PropertyName="sentAt")]
		public string SentAt { get; set; }

        [JsonProperty(PropertyName = "batch")]
        internal List<BaseAction> batch { get; set; }

		[JsonProperty(PropertyName = "context")]
		internal Context Context { get; set; }

      	internal Batch() 
		{ 
			this.MessageId = Guid.NewGuid ().ToString ();
			this.Context = new Context ();
			this.Context.Add ("library", new Dict() {
				{ "name", "Analytics.NET" },
				{ "version", Analytics.VERSION }
			});
		}

        internal Batch(string writeKey, List<BaseAction> batch) : this()
        {
            this.WriteKey = writeKey;
            this.batch = batch;
        }
    }
}
