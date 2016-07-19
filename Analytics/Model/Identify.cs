//-----------------------------------------------------------------------
// <copyright file="Identify.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment.Model
{
    using Newtonsoft.Json;

    public class Identify : BaseAction
    {
        internal Identify(string userId, Traits traits, Options options)
            : base("identify", userId, options)
        {
            this.Traits = traits ?? new Traits();
        }

        [JsonProperty(PropertyName = "traits")]
        public Traits Traits { get; set; }
    }
}
