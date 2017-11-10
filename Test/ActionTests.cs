using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Segment.Test
{
	public class ActionTests
	{
		public ActionTests()
		{
			Analytics.Dispose();
			Logger.Handlers += LoggingHandler;
			Analytics.Initialize(Constants.WRITE_KEY, new Config().SetAsync(false));
		}

		[Fact()]
		public void IdentifyTest()
		{
			Actions.Identify(Analytics.Client);
			FlushAndCheck(1);
		}

		[Fact()]
		public void TrackTest()
		{
			Actions.Track(Analytics.Client);
			FlushAndCheck(1);
		}

		[Fact()]
		public void AliasTest()
		{
			Actions.Alias(Analytics.Client);
			FlushAndCheck(1);
		}

		[Fact()]
		public void GroupTest()
		{
			Actions.Group(Analytics.Client);
			FlushAndCheck(1);
		}

		[Fact()]
		public void PageTest()
		{
			Actions.Page(Analytics.Client);
			FlushAndCheck(1);
		}

		[Fact()]
		public void ScreenTest()
		{
			Actions.Screen(Analytics.Client);
			FlushAndCheck(1);
		}

		private void FlushAndCheck(int messages)
		{
			Analytics.Client.Flush();
			Assert.Equal(messages, Analytics.Client.Statistics.Submitted);
			Assert.Equal(messages, Analytics.Client.Statistics.Succeeded);
			Assert.Equal(0, Analytics.Client.Statistics.Failed);
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
