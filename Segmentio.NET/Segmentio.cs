using System;
using System.Collections.Generic;
using System.Text;

namespace Segmentio
{
    public class Segmentio
    {
        internal static string _Protocol = "http://";
        internal static string _Host = "api2.segment.io";
        internal static Dictionary<string, string> _Endpoints = new Dictionary<string, string> {
            { "track", "/v1/track" },
            { "identify", "/v1/identify" },
            { "batch", "/v1/import" }
        };


        public static bool Secure
        {
            get { return _Protocol.Equals("https://"); }
            set
            {
                if (value) _Protocol = "https://";
                else _Protocol = "http://";
            }
        }

        public static string Host
        {
            get { return _Host; }
            set {  _Host = value;}
        }


        public static Client Client { get; private set; }

        /// <summary>
        /// Initialized the default Segment.io client with your API Key
        /// Use a different API Key for Development and Production environments
        /// as instructed on our site
        /// </summary>
        /// <param name="apiKey"></param>
        public static void Initialize(string apiKey)
        {
            if (Client == null)
            {
                Client = new Client();
                Client.Initialize(apiKey);
            }
        }

    }
}
