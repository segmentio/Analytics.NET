using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Segment.Test
{
    [TestFixture()]
    public class ActionTests
    {

        [SetUp]
        public void Init()
        {
            Analytics.Dispose();
            Logger.Handlers += LoggingHandler;
            Analytics.Initialize(Constants.WRITE_KEY, new Config().SetAsync(false));
        }

        [TearDown]
        public void CleanUp()
        {
            Logger.Handlers -= LoggingHandler;
        }

        [Test ()]
        public void IdentifyTestNetStanard20()
        {
            Actions.Identify(Analytics.Client);
            FlushAndCheck(1);
        }

        [Test()]
        public void TrackTestNetStanard20()
        {
            Actions.Track(Analytics.Client);
            FlushAndCheck(1);
        }

        [Test()]
        public void AliasTestNetStanard20()
        {
            Actions.Alias(Analytics.Client);
            FlushAndCheck(1);
        }

        [Test()]
        public void GroupTestNetStanard20()
        {
            Actions.Group(Analytics.Client);
            FlushAndCheck(1);
        }

        [Test()]
        public void PageTestNetStanard20()
        {
            Actions.Page(Analytics.Client);
            FlushAndCheck(1);
        }

        [Test()]
        public void ScreenTestNetStanard20()
        {
            Actions.Screen(Analytics.Client);
            FlushAndCheck(1);
        }

        private void FlushAndCheck(int messages)
        {
            Analytics.Client.Flush();
            Assert.AreEqual(messages, Analytics.Client.Statistics.Submitted);
            Assert.AreEqual(messages, Analytics.Client.Statistics.Succeeded);
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
            Console.WriteLine(String.Format("[ActionTests] [{0}] {1}", level, message));
        }
    }
}

