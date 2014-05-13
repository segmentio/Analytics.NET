using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Segment.Model
{
    public class Identify : BaseAction
    {
		[JsonProperty(PropertyName = "userId")]
		public string UserId { get; private set; }

        [JsonProperty(PropertyName = "traits")]
		public Traits Traits { get; set; }

        internal Identify(string userId,
		                  Traits traits, 
						  Options options)
	
			: base("identify", options)
        {
			this.UserId = userId;
            this.Traits = traits ?? new Traits();
        }
    }
}
