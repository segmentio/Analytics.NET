using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RudderStack.Exception
{
    public class NotInitializedException : System.Exception
    {
        public NotInitializedException() : base("Please initialize RudderStack first before using.") { }

    }
}
