using System;
using System.Collections.Generic;

namespace RudderStack.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Handlers += Logger_Handlers;

            RudderAnalytics.Initialize(RudderStack.Test.Constants.WRITE_KEY, new RudderConfig(dataPlaneUrl:RudderStack.Test.Constants.DATAPLANE_WITH_GZIP, gzip:false));
         //   RudderAnalytics.Initialize(RudderStack.Test.Constants.WRITE_KEY, new RudderConfig(dataPlaneUrl: RudderStack.Test.Constants.DATAPLANE_WITHOUT_GZIP, gzip: true));//

            RudderAnalytics.Client.Track("prateek", "Item Purchased (with .Net 3.5)");
            RudderAnalytics.Client.Flush();
        }

        private static void Logger_Handlers(Logger.Level level, string message, IDictionary<string, object> args)
        {
            Console.WriteLine(message);
        }
    }
}
