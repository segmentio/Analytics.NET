using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Segmentio.Model
{
    [DataContract]
    public class Identify : BaseAction
    {
        [DataMember]
        private string action = "identify";

        [DataMember]
        private Context context { get; set; }

        [DataMember]
        private Dictionary<string, object> traits { get; set; }

        internal Identify(string sessionId, string userId, 
            Dictionary<string, object> traits, 
            Context context, DateTime? timestamp) 
            : base(sessionId, userId, timestamp)
        {   
            this.traits = traits ?? new Dictionary<string, object>();
            this.context = context;
        }

        public override string GetAction()
        {
            return action;
        }
    }
}
