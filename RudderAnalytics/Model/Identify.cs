using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace RudderStack.Model
{
    public class Identify : BaseAction
    {
        [JsonProperty(PropertyName = "traits")]
        public IDictionary<string, object> Traits { get; set; }

        internal Identify(string userId,
                          IDictionary<string, object> traits,
                          RudderOptions options)

            : base("identify", userId, options)
        {
            this.Traits = traits ?? new Traits();
        }
    }
}
