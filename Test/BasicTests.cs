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

        [TestMethod]
        public void SynchronousFlushTest()
        {
            Analytics.Reset();

            Analytics.Initialize(SECRET, new Options().SetAsync(false));

            Analytics.Client.Succeeded += Client_Succeeded;
            Analytics.Client.Failed += Client_Failed;

            int trials = 10;

            RunTests(Analytics.Client, trials);

            Assert.IsTrue(Analytics.Client.Statistics.Submitted == trials);
            Assert.IsTrue(Analytics.Client.Statistics.Succeeded == trials);
            Assert.IsTrue(Analytics.Client.Statistics.Failed == 0);
        }

        [TestMethod]
        public void AsynchronousFlushTest()
        {
            Analytics.Reset();

            Analytics.Initialize(SECRET, new Options().SetAsync(true));

            Analytics.Client.Succeeded += Client_Succeeded;
            Analytics.Client.Failed += Client_Failed;

            int trials = 10;

            RunTests(Analytics.Client, trials);

            Analytics.Client.Flush();

            Assert.IsTrue(Analytics.Client.Statistics.Submitted == trials);
            Assert.IsTrue(Analytics.Client.Statistics.Succeeded == trials);
            Assert.IsTrue(Analytics.Client.Statistics.Failed == 0);
        }


        [TestMethod]
        public void PerformanceTest()
        {
            Analytics.Reset();

            Analytics.Initialize(SECRET);

            Analytics.Client.Succeeded += Client_Succeeded;
            Analytics.Client.Failed += Client_Failed;

            int trials = 100;

            DateTime start = DateTime.Now;

            RunTests(Analytics.Client, trials);

            Analytics.Client.Flush();

            TimeSpan duration = DateTime.Now.Subtract(start);

            Assert.IsTrue(Analytics.Client.Statistics.Submitted == trials);
            Assert.IsTrue(Analytics.Client.Statistics.Succeeded == trials);
            Assert.IsTrue(Analytics.Client.Statistics.Failed == 0);

            Assert.IsTrue(duration.CompareTo(TimeSpan.FromSeconds(5)) < 0);

        }

        private void RunTests(Client client, int trials)
        {
            for (int i = 0; i < trials; i += 1)
            {
                ActionRunner.Random(client);
            }
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
