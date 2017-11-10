using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Segment.Model;
using Xunit;

namespace Segment.Test
{
	public class FlushTests
	{
		public FlushTests()
		{
			Analytics.Dispose();
			Logger.Handlers += LoggingHandler;
		}

		[Fact()]
		public void SynchronousFlushTest()
		{
			Analytics.Initialize(Constants.WRITE_KEY, new Config().SetAsync(false));
			Analytics.Client.Succeeded += Client_Succeeded;
			Analytics.Client.Failed += Client_Failed;

			int trials = 10;

			RunTests(Analytics.Client, trials);

			Assert.Equal(trials, Analytics.Client.Statistics.Submitted);
			Assert.Equal(trials, Analytics.Client.Statistics.Succeeded);
			Assert.Equal(0, Analytics.Client.Statistics.Failed);
		}

		[Fact()]
		public void AsynchronousFlushTest()
		{
			Analytics.Initialize(Constants.WRITE_KEY, new Config().SetAsync(true));

			Analytics.Client.Succeeded += Client_Succeeded;
			Analytics.Client.Failed += Client_Failed;

			int trials = 10;

			RunTests(Analytics.Client, trials);

			Thread.Sleep(1000); // cant use flush to wait during asynchronous flushing

			Assert.Equal(trials, Analytics.Client.Statistics.Submitted);
			Assert.Equal(trials, Analytics.Client.Statistics.Succeeded);
			Assert.Equal(0, Analytics.Client.Statistics.Failed);
		}

		[Fact()]
		public async Task PerformanceTest()
		{
			Analytics.Initialize(Constants.WRITE_KEY);

			Analytics.Client.Succeeded += Client_Succeeded;
			Analytics.Client.Failed += Client_Failed;

			int trials = 100;

			DateTime start = DateTime.Now;

			RunTests(Analytics.Client, trials);

			Analytics.Client.Flush();

			TimeSpan duration = DateTime.Now.Subtract(start);

			Assert.Equal(trials, Analytics.Client.Statistics.Submitted);
			Assert.Equal(trials, Analytics.Client.Statistics.Succeeded);
			Assert.Equal(0, Analytics.Client.Statistics.Failed);

			Assert.True(duration.CompareTo(TimeSpan.FromSeconds(20)) < 0);
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
			Console.WriteLine(String.Format("Action [{0}] {1} failed : {2}",
				action.MessageId, action.Type, e.Message));
		}

		void Client_Succeeded(BaseAction action)
		{
			Console.WriteLine(String.Format("Action [{0}] {1} succeeded.",
				action.MessageId, action.Type));
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
			Console.WriteLine(String.Format("[FlushTests] [{0}] {1}", level, message));
		}
	}
}
