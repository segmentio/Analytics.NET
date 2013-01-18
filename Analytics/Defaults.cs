using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Segmentio
{
    public class Defaults
    {
        public static string Host = "https://api.segment.io";

        public static int FlushAt = 20;

        /// <summary>
        /// Defaults to 10 seconds
        /// </summary>
        public static TimeSpan FlushAfter = new TimeSpan(0, 0, 0, 10, 0);

        public static int MaxQueueCapacity = 10000;

    }
}
