using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;

namespace Segment.Test
{
    [TestFixture()]
    public class ConnectionTests
    {
        [SetUp]
        public void Init()
        {
            Analytics.Dispose();
            Logger.Handlers += LoggingHandler;
        }

        [Test()]
        public void RetryErrorTestNet35()
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

        [Test()]
        public void ProxyTestNet35()
        {
            // Set proxy address, like as "http://localhost:8888"
            Analytics.Initialize(Constants.WRITE_KEY, new Config().SetAsync(false).SetProxy(""));

            Actions.Identify(Analytics.Client);

            Assert.AreEqual(1, Analytics.Client.Statistics.Submitted);
            Assert.AreEqual(1, Analytics.Client.Statistics.Succeeded);
            Assert.AreEqual(0, Analytics.Client.Statistics.Failed);
        }

        [Test()]
        public void GZipTestNet35()
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
