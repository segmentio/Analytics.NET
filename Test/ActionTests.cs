using NUnit.Framework;
using System;

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
			Analytics.Reset();
			Analytics.Initialize (Constants.WRITE_KEY, new Config().SetAsync(false));
			Analytics.Client.Succeeded += Client_Succeeded;
			Analytics.Client.Failed += Client_Failed;
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
			
		private void FlushAndCheck (int messages) {
			Analytics.Client.Flush();
			Assert.AreEqual(messages, Analytics.Client.Statistics.Submitted);
			Assert.AreEqual(messages, Analytics.Client.Statistics.Succeeded);
			Assert.AreEqual(0, Analytics.Client.Statistics.Failed);
		}

		void Client_Failed(BaseAction action, System.Exception e)
		{
			Console.WriteLine(String.Format("Action {0} failed : {1}", action.GetType(), e.Message));
		}

		void Client_Succeeded(BaseAction action)
		{
			Console.WriteLine(String.Format("Action {0} succeeded.", action.GetType()));
		}

	}


}

