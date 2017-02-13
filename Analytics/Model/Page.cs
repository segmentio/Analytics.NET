﻿using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Segment.Model
{
    using Properties = System.Collections.Generic.IDictionary<string, object>;
    public class Page : BaseAction
    {
		[JsonProperty(PropertyName = "name")]
        private string Name { get; set; }

		[JsonProperty(PropertyName = "category")]
		private string Category { get; set; }

        [JsonProperty(PropertyName = "properties")]
        private IDictionary<string, object> Properties { get; set; }

		internal Page(string userId, 
					  string name,
					  string category,
                      IDictionary<string, object> properties, 
					  Options options)

			: base("page", userId, options)
		{
			this.Name = name;
			this.Category = category;
            this.Properties = properties ?? new Dictionary<string, object>();
        }
    }
}
