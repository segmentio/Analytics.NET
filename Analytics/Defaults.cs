using System;
using Segment.Model;

namespace Segment
{
    public class Defaults
    {
        public static readonly string Host = "https://api.segment.io";

        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

        public static readonly int MaxQueueCapacity = 10000;

        public static readonly int MaxBatchSize = 20;

        public static readonly bool Async = true;

        public static readonly int FlushIntervalInMillis = 30000;

        public static readonly string UserAgentHeader = GetDefaultUserContext();

        public static int FlushIntervalInMillis = 30000;

        public static int Threads = 1;

        private static string GetDefaultUserContext()
        {
            var lib = new Context()["library"] as Dict;
            return $"{lib["name"]}/{lib["version"]}";
        }
    }
}
