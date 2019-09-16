using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Segment.Test
{
	[TestFixture()]
	public class RequestHandlerTest
	{
		[SetUp]
		public void Init()
		{
			Analytics.Dispose();
		}

		[Test()]
		public void HeaderTestNet35()
		{
			Assert.IsTrue(true);
		}
	}
}
