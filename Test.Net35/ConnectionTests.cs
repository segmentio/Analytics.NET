using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace Segment.Test
{
    [TestFixture()]
    public class ConnectionTests
    {
        [SetUp]
        public void Init()
        {
            Logger.Handlers += LoggingHandler;
        }

        [TearDown]
        public void Dispose()
        {
            Analytics.Dispose();
            Logger.Handlers -= LoggingHandler;
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

        [Test()]
        public void BatchSendTestNet35()
        {
            Analytics.Initialize(Constants.WRITE_KEY);

            int trials = 1000;
            for (int i = 1; i <= trials; i++)
            {
                Actions.Random(Analytics.Client);
                Analytics.Client.Flush();

                Assert.AreEqual(i, Analytics.Client.Statistics.Submitted);
                Assert.AreEqual(i, Analytics.Client.Statistics.Succeeded);
                Assert.AreEqual(0, Analytics.Client.Statistics.Failed);

                Thread.Sleep(500);
            }
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
