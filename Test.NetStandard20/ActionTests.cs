using System;
using NUnit.Framework;
using System.Collections.Generic;
using Moq;
using RudderStack.Request;
using RudderStack.Model;
using System.Threading.Tasks;

namespace RudderStack.Test
{
    [TestFixture()]
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
                    b.batch.ForEach(_ => RudderAnalytics.Client.Statistics.IncrementSucceeded());
                    return Task.CompletedTask;
                });

            RudderAnalytics.Dispose();
            Logger.Handlers += LoggingHandler;
            var client = new RudderClient(Constants.WRITE_KEY, new RudderConfig().SetAsync(false), _mockRequestHandler.Object);
            RudderAnalytics.Initialize(client);
        }

        [TearDown]
        public void CleanUp()
        {
            Logger.Handlers -= LoggingHandler;
        }

        [Test()]
        public void IdentifyTestNetStanard20()
        {
            Actions.Identify(RudderAnalytics.Client);
            FlushAndCheck(1);
        }

        [Test()]
        public void TrackTestNetStanard20()
        {
            Actions.Track(RudderAnalytics.Client);
            FlushAndCheck(1);
        }

        [Test()]
        public void AliasTestNetStanard20()
        {
            Actions.Alias(RudderAnalytics.Client);
            FlushAndCheck(1);
        }

        [Test()]
        public void GroupTestNetStanard20()
        {
            Actions.Group(RudderAnalytics.Client);
            FlushAndCheck(1);
        }

        [Test()]
        public void PageTestNetStanard20()
        {
            Actions.Page(RudderAnalytics.Client);
            FlushAndCheck(1);
        }

        [Test()]
        public void ScreenTestNetStanard20()
        {
            Actions.Screen(RudderAnalytics.Client);
            FlushAndCheck(1);
        }

        private void FlushAndCheck(int messages)
        {
            RudderAnalytics.Client.Flush();
            Assert.AreEqual(messages, RudderAnalytics.Client.Statistics.Submitted);
            Assert.AreEqual(messages, RudderAnalytics.Client.Statistics.Succeeded);
            Assert.AreEqual(0, RudderAnalytics.Client.Statistics.Failed);
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

