//-----------------------------------------------------------------------
// <copyright file="Analytics.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment
{
    public class Analytics
    {
        // REMINDER: don't forget to set Properties.AssemblyInfo.AssemblyVersion as well
        public static readonly string VERSION = "2.0.2";

        /// <summary>
        /// Lock for thread-safety.
        /// </summary>
        private static readonly object Padlock = new object();

        public static Client Client { get; private set; }

        /// <summary>
        /// Initialized the default Segment.io client with your API writeKey.
        /// </summary>
        /// <param name="writeKey">The write key.</param>
        public static void Initialize(string writeKey)
        {
            // avoiding double locking as recommended:
            // http://www.yoda.arachsys.com/csharp/singleton.html
            lock (Padlock)
            {
                if (Client == null)
                {
                    Client = new Client(writeKey);
                }
            }
        }

        /// <summary>
        /// Initialized the default Segment.io client with your API writeKey.
        /// </summary>
        /// <param name="writeKey">The write key.</param>
        /// <param name="config">The client config.</param>
        public static void Initialize(string writeKey, Config config)
        {
            lock (Padlock)
            {
                if (Client == null)
                {
                    Client = new Client(writeKey, config);
                }
            }
        }

        /// <summary>
        /// Disposes of the current client and allows the creation of a new one.
        /// </summary>
        public static void Dispose()
        {
            lock (Padlock)
            {
                if (Client != null)
                {
                    Client.Dispose();
                    Client = null;
                }
            }
        }
    }
}
