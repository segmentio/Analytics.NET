//-----------------------------------------------------------------------
// <copyright file="BaseAction.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment.Model
{
    using System;
    using Newtonsoft.Json;

    public abstract class BaseAction
    {
        internal BaseAction(string type, string userId, Options options)
        {
            options = options ?? new Options();

            this.Type = type;
            this.MessageId = Guid.NewGuid().ToString();

            if (options.Timestamp.HasValue)
            {
                this.Timestamp = options.Timestamp.Value.ToString("o");
            }
            else
            {
                this.Timestamp = DateTime.Now.ToString("o");
            }

            this.Context = options.Context;
            this.Integrations = options.Integrations;
            this.AnonymousId = options.AnonymousId;
            this.UserId = userId;
        }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "messageId")]
        public string MessageId { get; private set; }
    
        [JsonProperty(PropertyName = "timestamp")]
        public string Timestamp { get; private set; }

        [JsonProperty(PropertyName = "context")]
        public Context Context { get; set; }

        [JsonProperty(PropertyName = "integrations")]
        public Dict Integrations { get; set; }

        [JsonProperty(PropertyName = "anonymousId")]
        public string AnonymousId { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; private set; }
    }
}
