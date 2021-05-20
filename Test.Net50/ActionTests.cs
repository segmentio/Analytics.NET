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
    public class ActionTests
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
            var client = new Client(Constants.WRITE_KEY, new Config().SetAsync(false), _mockRequestHandler.Object);
            Analytics.Initialize(client);
        }

        [TearDown]
        public void CleanUp()
        {
            Logger.Handlers -= LoggingHandler;
        }

        [Test()]
        public void IdentifyTestNetStanard20()
        {
            Actions.Identify(Analytics.Client);
            FlushAndCheck(1);
        }

        [Test()]
        public void TrackTestNetStanard20()
        {
            Actions.Track(Analytics.Client);
            FlushAndCheck(1);
        }

        [Test()]
        public void AliasTestNetStanard20()
        {
            Actions.Alias(Analytics.Client);
            FlushAndCheck(1);
        }

        [Test()]
        public void GroupTestNetStanard20()
        {
            Actions.Group(Analytics.Client);
            FlushAndCheck(1);
        }

        [Test()]
        public void PageTestNetStanard20()
        {
            Actions.Page(Analytics.Client);
            FlushAndCheck(1);
        }

        [Test()]
        public void ScreenTestNetStanard20()
        {
            Actions.Screen(Analytics.Client);
            FlushAndCheck(1);
        }

        private void FlushAndCheck(int messages)
        {
            Analytics.Client.Flush();
            Assert.AreEqual(messages, Analytics.Client.Statistics.Submitted);
            Assert.AreEqual(messages, Analytics.Client.Statistics.Succeeded);
            Assert.AreEqual(0, Analytics.Client.Statistics.Failed);
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
            Console.WriteLine(String.Format("[ActionTests] [{0}] {1}", level, message));
        }
    }
}
