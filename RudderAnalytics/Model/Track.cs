using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace RudderStack.Model
{
    public class Track : BaseAction
    {
        [JsonProperty(PropertyName = "event")]
        public string EventName { get; set; }

        [JsonProperty(PropertyName = "properties")]
        public IDictionary<string, object> Properties { get; set; }

        internal Track(string userId,
                       string eventName,
                       IDictionary<string, object> properties,
                       RudderOptions options)

            : base("track", userId, options)
        {
            this.EventName = eventName;
            this.Properties = properties ?? new Properties();
        }
    }
}
