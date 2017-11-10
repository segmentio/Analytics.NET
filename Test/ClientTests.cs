using System;
using System.Collections.Generic;
using Segment;
using Segment.Model;
using Xunit;

namespace Segment.Test
{
	public class ClientTests
	{
		Client client;

		public ClientTests()
		{
			client = new Client("foo");
		}

		[Fact()]
		public void TrackTest ()
		{
			// verify it doesn't fail for a null options
			client.Screen("bar", "qaz", null, null);
		}
	}
}
