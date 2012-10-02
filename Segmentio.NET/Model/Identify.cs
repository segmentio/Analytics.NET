using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Segmentio.Model
{
    public class Identify : BaseAction
    {

        [JsonProperty(PropertyName = "action")]
        private string Action = "identify";

        [JsonProperty(PropertyName = "context")]
        private Context Context { get; set; }

        [JsonProperty(PropertyName = "traits")]
        private Traits Traits { get; set; }

        internal Identify(string sessionId, string userId, 
            Traits traits, Context context, DateTime? timestamp) 
            : base(sessionId, userId, timestamp)
        {   
            this.Traits = traits ?? new Traits();
            this.Context = context;
        }

        public override string GetAction()
        {
            return Action;
        }
    }
}
