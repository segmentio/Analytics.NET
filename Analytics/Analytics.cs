using System;
using System.Collections.Generic;
using System.Text;

namespace Segmentio
{
    public class Analytics
    {
        /// <summary>
        /// Lock for thread-safety
        /// </summary>
        static readonly object padlock = new object();

        public static Client Client { get; private set; }

        /// <summary>
        /// Initialized the default Segment.io client with your API secret.
        /// </summary>
        /// <param name="secret"></param>
        public static void Initialize(string secret)
        {
            // avoiding double locking as recommended:
            // http://www.yoda.arachsys.com/csharp/singleton.html
            lock (padlock)
            {
                if (Client == null)
                {
                    Client = new Client(secret);
                }
            }
        }

        /// <summary>
        /// Initialized the default Segment.io client with your API secret.
        /// </summary>
        /// <param name="secret"></param>
        public static void Initialize(string secret, Options options)
        {
            lock (padlock)
            {
                if (Client == null)
                {
                    Client = new Client(secret, options);
                }
            }
        }

    }
}
