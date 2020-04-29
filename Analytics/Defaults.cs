using System;
using Segment.Model;

namespace Segment
{
    public class Defaults
    {
        public static readonly string Host = "https://api.segment.io";

        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

        public static readonly int MaxQueueCapacity = 10000;

        public static readonly bool Async = true;

        public static int FlushAt = 20;

        public static bool SyncMode = false;

        public static readonly string UserAgentHeader = GetDefaultUserContext();

        public static double FlushInterval = 10;

        public static readonly int Threads = 1;

        public static bool Gzip = false;

        private static string GetDefaultUserContext()
        {
            var lib = new Context()["library"] as Dict;
            return $"{lib["name"]}/{lib["version"]}";
        }
    }
}
