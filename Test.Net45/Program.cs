using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RudderStack;
using RudderStack.Test;

namespace RudderStack.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Handlers += Logger_Handlers;

            //Analytics.Initialize(RudderStack.Test.Constants.WRITE_KEY);

            //FlushTests tests = new FlushTests();
            //tests.PerformanceTestNet45();
            RudderAnalytics.Initialize("1sCR76JzHpQohjl33pi8qA5jQD2", new RudderConfig(dataPlaneUrl: "https://75652af01e6d.ngrok.io"));
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
