namespace Segment
{
    public class Analytics
    {
        // REMINDER: don't forget to set Properties.AssemblyInfo.AssemblyVersion as well
        public static string VERSION = "3.4.0-alpha";

        /// <summary>
        /// Lock for thread-safety
        /// </summary>
        static readonly object padlock = new object();

        public static Client Client { get; private set; }

        /// <summary>
        /// Initialized the default Segment.io client with your API writeKey.
        /// </summary>
        /// <param name="writeKey"></param>
        public static void Initialize(string writeKey)
        {
            // avoiding double locking as recommended:
            // http://www.yoda.arachsys.com/csharp/singleton.html
            lock (padlock)
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
        /// <param name="writeKey"></param>
        public static void Initialize(string writeKey, Config config)
        {
            lock (padlock)
            {
                if (Client == null)
                {
                    Client = new Client(writeKey, config);
                }
            }
        }

        internal static void Initialize(Client client)
        {
            lock (padlock)
            {
                if (Client == null)
                {
                    Client = client;
                }
            }
        }

        /// <summary>
        /// Disposes of the current client and allows the creation of a new one
        /// </summary>
        public static void Dispose()
        {
            lock (padlock)
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
