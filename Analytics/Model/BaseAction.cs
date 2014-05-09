using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Segment.Model
{
    public abstract class BaseAction
    {

		[JsonProperty(PropertyName="timestamp")]
		public string Timestamp { get; private set; }
		
		[JsonProperty(PropertyName = "context")]
		private Context Context { get; set; }

		public BaseAction(DateTime? timestamp, Context context)
		{
			if (timestamp.HasValue) this.Timestamp = timestamp.Value.ToString("o");
			this.Context = context;
        }

        /// <summary>
        /// Returns the string name representing this action based on the Segment.io REST API.
        /// A track returns "track", etc..
        /// </summary>
        /// <returns></returns>
        public abstract string GetAction();
    }
}
