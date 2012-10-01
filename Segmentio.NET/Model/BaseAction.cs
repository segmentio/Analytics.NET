using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Segmentio.Model
{
    [KnownType(typeof(Identify))]
    [KnownType(typeof(Track))]
    [DataContract]
    public abstract class BaseAction
    {

        public BaseAction(string sessionId, string userId, DateTime? timestamp)
        {
            this.sessionId = sessionId;
            this.userId = userId;
            if (timestamp.HasValue) this.timestamp = timestamp.Value.ToString("o");
        }

        [DataMember]
        public string timestamp { get; private set; }

        [DataMember]
        public string sessionId { get; private set; }

        [DataMember]
        public string userId { get; private set; }

        /// <summary>
        /// Returns the string name representing this action based on the Segment.io REST API.
        /// A track returns "track", etc..
        /// </summary>
        /// <returns></returns>
        public abstract string GetAction();
    }
}
