using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Segment.Model
{
    public abstract class BaseAction
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName="messageId")]
        public string MessageId { get; private set; }

        [JsonProperty(PropertyName="timestamp")]
        public string Timestamp { get; private set; }

        [JsonProperty(PropertyName="context")]
        public Context Context { get; set; }

        [JsonProperty(PropertyName="integrations")]
        public Dict Integrations { get; set; }

        [JsonProperty(PropertyName="anonymousId")]
        public string AnonymousId { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; private set; }

        [JsonIgnore]
        public int Size { get; set; }

        internal BaseAction(string type, string UserId, Options options)
        {
            options = options ?? new Options ();

            this.Type = type;
            this.MessageId = options.MessageId;
            if (options.Timestamp.HasValue)
                this.Timestamp = options.Timestamp.Value.ToString("o");
            else
                this.Timestamp = DateTime.Now.ToString("o");
            this.Context = options.Context;
            this.Integrations = options.Integrations;
            this.AnonymousId = options.AnonymousId;
            this.UserId = UserId;
        }
    }
}
