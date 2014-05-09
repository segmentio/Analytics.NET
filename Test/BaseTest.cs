using NUnit.Framework;
using System;

using Segmentio;
using Segmentio.Model;

namespace Segment.Test
{
	[TestFixture ()]
	public class BaseTest
	{
		// project segmentio/dotnet-test
		private const string WRITE_KEY = "r7bxis28wy";

		[Test ()]
		public void SynchronousFlushTest ()
		{
			Analytics.Reset();

			Analytics.Initialize(WRITE_KEY, new Options().SetAsync(false));

			Analytics.Client.Succeeded += Client_Succeeded;
			Analytics.Client.Failed += Client_Failed;

			int trials = 10;

			RunTests(Analytics.Client, trials);

			Assert.IsTrue(Analytics.Client.Statistics.Submitted == trials);
			Assert.IsTrue(Analytics.Client.Statistics.Succeeded == trials);
			Assert.IsTrue(Analytics.Client.Statistics.Failed == 0);
		}

		[Test ()]
		public void AsynchronousFlushTest()
		{
			Analytics.Reset();

			Analytics.Initialize(WRITE_KEY, new Options().SetAsync(true));

			Analytics.Client.Succeeded += Client_Succeeded;
			Analytics.Client.Failed += Client_Failed;

			int trials = 10;

			RunTests(Analytics.Client, trials);

			Analytics.Client.Flush();

			Assert.IsTrue(Analytics.Client.Statistics.Submitted == trials);
			Assert.IsTrue(Analytics.Client.Statistics.Succeeded == trials);
			Assert.IsTrue(Analytics.Client.Statistics.Failed == 0);
		}


		[Test ()]
		public void PerformanceTest()
		{
			Analytics.Reset();

			Analytics.Initialize(WRITE_KEY);

			Analytics.Client.Succeeded += Client_Succeeded;
			Analytics.Client.Failed += Client_Failed;

			int trials = 100;

			DateTime start = DateTime.Now;

			RunTests(Analytics.Client, trials);

			Analytics.Client.Flush();

			TimeSpan duration = DateTime.Now.Subtract(start);

			Assert.IsTrue(Analytics.Client.Statistics.Submitted == trials);
			Assert.IsTrue(Analytics.Client.Statistics.Succeeded == trials);
			Assert.IsTrue(Analytics.Client.Statistics.Failed == 0);

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

