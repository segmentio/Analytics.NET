using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Segmentio.Model
{
    [DataContract]
    public class Track : BaseAction
    {
        [DataMember]
        private string action = "track";

        [DataMember(Name="event")]
        private string eventName { get; set; }

        [DataMember]
        private Dictionary<string, object> properties { get; set; }

        internal Track(string sessionId, string userId, string eventName, 
            Dictionary<string, object> properties, DateTime? timestamp) 
            : base(sessionId, userId, timestamp)
        {
            this.eventName = eventName;
            this.properties = properties ?? new Dictionary<string, object>();
        }

        public override string GetAction()
        {
            return action;
        }
    }
}
