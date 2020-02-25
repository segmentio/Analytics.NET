using NUnit.Framework;

using System;
using System.Collections.Generic;

using Segment;
using Segment.Model;

namespace Segment.Test
{
    [TestFixture ()]
    public class ClientTests
    {
        Client client;

        [SetUp]
        public void Init()
        {
            client = new Client("foo");
        }


        [Test]
        public void InitilizationThrowsInvalidOperationExceptionWhenWriteKeyIsEmpty()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => new Client(""));
            Assert.AreEqual("Please supply a valid writeKey to initialize.", ex.Message);
        }

        [Test]
        public void PageThrowsInvalidOperationExceptionWhenPageNameIsEmtpy()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => client.Page("userId", ""));

            Assert.AreEqual("Please supply a valid name to call #Page.", ex.Message);
        }

        [Test]
        public void PageThrowsInvalidOperationExceptionWhenUserIdIsEmtpy()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => client.Page("", ""));

            Assert.AreEqual("Please supply a valid userId or anonymousId to call #Page.", ex.Message);
        }

        [Test]
        public void AliasThrowsInvalidOperationExceptionWhenUserIdIsEmtpy()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => client.Alias("previousId", ""));

            Assert.AreEqual("Please supply a valid 'userId' to Alias.", ex.Message);
        }

        [Test]
        public void AliasThrowsInvalidOperationExceptionWhenPreviousIdIsEmtpy()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => client.Alias("", ""));

            Assert.AreEqual("Please supply a valid 'previousId' to Alias.", ex.Message);
        }

        [Test]
        public void GroupThrowsInvalidOperationExceptionWhenGroupIdIsEmtpy()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => client.Group("userId", "", new Options()));

            Assert.AreEqual("Please supply a valid groupId to call #Group.", ex.Message);
        }

        [Test]
        public void GroupThrowsInvalidOperationExceptionWhenUserIdIsEmtpy()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => client.Group("", "", (Options)null));

            Assert.AreEqual("Please supply a valid userId or anonymousId to call #Group.", ex.Message);
        }


        [Test ()]
        public void TrackTestNet35 ()
        {
            // verify it doesn't fail for a null options
            client.Screen("bar", "qaz", null, null);
        }
    }
}

