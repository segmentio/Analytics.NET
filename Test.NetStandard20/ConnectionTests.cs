using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Labs.EmbedIO.Modules;

namespace Segment.Test
{
    [TestFixture()]
    public class ConnectionTests
    {
        private string DummyServerUrl = "http://localhost:9696";
        private WebServer DummyServer;

        class RetryErrorTestCase
        {
            public HttpStatusCode ResponseCode;
            public string ErrorMessage;
            public int Timeout;
            public bool LessThan;
        }

        [SetUp]
        public void Init()
        {
            Analytics.Dispose();
            Logger.Handlers += LoggingHandler;

            DummyServer = new WebServer(DummyServerUrl);
            DummyServer.RunAsync();
        }

        [TearDown]
        public void Dispose()
        {
            if (DummyServer != null)
            {
                DummyServer.Dispose();
                DummyServer = null;
            }
        }

        [Test()]
        public void RetryConnectionErrorTestNetStandard20()
        {
            Stopwatch watch = new Stopwatch();

            // Set invalid host address and make timeout to 1s
            var config = new Config().SetAsync(false);
            config.SetHost("https://api.segment.com");
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

        [Test()]
        public void RetryServerErrorTestNetStandard20()
        {
            Stopwatch watch = new Stopwatch();

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
                    LessThan = true,
                    Timeout = 10000
                },
                // 429 error requires retry
                new RetryErrorTestCase()
                {
                    ErrorMessage = "Too many requests",
                    ResponseCode = (HttpStatusCode)429,
                    LessThan = false,
                    Timeout = 10000
                },
                // Server errors require retry
                new RetryErrorTestCase()
                {
                    ErrorMessage = "Bad Gateway",
                    ResponseCode = HttpStatusCode.BadGateway,
                    LessThan = false,
                    Timeout = 10000
                }
            };

            foreach (var testCase in TestCases)
            {
                // Setup fallback module which returns error code
                var fallbackModule = new FallbackModule((ctx, ct) =>
                {
                    return ctx.JsonExceptionResponse(
                        new System.Exception(testCase.ErrorMessage), testCase.ResponseCode);
                });
                DummyServer.RegisterModule(fallbackModule);

                // Calculate working time for Identiy message with invalid host address
                watch.Start();
                Actions.Identify(Analytics.Client);
                watch.Stop();

                DummyServer.UnregisterModule(typeof(FallbackModule));

                Assert.AreEqual(0, Analytics.Client.Statistics.Succeeded);

                // Handling Identify message will less than 10s because the server returns GONE message.
                // That's because it retries submit when it's failed.
                Assert.AreEqual(testCase.LessThan, watch.ElapsedMilliseconds < testCase.Timeout);
            }
        }

        [Test()]
        public void ProxyTestNetStanard20()
        {
            // Set proxy address, like as "http://localhost:8888"
            Analytics.Initialize(Constants.WRITE_KEY, new Config().SetAsync(false).SetProxy(""));

            Actions.Identify(Analytics.Client);

            Assert.AreEqual(1, Analytics.Client.Statistics.Submitted);
            Assert.AreEqual(1, Analytics.Client.Statistics.Succeeded);
            Assert.AreEqual(0, Analytics.Client.Statistics.Failed);
        }

        [Test()]
        public void GZipTestNetStanard20()
        {
            // Set GZip/Deflate on request header
            Analytics.Initialize(Constants.WRITE_KEY, new Config().SetAsync(false).SetRequestCompression(true));

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
