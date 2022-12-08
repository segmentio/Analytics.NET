using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;

using RudderStack;
using RudderStack.Model;

namespace RudderStack.Test
{
	[TestClass]
	public class ClientTests
	{
		RudderClient client;

		[TestInitialize]
		public void Init()
		{
			client = new RudderClient("foo");
		}

		[TestMethod]
		public void TrackTestNetPortable()
		{
			// verify it doesn't fail for a null options
			client.Screen("bar", "qaz", null, null);
		}
	}
}

