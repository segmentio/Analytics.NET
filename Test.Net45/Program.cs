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
            Logger.Handlers += Logger_Handlers;

            var parentPath = Utilities.getParentPath(3, System.IO.Directory.GetCurrentDirectory());
            var filePath = parentPath + "\\.env";
            DotEnv.Load(filePath);
            var dataPlaneUrl = Environment.GetEnvironmentVariable("DATA_PLANE_URL");
            var writeKey = Environment.GetEnvironmentVariable("WRITE_KEY");

            RudderAnalytics.Initialize(writeKey, new RudderConfig(dataPlaneUrl: dataPlaneUrl));
            RudderAnalytics.Client.Identify(
                "userId",
                new Dictionary<string, object> { { "subscription", "inactive" }, }
            );
            RudderAnalytics.Client.Track(
                "userId",
                "CTA Clicked",
                new Dictionary<string, object>
                { {"plan", "premium"}, }
            );
            RudderAnalytics.Client.Page(
                "userId",
                "Sign Up",
                new Dictionary<string, object> {
                    {"url", "https://wwww.example.com/sign-up"},
                }
            );
            RudderAnalytics.Client.Screen(
                "userId",
                "Dashboard",
                new Dictionary<string, object> {
                    {"name", "Paid Dashboard"},
                }
            );
            RudderAnalytics.Client.Group(
                "userId",
                "accountId",
                new Dictionary<string, object> { { "role", "Owner" }, }
            );
            RudderAnalytics.Client.Alias("anonUserId", "userId");

            RudderAnalytics.Client.Flush();
        }

        private static void Logger_Handlers(Logger.Level level, string message, IDictionary<string, object> args)
        {
            Console.WriteLine(message);
        }
    }
}
