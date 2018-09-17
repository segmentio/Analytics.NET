using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Segment.Test
{
    [TestClass]
    public class ConnectionTests
    {
        [TestInitialize]
        public void Init()
        {
            Analytics.Dispose();
            Logger.Handlers += LoggingHandler;
        }

        [TestMethod]
        public void ProxyTestNetPortable()
        {
            // Set proxy address, like as "http://localhost:8888"
            Analytics.Initialize(Constants.WRITE_KEY, new Config().SetAsync(false).SetProxy(""));

            Actions.Identify(Analytics.Client);

            Assert.AreEqual(1, Analytics.Client.Statistics.Submitted);
            Assert.AreEqual(1, Analytics.Client.Statistics.Succeeded);
            Assert.AreEqual(0, Analytics.Client.Statistics.Failed);
        }

        [TestMethod]
        public void GZipTestNetPortable()
        {
            // Set proxy address, like as "http://localhost:8888"
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
            Debug.WriteLine(String.Format("[ConnectionTests] [{0}] {1}", level, message));
        }
    }
}
