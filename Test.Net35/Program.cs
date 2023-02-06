using System;
using System.Collections.Generic;
using RudderStack.Utils;
namespace RudderStack.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Handlers += Logger_Handlers;

            var parentPath = Utilities.getParentPath(3, System.IO.Directory.GetCurrentDirectory());
            var filePath = parentPath + "\\.env";
            DotEnv.Load(filePath);
            var dataPlaneUrl = Environment.GetEnvironmentVariable("DATA_PLANE_URL");
            var writeKey = Environment.GetEnvironmentVariable("WRITE_KEY");

            RudderAnalytics.Initialize(writeKey, new RudderConfig(dataPlaneUrl: dataPlaneUrl, gzip: false));
            RudderAnalytics.Client.Track("prateek", "Item Purchased (with .Net 3.5)");
            RudderAnalytics.Client.Flush();
        }

        private static void Logger_Handlers(Logger.Level level, string message, IDictionary<string, object> args)
        {
            Console.WriteLine(message);
        }
    }
}
