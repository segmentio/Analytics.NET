namespace RudderStack
{
    public class RudderAnalytics
    {
        // REMINDER: don't forget to set Properties.AssemblyInfo.AssemblyVersion as well
        public static string VERSION = "2.0.0";

        /// <summary>
        /// Lock for thread-safety
        /// </summary>
        static readonly object padlock = new object();

        public static RudderClient Client { get; private set; }

        /// <summary>
        /// Initialized the default RudderStack client with your API writeKey.
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
                    Client = new RudderClient(writeKey);
                }
            }
        }

        /// <summary>
        /// Initialized the default RudderStack client with your API writeKey.
        /// </summary>
        /// <param name="writeKey"></param>
        public static void Initialize(string writeKey, RudderConfig config)
        {
            lock (padlock)
            {
                if (Client == null)
                {
                    Client = new RudderClient(writeKey, config);
                }
            }
        }

        /// <summary>
        /// Initialized the default RudderStack client with your Custom Client.
        /// </summary>
        /// <param name="client"></param>
        public static void Initialize(RudderClient client)
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
