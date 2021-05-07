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
            RudderAnalytics.Client.Track("prateek", "Item Purchased");
            RudderAnalytics.Client.Flush();
        }

        private static void Logger_Handlers(Logger.Level level, string message, IDictionary<string, object> args)
        {
            Console.WriteLine(message);
        }
    }
}
