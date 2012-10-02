using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Segmentio.Model
{
    [DataContract]
    public class Track : BaseAction
    {
        [DataMember(Name="action")]
        private string Action = "track";

        [DataMember(Name="event")]
        private string EventName { get; set; }

        [DataMember(Name="properties")]
        private Properties Properties { get; set; }

        internal Track(string sessionId, string userId, string eventName, 
            Properties properties, DateTime? timestamp) 
            : base(sessionId, userId, timestamp)
        {
            this.EventName = eventName;
            this.Properties = properties ?? new Properties();
        }

        public override string GetAction()
        {
            return Action;
        }
    }
}
