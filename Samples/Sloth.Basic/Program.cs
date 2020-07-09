using System;
using Segment;
using Sloth.Common;
using Environment = System.Environment;

namespace Sloth.Basic
{
    class Program
    {
        private const int UserJourneys = 500;

        static void Main(string[] args)
        {
            var writeKey = Environment.GetEnvironmentVariable("writeKey");

            if (string.IsNullOrWhiteSpace(writeKey)) throw new ArgumentException(nameof(writeKey));

            OnExecute(writeKey);
        }

        private static void OnExecute(string writeKey)
        {
            Analytics.Initialize(writeKey);

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
