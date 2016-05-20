//-----------------------------------------------------------------------
// <copyright file="Alias.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment.Model
{
    using Newtonsoft.Json;

    public class Alias : BaseAction
    {
        internal Alias(string previousId, string userId, Options options)
            : base("alias", userId, options)
        {
            this.PreviousId = previousId;
        }

        [JsonProperty(PropertyName = "previousId")]
        private string PreviousId { get; set; }
    }
}
