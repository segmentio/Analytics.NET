using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RudderStack;
using RudderStack.Utils;
using RudderStack.Test;

namespace RudderStack.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("The App is running");
            Logger.Handlers += Logger_Handlers;

            var parentPath = Utilities.getParentPath(4, System.IO.Directory.GetCurrentDirectory());
            var filePath = parentPath + "\\.env";
            DotEnv.Load(filePath);
            var dataPlaneUrl = Environment.GetEnvironmentVariable("DATA_PLANE_URL");
            var writeKey = Environment.GetEnvironmentVariable("WRITE_KEY");

            RudderAnalytics.Initialize(writeKey, new RudderConfig(dataPlaneUrl: dataPlaneUrl, gzip: true));
            RudderAnalytics.Client.Track("prateek", "Item Purchased");
            RudderAnalytics.Client.Flush();

        }

        private static void Logger_Handlers(Logger.Level level, string message, IDictionary<string, object> args)
        {
            Console.WriteLine(message);
        }
    }
}
