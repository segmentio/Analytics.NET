using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Actions;
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
        public void RetryConnectionErrorTestNetStandard20()
        {
            Stopwatch watch = new Stopwatch();

            // Set invalid host address and make timeout to 1s
            var config = new Config().SetAsync(false);
            config.SetHost("https://fake.segment-server.com");
            config.SetTimeout(new TimeSpan(0, 0, 1));
            config.SetMaxRetryTime(new TimeSpan(0, 0, 10));
            Analytics.Initialize(Constants.WRITE_KEY, config);

            // Calculate working time for Identiy message with invalid host address
            watch.Start();
            Actions.Identify(Analytics.Client);
            watch.Stop();

            Assert.AreEqual(1, Analytics.Client.Statistics.Submitted);
            Assert.AreEqual(0, Analytics.Client.Statistics.Succeeded);
            Assert.AreEqual(1, Analytics.Client.Statistics.Failed);

            // Handling Identify message will take more than 10s even though the timeout is 1s.
            // That's because it retries submit when it's failed.
            Assert.AreEqual(true, watch.ElapsedMilliseconds > 10000);
        }

        [Test()]
        public void RetryConnectionErrorWithDefaultMaxRetryTimeTestNetStandard20()
        {
            Stopwatch watch = new Stopwatch();

            // Set invalid host address and make timeout to 1s
            var config = new Config().SetAsync(false);
            config.SetHost("https://fake.segment-server.com");
            config.SetTimeout(new TimeSpan(0, 0, 1));
            Analytics.Initialize(Constants.WRITE_KEY, config);

            // Calculate working time for Identiy message with invalid host address
            watch.Start();
            Actions.Identify(Analytics.Client);
            watch.Stop();

            Assert.AreEqual(1, Analytics.Client.Statistics.Submitted);
            Assert.AreEqual(0, Analytics.Client.Statistics.Succeeded);
            Assert.AreEqual(1, Analytics.Client.Statistics.Failed);

            // Handling Identify message will take more than 10s even though the timeout is 1s.
            // That's because it retries submit when it's failed.
            Assert.AreEqual(true, watch.ElapsedMilliseconds > 10000);
        }

        class RetryErrorTestCase
        {
            public HttpStatusCode ResponseCode;
            public string ErrorMessage;
            public int Timeout;
            public bool ShouldRetry;
            public string BaseActionUrl;
        }

        [Test()]
        public void RetryServerErrorTestNetStandard20()
        {

            Stopwatch watch = new Stopwatch();
            string DummyServerUrl = "http://localhost:9696";
            using (var DummyServer = new WebServer(DummyServerUrl))
            {

                // Set invalid host address and make timeout to 1s
                var config = new Config().SetAsync(false);
                config.SetHost(DummyServerUrl);
                config.SetTimeout(new TimeSpan(0, 0, 1));
                config.SetMaxRetryTime(new TimeSpan(0, 0, 10));
                Analytics.Initialize(Constants.WRITE_KEY, config);

                var TestCases = new RetryErrorTestCase[]
                {
                    // The errors (500 > code >= 400) doesn't require retry
                    new RetryErrorTestCase()
                    {
                        ErrorMessage = "Server Gone",
                        ResponseCode = HttpStatusCode.Gone,
                        ShouldRetry = false,
                        Timeout = 10000,
                        BaseActionUrl = "/ServerGone"
                    },
                    // 429 error requires retry
                    new RetryErrorTestCase()
                    {
                        ErrorMessage = "Too many requests",
                        ResponseCode = (HttpStatusCode)429,
                        ShouldRetry = true,
                        Timeout = 10000,
                        BaseActionUrl = "/TooManyRequests"
                    },
                    // Server errors require retry
                    new RetryErrorTestCase()
                    {
                        ErrorMessage = "Bad Gateway",
                        ResponseCode = HttpStatusCode.BadGateway,
                        ShouldRetry = true,
                        Timeout = 10000,
                        BaseActionUrl = "/BadGateWay"
                    }
                };

                foreach (var testCase in TestCases)
                {
                    // Setup Action module which returns error code
                    var actionModule = new ActionModule(testCase.BaseActionUrl, HttpVerbs.Any,(ctx) =>
                    {
                        return ctx.SendStandardHtmlAsync((int)testCase.ResponseCode);
                    });
                    DummyServer.WithModule(actionModule);
                }

                DummyServer.RunAsync();

                foreach (var testCase in TestCases) 
                {
                    Analytics.Client.Config.SetHost(DummyServerUrl + testCase.BaseActionUrl);
                    // Calculate working time for Identiy message with invalid host address
                    watch.Reset();
                    watch.Start();
                    Actions.Identify(Analytics.Client);
                    watch.Stop();

                    Assert.AreEqual(0, Analytics.Client.Statistics.Succeeded);

                    // Handling Identify message will less than 10s because the server returns GONE message.
                    // That's because it retries submit when it's failed.
                    if (testCase.ShouldRetry)
                        Assert.IsTrue(watch.ElapsedMilliseconds > testCase.Timeout);
                    else 
                        Assert.IsFalse(watch.ElapsedMilliseconds > testCase.Timeout);     
                }
            }
        }

        [Test()]
        public void RetryServerErrorWithDefaultMaxRetryTimeTestNetStandard20()
        {

            Stopwatch watch = new Stopwatch();
            string DummyServerUrl = "http://localhost:8181";
            using (var DummyServer = new WebServer(DummyServerUrl))
            {

                // Set invalid host address and make timeout to 1s
                var config = new Config().SetAsync(false);
                config.SetHost(DummyServerUrl);
                config.SetTimeout(new TimeSpan(0, 0, 1));
                Analytics.Initialize(Constants.WRITE_KEY, config);

                var TestCases = new RetryErrorTestCase[]
                {
                    // The errors (500 > code >= 400) doesn't require retry
                    new RetryErrorTestCase()
                    {
                        ErrorMessage = "Server Gone",
                        ResponseCode = HttpStatusCode.Gone,
                        ShouldRetry = false,
                        Timeout = 10000,
                        BaseActionUrl = "/ServerGone"
                    },
                    // 429 error requires retry
                    new RetryErrorTestCase()
                    {
                        ErrorMessage = "Too many requests",
                        ResponseCode = (HttpStatusCode)429,
                        ShouldRetry = true,
                        Timeout = 10000,
                        BaseActionUrl = "/TooManyRequests"
                    },
                    // Server errors require retry
                    new RetryErrorTestCase()
                    {
                        ErrorMessage = "Bad Gateway",
                        ResponseCode = HttpStatusCode.BadGateway,
                        ShouldRetry = true,
                        Timeout = 10000,
                        BaseActionUrl = "/BadGateWay"
                    }
                };

                foreach (var testCase in TestCases)
                {
                    // Setup Action module which returns error code
                    var actionModule = new ActionModule(testCase.BaseActionUrl, HttpVerbs.Any, (ctx) =>
                    {
                        return ctx.SendStandardHtmlAsync((int)testCase.ResponseCode);
                    });
                    DummyServer.WithModule(actionModule);
                }

                DummyServer.RunAsync();

                foreach (var testCase in TestCases)
                {
                    Analytics.Client.Config.SetHost(DummyServerUrl + testCase.BaseActionUrl);
                    // Calculate working time for Identiy message with invalid host address
                    watch.Reset();
                    watch.Start();
                    Actions.Identify(Analytics.Client);
                    watch.Stop();

                    Assert.AreEqual(0, Analytics.Client.Statistics.Succeeded);

                    // Handling Identify message will less than 10s because the server returns GONE message.
                    // That's because it retries submit when it's failed.
                    if (testCase.ShouldRetry)
                        Assert.IsTrue(watch.ElapsedMilliseconds > testCase.Timeout);
                    else
                        Assert.IsFalse(watch.ElapsedMilliseconds > testCase.Timeout);
                }
            }
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
