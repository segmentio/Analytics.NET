using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Segmentio.Model
{
	public class Alias : BaseAction
	{
		
		[JsonProperty(PropertyName = "action")]
		private string Action = "alias";

		[JsonProperty(PropertyName = "from")]
		private string From { get; set; }

		[JsonProperty(PropertyName = "to")]
		private string To { get; set; }
		
		internal Alias(string from, string to, DateTime? timestamp, Context context)
			: base(timestamp, context)
		{
			this.From = from;
			this.To = to;
		}
		
		public override string GetAction()
		{
			return Action;
		}
	}
}

