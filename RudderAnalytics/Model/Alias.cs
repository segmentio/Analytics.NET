using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace RudderStack.Model
{
    public class Alias : BaseAction
    {
        [JsonProperty(PropertyName = "previousId")]
        public string PreviousId { get; set; }

        internal Alias(string previousId, string userId, RudderOptions options)
            : base("alias", userId, options)
        {
            this.PreviousId = previousId;
        }
    }
}
