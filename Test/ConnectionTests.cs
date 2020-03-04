using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Segment.Model;
using Segment.Request;

namespace Segment.Test
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
                    b.batch.ForEach(_ => Analytics.Client.Statistics.IncrementSucceeded());
                    return Task.CompletedTask;
                });

            Analytics.Dispose();
            Logger.Handlers += LoggingHandler;
        }

        [Test()]
        public void ProxyTest()
        {
            // Set proxy address, like as "http://localhost:8888"
            var client = new Client(Constants.WRITE_KEY, new Config().SetAsync(false).SetProxy(""), _mockRequestHandler.Object);
            Analytics.Initialize(client);

            Actions.Identify(Analytics.Client);

            Assert.AreEqual(1, Analytics.Client.Statistics.Submitted);
            Assert.AreEqual(1, Analytics.Client.Statistics.Succeeded);
            Assert.AreEqual(0, Analytics.Client.Statistics.Failed);
        }

        [Test()]
        public void GZipTest()
        {
            // Set GZip/Deflate on request header
            var client = new Client(Constants.WRITE_KEY, new Config().SetAsync(false).SetRequestCompression(true), _mockRequestHandler.Object);
            Analytics.Initialize(client);

            Actions.Identify(Analytics.Client);

            Assert.AreEqual(1, Analytics.Client.Statistics.Submitted);
            Assert.AreEqual(1, Analytics.Client.Statistics.Succeeded);
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
            Console.WriteLine(String.Format("[ConnectionTests] [{0}] {1}", level, message));
        }
    }
}
