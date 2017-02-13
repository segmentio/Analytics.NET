﻿using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Segment.Model
{
    using Properties = System.Collections.Generic.IDictionary<string, object>;
    public class Track : BaseAction
    {
        [JsonProperty(PropertyName = "event")]
        private string EventName { get; set; }

        [JsonProperty(PropertyName = "properties")]
        private IDictionary<string, object> Properties { get; set; }

        internal Track(string userId, 
		               string eventName,
                       IDictionary<string, object> properties, 
					   Options options)

			: base("track", userId, options)
        {
            this.EventName = eventName;
            this.Properties = properties ?? new Dictionary<string, object>();
        }
    }
}
