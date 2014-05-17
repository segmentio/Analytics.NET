using NUnit.Framework;

using System;
using System.Collections.Generic;

using Segment;
using Segment.Model;

namespace Segment.Test
{
	[TestFixture ()]
	public class ActionTests
	{

		[SetUp] 
		public void Init()
		{
            Analytics.Dispose();
            Logger.Handlers += LoggingHandler;
			Analytics.Initialize (Constants.WRITE_KEY, new Config().SetAsync(false));
		}

		[Test ()]
		public void IdentifyTest ()
		{
			Actions.Identify (Analytics.Client);
			FlushAndCheck (1);
		}
			
		[Test ()]
		public void TrackTest ()
		{
			Actions.Track (Analytics.Client);
			FlushAndCheck (1);
		}
			
		[Test ()]
		public void AliasTest ()
		{
			Actions.Alias (Analytics.Client);
			FlushAndCheck (1);
		}

		[Test ()]
		public void GroupTest ()
		{
			Actions.Group (Analytics.Client);
			FlushAndCheck (1);
		}

		[Test ()]
		public void PageTest ()
		{
			Actions.Page (Analytics.Client);
			FlushAndCheck (1);
		}

		[Test ()]
		public void ScreenTest ()
		{
			Actions.Screen (Analytics.Client);
			FlushAndCheck (1);
		}

		private void FlushAndCheck (int messages) {
			Analytics.Client.Flush();
			Assert.AreEqual(messages, Analytics.Client.Statistics.Submitted);
			Assert.AreEqual(messages, Analytics.Client.Statistics.Succeeded);
			Assert.AreEqual(0, Analytics.Client.Statistics.Failed);
		}

        static void LoggingHandler(Logger.Level level, string message, Dict args)
        {
            if (args != null)
            {
                foreach (string key in args.Keys) {
                    message += String.Format(" {0}: {1},", "" + key, "" + args[key]);
                }
            }
            Console.WriteLine(String.Format("[ActionTests] [{0}] {1}", level, message));
        }
	}


}

