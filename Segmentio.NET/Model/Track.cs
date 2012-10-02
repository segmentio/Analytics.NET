using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Segmentio.Model
{
    public class Track : BaseAction
    {
        [JsonProperty(PropertyName = "action")]
        private string Action = "track";

        [JsonProperty(PropertyName = "event")]
        private string EventName { get; set; }

        [JsonProperty(PropertyName = "properties")]
        private Properties Properties { get; set; }

        internal Track(string sessionId, string userId, string eventName, 
            Properties properties, DateTime? timestamp) 
            : base(sessionId, userId, timestamp)
        {
            this.EventName = eventName;
            this.Properties = properties ?? new Properties();
        }

        public override string GetAction()
        {
            return Action;
        }
    }
}
