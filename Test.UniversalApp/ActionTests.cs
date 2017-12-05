using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Segment.Test
{
	[TestClass]
	public class ActionTests
	{
		[TestInitialize]
		public void Init()
		{
			Analytics.Dispose();
			Logger.Handlers += LoggingHandler;
			Analytics.Initialize(Constants.WRITE_KEY, new Config().SetAsync(false));
		}

		[TestMethod]
		public void IdentifyTestNetPortable()
		{
			Actions.Identify(Analytics.Client);
			FlushAndCheck(1);
		}

		[TestMethod]
		public void TrackTestNetPortable()
		{
			Actions.Track(Analytics.Client);
			FlushAndCheck(1);
		}

		[TestMethod]
		public void AliasTestNetPortable()
		{
			Actions.Alias(Analytics.Client);
			FlushAndCheck(1);
		}

		[TestMethod]
		public void GroupTestNetPortable()
		{
			Actions.Group(Analytics.Client);
			FlushAndCheck(1);
		}

		[TestMethod]
		public void PageTestNetPortable()
		{
			Actions.Page(Analytics.Client);
			FlushAndCheck(1);
		}

		[TestMethod]
		public void ScreenTestNetPortable()
		{
			Actions.Screen(Analytics.Client);
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
			Debug.WriteLine(String.Format("[ActionTests] [{0}] {1}", level, message));
		}
	}
}

