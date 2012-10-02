using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Segmentio.Model
{
    public abstract class BaseAction
    {

        public BaseAction(string sessionId, string userId, DateTime? timestamp)
        {
            this.SessionId = sessionId;
            this.UserId = userId;
            if (timestamp.HasValue) this.Timestamp = timestamp.Value.ToString("o");
        }
        
        [JsonProperty(PropertyName="timestamp")]
        public string Timestamp { get; private set; }

        [JsonProperty(PropertyName = "sessionId")]
        public string SessionId { get; private set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; private set; }

        /// <summary>
        /// Returns the string name representing this action based on the Segment.io REST API.
        /// A track returns "track", etc..
        /// </summary>
        /// <returns></returns>
        public abstract string GetAction();
    }
}
