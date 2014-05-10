using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Segment.Model
{
	public class Alias : BaseAction
	{
		
		[JsonProperty(PropertyName = "action")]
		private string Action = "alias";

		[JsonProperty(PropertyName = "previousId")]
		private string PreviousId { get; set; }

		[JsonProperty(PropertyName = "userId")]
		private string UserId { get; set; }
		
		internal Alias(string previousId, string userId, 
			Options options, DateTime? timestamp)
			: base(timestamp, options)
		{
			this.PreviousId = previousId;
			this.UserId = userId;
		}
		
		public override string GetAction()
		{
			return Action;
		}
	}
}

