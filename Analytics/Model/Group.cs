using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Segment.Model
{
    using Traits = System.Collections.Generic.IDictionary<string, object>;
    public class Group : BaseAction
    {
		[JsonProperty(PropertyName = "groupId")]
		private string GroupId { get; set; }

		[JsonProperty(PropertyName = "traits")]
		private IDictionary<string, object> Traits { get; set; }

		internal Group(string userId, 
					   string groupId,
                       IDictionary<string, object> traits, 
					   Options options)
			: base("group", userId, options)
        {
			this.GroupId = groupId;
			this.Traits = traits ?? new Dictionary<string, object>();
        }
    }
}
