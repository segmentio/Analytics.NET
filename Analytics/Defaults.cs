using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Segment
{
    public class Defaults
    {
        public static string Host = "https://api.segment.io";

        public static TimeSpan Timeout = TimeSpan.FromSeconds(5);

        public static int MaxQueueCapacity = 10000;

        public static int FlushAt = 20;

        public static bool SyncMode = false;

        public static double FlushInterval = 10;

        public static int Threads = 1;

        public static bool Gzip = false;
    }
}
