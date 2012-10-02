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
            Segmentio.Secure = false;
            Segmentio.Host = "192.168.1.139:81";

            Segmentio.Initialize(API_KEY);

            Segmentio.Client.Succeeded += Client_Succeeded;
            Segmentio.Client.Failed += Client_Failed;

            string sessionId = "sdkfjh2khsdjhf32";
            string userId = "ilya@segment.io";

            int trials = 200;
            int submitted = 0;

            for (int i = 0; i < trials; i += 1)
            {
                Segmentio.Client.Identify(sessionId, userId, new Traits() {
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

                Segmentio.Client.Track(sessionId, userId, "Ran .NET test", new Properties() {
                    { "Success", true },
                    { "When", DateTime.Now }
                }, DateTime.Now);

                submitted += 2;
            }

            Assert.IsTrue(Segmentio.Client.Statistics.Submitted == submitted);

            Assert.IsTrue(Segmentio.Client.Statistics.Succeeded > 0);
            Assert.IsTrue(Segmentio.Client.Statistics.Failed == 0);
        }

        void Client_Failed(BaseAction action, System.Exception e)
        {
            Console.WriteLine(String.Format("Action {0} failed : {1}", action.GetAction(), e.Message));
        }

        void Client_Succeeded(BaseAction action)
        {
            Console.WriteLine(String.Format("Action {0} succeeded.", action.GetAction()));
        }

    }
}
