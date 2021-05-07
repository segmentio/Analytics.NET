using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using RudderStack.Request;
using Moq;
using RudderStack.Model;

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
                    return Task.FromResult(true);
                });

            RudderAnalytics.Dispose();
            Logger.Handlers += LoggingHandler;
            var client = new RudderClient(Constants.WRITE_KEY, new RudderConfig().SetAsync(false), _mockRequestHandler.Object);
            RudderAnalytics.Initialize(client);
        }

        [TearDown]
        public void CleanUp()
        {
            RudderAnalytics.Dispose();
            Logger.Handlers -= LoggingHandler;
        }

        [Test ()]
        public void IdentifyTestNet45()
        {
            Actions.Identify(RudderAnalytics.Client);
            FlushAndCheck(1);
        }

        [Test()]
        public void IdentifyWithCustomOptionsTestNet45()
        {
            var traits = new Model.Traits() {
                { "email", "friends@rudder.com" }
            };
            var options = new Model.RudderOptions()
                .SetIntegration("Vero", new Model.Dict() {
                    {
                        "tags", new Model.Dict() {
                            { "id", "235FAG" },
                            { "action", "add" },
                            { "values", new string[] {"warriors", "giants", "niners"} }
                        }
                    }
                });

            Actions.Identify(RudderAnalytics.Client, traits, options);
            FlushAndCheck(1);
        }

        [Test()]
        public void TrackTestNet45()
        {
            Actions.Track(RudderAnalytics.Client);
            FlushAndCheck(1);
        }

        [Test()]
        public void AliasTestNet45()
        {
            Actions.Alias(RudderAnalytics.Client);
            FlushAndCheck(1);
        }

        [Test()]
        public void GroupTestNet45()
        {
            Actions.Group(RudderAnalytics.Client);
            FlushAndCheck(1);
        }

        [Test()]
        public void PageTestNet45()
        {
            Actions.Page(RudderAnalytics.Client);
            FlushAndCheck(1);
        }

        [Test()]
        public void ScreenTestNet45()
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

