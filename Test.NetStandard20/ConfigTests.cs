using NUnit.Framework;
using System;

namespace RudderStack.Test
{
    [TestFixture]
    class ConfigTests
    {
        RudderConfig _config;

        [SetUp]
        public void Init()
        {
            _config = new RudderConfig();
        }

        [Test]
        public void SetGzipUpdateTheConfigProperty()
        {
            //_config.SetGzip(true);
            //Assert.IsTrue(_config.Gzip);
            //_config.SetGzip(false);
            //Assert.IsFalse(_config.Gzip);
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

        [Test]
        public void GetGzipProperty()
        {
            _config.SetGzip(true);
            Assert.IsTrue(_config.GetGzip());
            _config.SetGzip(false);
            Assert.IsFalse(_config.GetGzip());
        }

        [Test]
        public void GetFlushIntervalProperty()
        {
            _config.SetFlushInterval(2);
            Assert.AreEqual(2, _config.GetFlushInterval());
        }

        [Test]
        public void GetFlushAtProperty()
        {
            _config.SetFlushAt(100);
            Assert.AreEqual(100, _config.GetFlushAt());
        }

        [Test]
        public void GetMaxQueueSizeProperty()
        {
            _config.SetMaxQueueSize(10000);
            Assert.AreEqual(10000, _config.GetMaxQueueSize());
        }

        [Test]
        public void GetUserAgentProperty()
        {
            _config.SetUserAgent("UserAgent");
            Assert.AreEqual("UserAgent", _config.GetUserAgent());
        }

        [Test]
        public void GetThreadsProperty()
        {
            _config.SetThreads(2);
            Assert.AreEqual(2, _config.GetThreads());
        }

        [Test]
        public void GetTimeoutProperty()
        {
            var timeSpan = new TimeSpan(0, 0, 10);
            _config.SetTimeout(timeSpan);
            Assert.AreEqual(timeSpan, _config.GetTimeout());
        }

        [Test]
        public void GetMaxRetryTimeProperty()
        {
            var timeSpan = new TimeSpan(0, 0, 10);
            _config.SetMaxRetryTime(timeSpan);
            Assert.AreEqual(timeSpan, _config.GetMaxRetryTime());
        }

        [Test]
        public void GetSendProperty()
        {
            _config.SetSend(true);
            Assert.IsTrue(_config.GetSend());
            _config.SetSend(false);
            Assert.IsFalse(_config.GetSend());
        }

        [Test]
        public void GetProxyProperty()
        {
            _config.SetProxy("testProxy");
            Assert.AreEqual("testProxy", _config.GetProxy());
        }

        [Test]
        public void GetAsyncProperty()
        {
            _config.SetAsync(true);
            Assert.IsTrue(_config.GetAsync());
            _config.SetAsync(false);
            Assert.IsFalse(_config.GetAsync());
        }

        [Test]
        public void GetHostProperty()
        {
            _config.SetHost("testHost.com");
            Assert.AreEqual("testHost.com", _config.GetHost());
        }
    }
}
