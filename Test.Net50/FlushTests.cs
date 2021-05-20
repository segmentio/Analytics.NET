using Moq;
using NUnit.Framework;
using Segment;
using Segment.Model;
using Segment.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Net50
{
    [TestFixture]
    public class FlushTests
    {
        private Mock<IRequestHandler> _mockRequestHandler;

        [SetUp]
        public void Init()
        {
            _mockRequestHandler = new Mock<IRequestHandler>();
            _mockRequestHandler
                .Setup(x => x.MakeRequest(It.IsAny<Batch>()))
                .Returns((Batch b) =>
                {
                    b.batch.ForEach(_ => Analytics.Client.Statistics.IncrementSucceeded());
                    return Task.CompletedTask;
                });

            Analytics.Dispose();
            Logger.Handlers += LoggingHandler;
        }

        [TearDown]
        public void CleanUp()
        {
            Logger.Handlers -= LoggingHandler;
        }

        [Test()]
        public void SynchronousFlushTestNetStandard20()
        {
            var client = new Client(Constants.WRITE_KEY, new Config().SetAsync(false), _mockRequestHandler.Object);
            Analytics.Initialize(client);
            Analytics.Client.Succeeded += Client_Succeeded;
            Analytics.Client.Failed += Client_Failed;

            int trials = 10;

            RunTests(Analytics.Client, trials);

            Assert.AreEqual(trials, Analytics.Client.Statistics.Submitted);
            Assert.AreEqual(trials, Analytics.Client.Statistics.Succeeded);
            Assert.AreEqual(0, Analytics.Client.Statistics.Failed);
        }

        [Test()]
        public void AsynchronousFlushTestNetStandard20()
        {
            var client = new Client(Constants.WRITE_KEY, new Config().SetAsync(true), _mockRequestHandler.Object);
            Analytics.Initialize(client);

            Analytics.Client.Succeeded += Client_Succeeded;
            Analytics.Client.Failed += Client_Failed;

            int trials = 10;

            RunTests(Analytics.Client, trials);

            Analytics.Client.Flush();

            Assert.AreEqual(trials, Analytics.Client.Statistics.Submitted);
            Assert.AreEqual(trials, Analytics.Client.Statistics.Succeeded);
            Assert.AreEqual(0, Analytics.Client.Statistics.Failed);
        }

        [Test()]
        public async Task PerformanceTestNetStandard20()
        {
            var client = new Client(Constants.WRITE_KEY, new Config(), _mockRequestHandler.Object);
            Analytics.Initialize(client);

            Analytics.Client.Succeeded += Client_Succeeded;
            Analytics.Client.Failed += Client_Failed;

            int trials = 100;

            DateTime start = DateTime.Now;

            RunTests(Analytics.Client, trials);

            await Analytics.Client.FlushAsync();

            TimeSpan duration = DateTime.Now.Subtract(start);

            Assert.AreEqual(trials, Analytics.Client.Statistics.Submitted);
            Assert.AreEqual(trials, Analytics.Client.Statistics.Succeeded);
            Assert.AreEqual(0, Analytics.Client.Statistics.Failed);

            Assert.IsTrue(duration.CompareTo(TimeSpan.FromSeconds(20)) < 0);
        }

        private void RunTests(Client client, int trials)
        {
            for (int i = 0; i < trials; i += 1)
            {
                Actions.Random(client);
            }
        }

        void Client_Failed(BaseAction action, System.Exception e)
        {
            Console.WriteLine(String.Format("Action [{0}] {1} failed : {2}",
                action.MessageId, action.Type, e.Message));
        }

        void Client_Succeeded(BaseAction action)
        {
            Console.WriteLine(String.Format("Action [{0}] {1} succeeded.",
                action.MessageId, action.Type));
        }

        static void LoggingHandler(Logger.Level level, string message, IDictionary<string, object> args)
        {
            if (args != null)
            {
                foreach (string key in args.Keys)
                {
                    message += String.Format(" {0}: {1},", "" + key, "" + args[key]);
                }
            }
            Console.WriteLine(String.Format("[FlushTests] [{0}] {1}", level, message));
        }
    }
}
