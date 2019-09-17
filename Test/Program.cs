using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Segment;
using Segment.Test;

namespace Segment.Test
{
    class Program
    {
        static void Main_Exe(string[] args)
        {
            Logger.Handlers += Logger_Handlers;

            Analytics.Initialize("nAZ4rSBdQMoMJ6bswze53Jorbpjtne78");
            Analytics.Client.Track("prateek", "Item Purchased");
            Analytics.Client.Flush();
            Analytics.Dispose();
        }

        private static void Logger_Handlers(Logger.Level level, string message, IDictionary<string, object> args)
        {
            Console.WriteLine(message);
        }
    }
}
