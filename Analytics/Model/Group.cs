
using Newtonsoft.Json;

namespace Segment.Model
{
	public class Group : BaseAction
    {
		[JsonProperty(PropertyName = "groupId")]
		private string GroupId { get; set; }

		[JsonProperty(PropertyName = "traits")]
		private Traits Traits { get; set; }

		internal Group(string userId, string groupId, Traits traits, Options options) : base("group", userId, options)
        {
			this.GroupId = groupId;
			this.Traits = traits ?? new Traits();
        }
    }
}
