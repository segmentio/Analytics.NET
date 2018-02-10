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
		class TestClient : Client
		{
			public TestClient() : base("catpants")
			{
			}

			public bool EnsureId(String userId, Options options)
			{
				try
				{
					base.ensureId(userId, options);
				}
				catch
				{
					return false;
				}

				return true;
			}
		}

		[Test]
		public void HasUserId_NoOptions()
		{
			var test = new TestClient();
			Assert.IsTrue(test.EnsureId("user id", null));
		}

		[Test]
		public void NoUserId_HasAnonId()
		{
			var test = new TestClient();

			var options = new Options();
			options.SetAnonymousId("anon id");
			Assert.IsTrue(test.EnsureId("", options));
		}

		[Test]
		public void NoUserId_NoAnon()
		{
			var test = new TestClient();

			Assert.IsFalse(test.EnsureId("", new Options()));
		}

		[Test]
		public void NoUserId_NoOptions()
		{
			var test = new TestClient();

			Assert.IsFalse(test.EnsureId("", null));
		}
	}
}

