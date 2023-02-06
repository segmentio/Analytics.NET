using System;
using RudderStack;
using RudderStack.Utils;
using Sloth.Common;

namespace Sloth.Basic
{
    class Program
    {
        private const int UserJourneys = 1;

        static void Main(string[] args)
        {
            var parentPath = Utilities.getParentPath(5, System.IO.Directory.GetCurrentDirectory());
            var filePath = parentPath + "\\.env";
            DotEnv.Load(filePath);
            var dataPlaneUrl = Environment.GetEnvironmentVariable("DATA_PLANE_URL");
            var writeKey = Environment.GetEnvironmentVariable("WRITE_KEY");

            if (string.IsNullOrWhiteSpace(writeKey)) throw new ArgumentException(nameof(writeKey));

            OnExecute(writeKey, dataPlaneUrl);
        }

        private static void OnExecute(string writeKey, string dataPlaneUrl)
        {
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
