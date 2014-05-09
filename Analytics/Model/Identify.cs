using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Segment.Model
{
    public class Identify : BaseAction
    {

        [JsonProperty(PropertyName = "action")]
        private string Action = "identify";
		
		[JsonProperty(PropertyName = "userId")]
		public string UserId { get; private set; }

        [JsonProperty(PropertyName = "traits")]
        private Traits Traits { get; set; }

        internal Identify(string userId,
		                  Traits traits, 
		                  DateTime? timestamp,
		                  Context context)
	
			: base(timestamp, context)
        {
			this.UserId = userId;
            this.Traits = traits ?? new Traits();
        }

        public override string GetAction()
        {
            return Action;
        }
    }
}
