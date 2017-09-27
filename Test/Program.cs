using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Segment;
using Segment.Test;

namespace Segment.Test
{
	class Program
	{
		static void Main(string[] args)
		{
			Analytics.Initialize(Segment.Test.Constants.WRITE_KEY);

			FlushTests tests = new FlushTests();
			tests.PerformanceTest().Wait();
		}
	}
}
