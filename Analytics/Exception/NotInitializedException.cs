using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analytics.Exception
{
    public class NotInitializedException : System.Exception
    {
        public NotInitializedException() : base("Please initialize Segment.io first before using.") { }

    }
}
