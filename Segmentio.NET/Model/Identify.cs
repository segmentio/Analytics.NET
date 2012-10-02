using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Segmentio.Model
{
    [DataContract]
    public class Identify : BaseAction
    {
        [DataMember(Name="action")]
        private string Action = "identify";

        [DataMember(Name="context")]
        private Context Context { get; set; }

        [DataMember(Name="traits")]
        private Traits Traits { get; set; }

        internal Identify(string sessionId, string userId, 
            Traits traits, Context context, DateTime? timestamp) 
            : base(sessionId, userId, timestamp)
        {   
            this.Traits = Traits ?? new Traits();
            this.Context = context;
        }

        public override string GetAction()
        {
            return Action;
        }
    }
}
