using NUnit.Framework;

namespace Segment.Test
{
    [TestFixture]
    class ConfigTests
    {
        Config _config;

        [SetUp]
        public void Init()
        {
            _config = new Config();
        }

        [Test]
        public void SetGzipUpdateTheConfigProperty()
        {
            _config.SetGzip(true);
            Assert.IsTrue(_config.Gzip);
            _config.SetGzip(false);
            Assert.IsFalse(_config.Gzip);
        }

        [Test]
        public void SetFlushIntervalUpdateTheConfigProperty()
        {
            _config.SetFlushInterval(2);
            Assert.AreEqual(2000,_config.FlushIntervalInMillis);
        }

        [Test]
        public void SetFlushAtUpdateTheConfigProperty()
        {
            _config.SetFlushAt(100);
            Assert.AreEqual(100, _config.FlushAt);
        }

        [Test]
        public void SetMaxBatchSizeUpdateTheConfigProperty()
        {
            _config.SetMaxBatchSize(100);
            Assert.AreEqual(100, _config.FlushAt);
        }

        [Test]
        public void SetMaxQueueSizeUpdateTheConfigProperty()
        {
            _config.SetMaxQueueSize(10000);
            Assert.AreEqual(10000, _config.MaxQueueSize);
        }

        [Test]
        public void SetUserAgentUpdateTheConfigProperty()
        {
            _config.SetUserAgent("UserAgent");
            Assert.AreEqual("UserAgent", _config.UserAgent);
        }
    }
}
