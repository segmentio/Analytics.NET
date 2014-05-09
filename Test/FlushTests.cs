using NUnit.Framework;
using System;
using System.Threading;

using Segment;
using Segment.Model;

namespace Segment.Test
{
	[TestFixture ()]
	public class FlushTests
	{
		[SetUp] 
		public void Init()
		{
			Analytics.Reset();
		}

		[Test ()]
		public void SynchronousFlushTest ()
		{
			Analytics.Initialize(Constants.WRITE_KEY, new Options().SetAsync(false));
			Analytics.Client.Succeeded += Client_Succeeded;
			Analytics.Client.Failed += Client_Failed;

			int trials = 10;

			RunTests(Analytics.Client, trials);

			Assert.AreEqual(trials, Analytics.Client.Statistics.Submitted);
			Assert.AreEqual(trials, Analytics.Client.Statistics.Succeeded);
			Assert.AreEqual(0, Analytics.Client.Statistics.Failed);
		}

		[Test ()]
		public void AsynchronousFlushTest()
		{
			Analytics.Initialize(Constants.WRITE_KEY, new Options().SetAsync(true));

			Analytics.Client.Succeeded += Client_Succeeded;
			Analytics.Client.Failed += Client_Failed;

			int trials = 10;

			RunTests(Analytics.Client, trials);

			Thread.Sleep (500); // cant use flush to wait during asynchronous flushing

			Assert.AreEqual(trials, Analytics.Client.Statistics.Submitted);
			Assert.AreEqual(trials, Analytics.Client.Statistics.Succeeded);
			Assert.AreEqual(0, Analytics.Client.Statistics.Failed);
		}

		[Test ()]
		public void PerformanceTest()
		{
			Analytics.Initialize(Constants.WRITE_KEY);

			Analytics.Client.Succeeded += Client_Succeeded;
			Analytics.Client.Failed += Client_Failed;

			int trials = 100;

			DateTime start = DateTime.Now;

			RunTests(Analytics.Client, trials);

			Analytics.Client.Flush();

			TimeSpan duration = DateTime.Now.Subtract(start);

			Assert.AreEqual(trials, Analytics.Client.Statistics.Submitted);
			Assert.AreEqual(trials, Analytics.Client.Statistics.Succeeded);
			Assert.AreEqual(0, Analytics.Client.Statistics.Failed);

			Assert.IsTrue(duration.CompareTo(TimeSpan.FromSeconds(10)) < 0);
		}

		private void RunTests(Client client, int trials)
		{
			for (int i = 0; i < trials; i += 1)
			{
				Actions.Random(client);
			}
		}

		void Client_Failed(BaseAction action, System.Exception e)
		{
			Console.WriteLine(String.Format("Action {0} failed : {1}", action.GetAction(), e.Message));
		}

		void Client_Succeeded(BaseAction action)
		{
			Console.WriteLine(String.Format("Action {0} succeeded.", action.GetAction()));
		}

	}


}

