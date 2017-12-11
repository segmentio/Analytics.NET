using System;

namespace Segment
{
    public class Defaults
    {
        public static string Host = "https://api.segment.io";

		public static TimeSpan Timeout = TimeSpan.FromSeconds(5);

        public static int MaxQueueCapacity = 10000;

        public static int BatchIncrement = Constants.BatchIncrement;

        public static bool Async = true;
    }
}
