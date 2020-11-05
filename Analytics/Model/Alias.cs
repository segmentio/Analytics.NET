using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace RudderStack.Model
{
    public class Alias : BaseAction
    {
        [JsonProperty(PropertyName = "previousId")]
        private string PreviousId { get; set; }

        internal Alias(string previousId, string userId, Options options)
            : base("alias", userId, options)
        {
            this.PreviousId = previousId;
        }
    }
}
