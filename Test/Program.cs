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
            Console.WriteLine("The App is running");
            Logger.Handlers += Logger_Handlers;

            RudderAnalytics.Initialize("1n0JdVPZTRUIkLXYccrWzZwdGSx", new RudderConfig(dataPlaneUrl: "https://02f1-175-101-36-100.in.ngrok.io", gzip: true));
            //RudderAnalytics.Initialize("1n0JdVPZTRUIkLXYccrWzZwdGSx", new RudderConfig(dataPlaneUrl: "https://rudderstachvf.dataplane.rudderstack.com", gzip: true));
            RudderAnalytics.Client.Track("prateek", "Item Purchased");
            RudderAnalytics.Client.Flush();

        }

        private static void Logger_Handlers(Logger.Level level, string message, IDictionary<string, object> args)
        {
            Console.WriteLine(message);
        }
    }
}
