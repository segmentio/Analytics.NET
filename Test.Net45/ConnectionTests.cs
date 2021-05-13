using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                    return Task.FromResult(true);
                });

            RudderAnalytics.Dispose();
            Logger.Handlers += LoggingHandler;
        }

        [TearDown]
        public void CleanUp()
        {
            RudderAnalytics.Dispose();
            Logger.Handlers -= LoggingHandler;
        }
        [Test()]
        public void RetryErrorTestNet45()
        {
            Stopwatch watch = new Stopwatch();

            // Set invalid host address and make timeout to 1s
            var config = new RudderConfig().SetAsync(false);
            config.SetHost("https://fake.rudder-server.com");
            config.SetTimeout(new TimeSpan(0, 0, 1));
            config.SetMaxRetryTime(new TimeSpan(0, 0, 10));
            RudderAnalytics.Initialize(Constants.WRITE_KEY, config);

            // Calculate working time for Identiy message with invalid host address
            watch.Start();
            Actions.Identify(RudderAnalytics.Client);
            watch.Stop();

            Assert.AreEqual(1, RudderAnalytics.Client.Statistics.Submitted);
            Assert.AreEqual(0, RudderAnalytics.Client.Statistics.Succeeded);
            Assert.AreEqual(1, RudderAnalytics.Client.Statistics.Failed);

            // Handling Identify message will take more than 10s even though the timeout is 1s.
            // That's because it retries submit when it's failed.
            Assert.AreEqual(true, watch.ElapsedMilliseconds > 10000);
        }

        [Test()]
        public void RetryErrorWithDefaultMaxRetryTimeTestNet45()
        {
            Stopwatch watch = new Stopwatch();

            // Set invalid host address and make timeout to 1s
            var config = new RudderConfig().SetAsync(false);
            config.SetHost("https://fake.rudder-server.com");
            config.SetTimeout(new TimeSpan(0, 0, 1));
            RudderAnalytics.Initialize(Constants.WRITE_KEY, config);

            // Calculate working time for Identiy message with invalid host address
            watch.Start();
            Actions.Identify(RudderAnalytics.Client);
            watch.Stop();

            Assert.AreEqual(1, RudderAnalytics.Client.Statistics.Submitted);
            Assert.AreEqual(0, RudderAnalytics.Client.Statistics.Succeeded);
            Assert.AreEqual(1, RudderAnalytics.Client.Statistics.Failed);

            // Handling Identify message will take more than 10s even though the timeout is 1s.
            // That's because it retries submit when it's failed.
            Assert.AreEqual(true, watch.ElapsedMilliseconds > 10000);
        }

        [Test()]
        public void ProxyTestNet45()
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
        public void GZipTestNet45()
        {
            // Set GZip/Deflate on request header
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
