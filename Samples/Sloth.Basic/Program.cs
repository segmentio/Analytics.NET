using System;
using RudderStack;
using Sloth.Common;

namespace Sloth.Basic
{
    class Program
    {
        private const int UserJourneys = 1;

        static void Main(string[] args)
        {
            var writeKey = "testWriteKey";
            var dataPlaneUrl = "https://8244956a91ea.ngrok.io";

            if (string.IsNullOrWhiteSpace(writeKey)) throw new ArgumentException(nameof(writeKey));

            OnExecute(writeKey, dataPlaneUrl);
        }

        private static void OnExecute(string writeKey, string dataPlaneUrl)
        {
            //RudderAnalytics.Initialize(writeKey);

            RudderAnalytics.Initialize(writeKey, new RudderConfig(dataPlaneUrl: dataPlaneUrl));

            Logger.Handlers += Utils.LoggerOnHandlers;

            for (var i = 0; i < UserJourneys; i++)
            {
                Utils.DoJourney();
            }

            // sending all pendant messages
            RudderAnalytics.Client.Flush();

            Utils.PrintSummary();

            RudderAnalytics.Client.Dispose();
        }

    }
}
