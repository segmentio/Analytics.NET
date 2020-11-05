using System;
using RudderStack;
using Sloth.Common;

namespace Sloth.Enterprise
{
    class Program
    {

        private const int UserJourneys = 5000;

        static void Main(string[] args)
        {
            var writeKey = Environment.GetEnvironmentVariable("writeKey");

            if (string.IsNullOrWhiteSpace(writeKey)) throw new ArgumentException(nameof(writeKey));

            OnExecute(writeKey);
        }

        private static void OnExecute(string writeKey)
        {
            var config = new Config()
                .SetMaxQueueSize(100000)
                .SetHost("https://hosted.rudderlabs.com")
                .SetMaxBatchSize(40)
                .SetRequestCompression(true);

            Analytics.Initialize(writeKey, config);

            Logger.Handlers += Utils.LoggerOnHandlers;

            for (var i = 0; i < UserJourneys; i++)
            {
                Utils.DoJourney();
            }

            // sending all pendant messages
            Analytics.Client.Flush();

            Utils.PrintSummary();

            Analytics.Client.Dispose();
        }
    }
}
