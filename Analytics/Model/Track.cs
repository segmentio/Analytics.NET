using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Segment.Model
{
    public class Track : BaseAction
    {
        [JsonProperty(PropertyName = "action")]
        private string Action = "track";
		
		[JsonProperty(PropertyName = "userId")]
		public string UserId { get; private set; }

        [JsonProperty(PropertyName = "event")]
        private string EventName { get; set; }

        [JsonProperty(PropertyName = "properties")]
        private Properties Properties { get; set; }

        internal Track(string userId, 
		               string eventName,
            		   Properties properties, 
		               DateTime? timestamp,
		               Context context)

            : base(timestamp, context)
        {
			this.UserId = userId;
            this.EventName = eventName;
            this.Properties = properties ?? new Properties();
        }

        public override string GetAction()
        {
            return Action;
        }
    }
}
