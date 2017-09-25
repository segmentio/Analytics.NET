using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Segment;
using Segment.Test;

namespace Test.Net35
{
	class Program
	{
		static void Main(string[] args)
		{
			Analytics.Initialize(Segment.Test.Constants.WRITE_KEY);

			FlushTests tests = new FlushTests();
			tests.PerformanceTestNet35();
		}
	}
}
