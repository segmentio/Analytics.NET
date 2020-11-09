using NUnit.Framework;

using System;
using Moq;
using RudderStack.Flush;
using RudderStack.Model;
using RudderStack.Request;

namespace RudderStack.Test
{
    [TestFixture()]
    public class ClientTests
    {
        Client _client;

        [SetUp]
        public void Init()
        {
            _client = new Client("foo", new Config(), Mock.Of<IRequestHandler>());
        }

        [Test]
        public void InitializationThrowsInvalidOperationExceptionWhenWriteKeyIsEmpty()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => new Client(""));
            Assert.AreEqual("Please supply a valid writeKey to initialize.", ex.Message);
        }

        [Test]
        public void VerifyWriteKey()
        {
            Assert.AreEqual("foo", _client.WriteKey);
        }

        [Test]
        public void PageSubmit()
        {
            _client.Page("UserId", "Name");
            _client.Page("UserId", "Name", "Category");
            _client.Page("UserId", "Name", properties: null);
            _client.Page("UserId", "Name", options: null);
            _client.Page("UserId", "Name", null, null);

            Assert.AreEqual(5, _client.Statistics.Submitted);
        }

        [Test]
        public void PageThrowsInvalidOperationExceptionWhenPageNameIsEmpty()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _client.Page("userId", ""));

            Assert.AreEqual("Please supply a valid name to call #Page.", ex.Message);
        }

        [Test]
        public void PageThrowsInvalidOperationExceptionWhenUserIdIsEmpty()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _client.Page("", ""));

            Assert.AreEqual("Please supply a valid userId or anonymousId to call #Page.", ex.Message);
        }

        [Test]
        public void AliasSubmit()
        {
            _client.Alias("PreviousId", "UserId");
            _client.Alias("PreviousId", "UserId", options: null);

            Assert.AreEqual(2, _client.Statistics.Submitted);
        }

        [Test]
        public void AliasThrowsInvalidOperationExceptionWhenUserIdIsEmpty()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _client.Alias("previousId", ""));

            Assert.AreEqual("Please supply a valid 'userId' to Alias.", ex.Message);
        }

        [Test]
        public void AliasThrowsInvalidOperationExceptionWhenPreviousIdIsEmpty()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _client.Alias("", ""));

            Assert.AreEqual("Please supply a valid 'previousId' to Alias.", ex.Message);
        }

        [Test]
        public void GroupSubmit()
        {
            _client.Group("UserId", "GroupId", traits: null);
            _client.Group("UserId", "GroupId", null, null);
            _client.Group("UserId", "GroupId", options: null);

            Assert.AreEqual(3, _client.Statistics.Submitted);
        }

        [Test]
        public void GroupThrowsInvalidOperationExceptionWhenGroupIdIsEmpty()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _client.Group("userId", "", new Options()));

            Assert.AreEqual("Please supply a valid groupId to call #Group.", ex.Message);
        }

        [Test]
        public void GroupThrowsInvalidOperationExceptionWhenUserIdIsEmpty()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _client.Group("", "", (Options)null));

            Assert.AreEqual("Please supply a valid userId or anonymousId to call #Group.", ex.Message);
        }

        [Test]
        public void TrackSubmit()
        {
            _client.Track("UserId", "EventName");
            _client.Track("UserId", "EventName", null, null);
            _client.Track("UserId", "EventName", options: null);
            _client.Track("UserId", "EventName", properties: null);

            Assert.AreEqual(4, _client.Statistics.Submitted);
        }

        [Test]
        public void TrackThrowsInvalidOperationExceptionWhenUserIdIsEmpty()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _client.Track("", "", options: null));
            Assert.AreEqual("Please supply a valid userId or anonymousId to call #Track.", ex.Message);
        }

        [Test]
        public void TrackThrowsInvalidOperationExceptionWhenEvenNameIsEmpty()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _client.Track("userId", "", new Options()));

            Assert.AreEqual("Please supply a valid event to call #Track.", ex.Message);
        }

        [Test]
        public void ScreenSubmit()
        {
            _client.Screen("UserId", "Name");
            _client.Screen("UserId", "Name");
            _client.Screen("UserId", "Name", "Category");
            _client.Screen("UserId", "Name", null, null);
            _client.Screen("UserId", "Name", options: null);
            _client.Screen("UserId", "Name", properties: null);

            Assert.AreEqual(6, _client.Statistics.Submitted);
        }

        [Test]
        public void ScreenThrowsInvalidOperationExceptionWhenUserIdIsEmpty()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _client.Screen("", "", (Options)null));

            Assert.AreEqual("Please supply a valid userId or anonymousId to call #Screen.", ex.Message);
        }

        [Test]
        public void ScreenThrowsInvalidOperationExceptionWhenNameIsEmpty()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _client.Screen("userId", "", new Options()));

            Assert.AreEqual("Please supply a valid name to call #Screen.", ex.Message);
        }

        [Test]
        public void IdentifySubmit()
        {
            _client.Identify("userId", null);
            _client.Identify("userId", null, null);

            Assert.AreEqual(2, _client.Statistics.Submitted);
        }

        [Test]
        public void IdentifyThrowsInvalidOperationExceptionWhenUserIdIsEmpty()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _client.Identify("", null));

            Assert.AreEqual("Please supply a valid userId to Identify.", ex.Message);
        }

        [Test]
        public void ClientUsesFakeRequestHandlerWhenSendIsTrue()
        {
            var client = new Client("writeKey", new Config(send: true));

            var flushHandler = GetPrivateFieldValue<AsyncIntervalFlushHandler>(client, "_flushHandler");

            var requestHandler = GetPrivateFieldValue<object>(flushHandler, "_requestHandler");

            Assert.IsInstanceOf<FakeRequestHandler>(requestHandler);
        }

        [Test]
        public void ClientUsesBlockingRequestHandlerWhenSendIsFalse()
        {
            var client = new Client("writeKey", new Config(send: false));

            var flushHandler = GetPrivateFieldValue<AsyncIntervalFlushHandler>(client, "_flushHandler");

            var requestHandler = GetPrivateFieldValue<object>(flushHandler, "_requestHandler");

            Assert.IsInstanceOf<BlockingRequestHandler>(requestHandler);
        }

        private static T GetPrivateFieldValue<T>(object obj, string field)
        {
            var type = obj.GetType();
            var prop = type.GetField(field,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            return (T)prop?.GetValue(obj);

        }
    }
}

