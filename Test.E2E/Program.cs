using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Segment;

namespace Segment.E2ETest
{
	class Program
	{
		static void Main_Exe(string[] args)
		{
			Logger.Handlers += Logger_Handlers;

			Tests test = new Tests();
			test.Test();
		}

		private static void Logger_Handlers(Logger.Level level, string message, IDictionary<string, object> args)
		{
			Console.WriteLine(message);
		}
	}
}
