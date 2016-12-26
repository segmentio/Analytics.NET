using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Segment.Stats
{
    public class Statistics
    {
        public int Submitted { get; set; }
        public int Succeeded { get; set; }
        public int Failed { get; set; }

        internal static int Increment(int value)
        {
            // This is to make counters overflow to zero instead
            // of going negative.
            // Cannot use "uint" type because it's not CLS compliant
            if (value == int.MaxValue)
            {
                return 0;
            }
            return value + 1;
        }
    }
}
