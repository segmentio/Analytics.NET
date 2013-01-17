using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analytics.Stats
{
    public class Statistics
    {
        public int Submitted { get; set; }
        public int Succeeded { get; set; }
        public int Failed { get; set; }
        public int Flushed { get; set; }
    }
}
