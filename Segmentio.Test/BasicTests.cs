using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Segmentio;
using Segmentio.Model;

namespace Segmentio.Test
{
    [TestClass]
    public class BasicTests
    {
        private const string API_KEY = "fakeid";

        [TestMethod]
        public void TestMethod1()
        {
            Segmentio.Initialize(API_KEY);

            string sessionId = "sdkfjh2khsdjhf32";
            string userId = "ilya@segment.io";

            int trials = 200;
            int submitted = 0;

            for (int i = 0; i < trials; i += 1)
            {
                Segmentio.Client.Identify(sessionId, userId, new Dictionary<string, object>() {
                { "Subscription Plan", "Free" },
                { "Friends", 30 },
                { "Joined", DateTime.Now },
                { "Cool", true },
                { "Revenue", 40.32 },
                { "Don't Submit This, Kids", new UnauthorizedAccessException() } },
                    new Context()
                        .SetIp("12.212.12.49")
                        .SetLocation(new Location("US", "CA"))
                        .SetLanguage("en-us"),
                    DateTime.Now
                );

                Segmentio.Client.Track(sessionId, userId, "Ran .NET test", new Dictionary<string, object>() {
                    { "Success", true },
                    { "When", DateTime.Now }
                }, DateTime.Now);

                submitted += 2;
            }

            Assert.IsTrue(Segmentio.Client.Statistics.Submitted == submitted);

            Assert.IsTrue(Segmentio.Client.Statistics.Succeeded > 0);
            Assert.IsTrue(Segmentio.Client.Statistics.Failed == 0);
        }

    }
}
