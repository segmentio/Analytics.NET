using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RudderStack.Model;
using RudderStack.Request;

namespace RudderStack.Test
{
    [TestFixture()]
    public class ConnectionTests
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
        }

        [TearDown]
        public void CleanUp()
        {
            Logger.Handlers -= LoggingHandler;
        }

        [Test()]
        public void ProxyTestNetStanard20()
        {
            // Set proxy address, like as "http://localhost:8888"
            var client = new RudderClient(Constants.WRITE_KEY, new RudderConfig().SetAsync(false).SetProxy(""), _mockRequestHandler.Object);
            RudderAnalytics.Initialize(client);

            Actions.Identify(RudderAnalytics.Client);

            Assert.AreEqual(1, RudderAnalytics.Client.Statistics.Submitted);
            Assert.AreEqual(1, RudderAnalytics.Client.Statistics.Succeeded);
            Assert.AreEqual(0, RudderAnalytics.Client.Statistics.Failed);
        }

        [Test()]
        public void GZipTestNetStanard20()
        {
            // Set proxy address, like as "http://localhost:8888"
            var client = new RudderClient(Constants.WRITE_KEY, new RudderConfig().SetAsync(false), _mockRequestHandler.Object);
            RudderAnalytics.Initialize(client);

            Actions.Identify(RudderAnalytics.Client);

            Assert.AreEqual(1, RudderAnalytics.Client.Statistics.Submitted);
            Assert.AreEqual(1, RudderAnalytics.Client.Statistics.Succeeded);
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
            Console.WriteLine(String.Format("[ConnectionTests] [{0}] {1}", level, message));
        }
    }
}
