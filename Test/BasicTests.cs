using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Segmentio;
using Segmentio.Model;

using System.Threading;

namespace Segmentio.Test
{
    [TestClass]
    public class BasicTests
    {
        private const string SECRET = "testsecret";

        private CountdownLatch latch;

        [TestMethod]
        public void BasicFlushTest()
        {
            Analytics.Initialize(SECRET, new Options().SetFlushAt(25));

            Analytics.Client.Succeeded += Client_Succeeded;
            Analytics.Client.Failed += Client_Failed;

            string userId = "ilya@segment.io";

            int trials = 50;
            int submitted = 0;

            latch = new CountdownLatch(trials * 2);

            for (int i = 0; i < trials; i += 1)
            {
                Analytics.Client.Identify(userId, new Traits() {
                    { "Subscription Plan", "Free" },
                    { "Friends", 30 },
                    { "Joined", DateTime.Now },
                    { "Cool", true },
                    { "Revenue", 40.32 },
                    { "Don't Submit This, Kids", new UnauthorizedAccessException() } },
                        new DateTime(),
                        new Context()
                            .SetIp("12.212.12.49")
                            .SetLanguage("en-us")
                    );

                Analytics.Client.Track(userId, "Ran .NET test", new Properties() {
                    { "Success", true },
                    { "When", DateTime.Now }
                }, DateTime.Now);

                submitted += 2;
            }

            Analytics.Client.Flush();

            latch.Wait();

            Assert.IsTrue(Analytics.Client.Statistics.Submitted == submitted);

            Assert.IsTrue(Analytics.Client.Statistics.Succeeded == submitted);
            Assert.IsTrue(Analytics.Client.Statistics.Failed == 0);
        }

        void Client_Failed(BaseAction action, System.Exception e)
        {
            Console.WriteLine(String.Format("Action {0} failed : {1}", action.GetAction(), e.Message));
            latch.Signal();
        }

        void Client_Succeeded(BaseAction action)
        {
            Console.WriteLine(String.Format("Action {0} succeeded.", action.GetAction()));
            latch.Signal();
        }

    }
}
