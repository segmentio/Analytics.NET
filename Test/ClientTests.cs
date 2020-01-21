using NUnit.Framework;

using System;
using System.Collections.Generic;

using Segment;
using Segment.Model;

namespace Segment.Test
{
	[TestFixture()]
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

		[Test ()]
        public void TrackTest ()
        {
            // verify it doesn't fail for a null options
            client.Screen("bar", "qaz", null, null);
        }
    }
}

