//-----------------------------------------------------------------------
// <copyright file="Group.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment.Model
{
    using Newtonsoft.Json;

    public class Group : BaseAction
    {
        internal Group(string userId, string groupId, Traits traits, Options options)
            : base("group", userId, options)
        {
            this.GroupId = groupId;
            this.Traits = traits ?? new Traits();
        }

        [JsonProperty(PropertyName = "groupId")]
        private string GroupId { get; set; }

        [JsonProperty(PropertyName = "traits")]
        private Traits Traits { get; set; }
    }
}
