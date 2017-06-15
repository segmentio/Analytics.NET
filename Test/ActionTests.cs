using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Segment.Test
{
	[TestFixture()]
	public class ActionTests
	{

		[SetUp]
		public void Init()
		{
			Analytics.Dispose();
			Logger.Handlers += LoggingHandler;
			Analytics.Initialize(Constants.WRITE_KEY, new Config().SetAsync(false));
		}

		[Test ()]
		public async Task IdentifyTest()
		{
			await Actions.Identify(Analytics.Client);
			FlushAndCheck(1);
		}

		[Test()]
		public async Task TrackTest()
		{
			await Actions.Track(Analytics.Client);
			FlushAndCheck(1);
		}

		[Test()]
		public async Task AliasTest()
		{
			await Actions.Alias(Analytics.Client);
			FlushAndCheck(1);
		}

		[Test()]
		public async Task GroupTest()
		{
			await Actions.Group(Analytics.Client);
			FlushAndCheck(1);
		}

		[Test()]
		public async Task PageTest()
		{
			await Actions.Page(Analytics.Client);
			FlushAndCheck(1);
		}

		[Test()]
		public async Task ScreenTest()
		{
			await Actions.Screen(Analytics.Client);
			FlushAndCheck(1);
		}

		private void FlushAndCheck(int messages)
		{
			Analytics.Client.Flush();
			Assert.AreEqual(messages, Analytics.Client.Statistics.Submitted);
			Assert.AreEqual(messages, Analytics.Client.Statistics.Succeeded);
			Assert.AreEqual(0, Analytics.Client.Statistics.Failed);
		}

		static void LoggingHandler(Logger.Level level, string message, IDictionary<string, object> args)
		{
			if (args != null)
			{
				foreach (string key in args.Keys)
				{
					message += String.Format(" {0}: {1},", "" + key, "" + args[key]);
				}
			}
			Console.WriteLine(String.Format("[ActionTests] [{0}] {1}", level, message));
		}
	}


}

