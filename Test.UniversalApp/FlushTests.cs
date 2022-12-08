using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RudderStack.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Moq;
using RudderStack;
using RudderStack.Request;

namespace RudderStack.Test
{
	[TestClass]
	public class FlushTests
	{
		private Mock<IRequestHandler> _mockRequestHandler;

		[TestInitialize]
		public void Init()
		{
			_mockRequestHandler = new Mock<IRequestHandler>();
			_mockRequestHandler
				.Setup(x => x.MakeRequest(It.IsAny<Batch>()))
				.Returns((Batch b) =>
				{
                    b.batch.ForEach(_ => RudderAnalytics.Client.Statistics.IncrementSucceeded());
					return Task.CompletedTask;
				});
			RudderAnalytics.Dispose();
			Logger.Handlers += LoggingHandler;
		}

        [TestCleanup]
        public void CleanUp()
        {
            Logger.Handlers -= LoggingHandler;
        }

		[TestMethod]
		public void SynchronousFlushTestNetPortable()
		{
			var client = new RudderClient(Constants.WRITE_KEY, new RudderConfig().SetAsync(false), _mockRequestHandler.Object);
            RudderAnalytics.Initialize(client);
            RudderAnalytics.Client.Succeeded += Client_Succeeded;
			RudderAnalytics.Client.Failed += Client_Failed;

			int trials = 10;

			RunTests(RudderAnalytics.Client, trials);

			Assert.AreEqual(trials, RudderAnalytics.Client.Statistics.Submitted);
			Assert.AreEqual(trials, RudderAnalytics.Client.Statistics.Succeeded);
			Assert.AreEqual(0, RudderAnalytics.Client.Statistics.Failed);
		}

		[TestMethod]
		public void AsynchronousFlushTestNetPortable()
		{
			var client = new RudderClient(Constants.WRITE_KEY, new RudderConfig().SetAsync(true), _mockRequestHandler.Object);
            RudderAnalytics.Initialize(client);

            RudderAnalytics.Client.Succeeded += Client_Succeeded;
            RudderAnalytics.Client.Failed += Client_Failed;

			int trials = 10;

			RunTests(RudderAnalytics.Client, trials);

            RudderAnalytics.Client.Flush();

			Assert.AreEqual(trials, RudderAnalytics.Client.Statistics.Submitted);
			Assert.AreEqual(trials, RudderAnalytics.Client.Statistics.Succeeded);
			Assert.AreEqual(0, RudderAnalytics.Client.Statistics.Failed);
		}

		[TestMethod]
		public async Task PerformanceTestNetPortable()
		{
			var client = new RudderClient(Constants.WRITE_KEY, new RudderConfig(), _mockRequestHandler.Object);
            RudderAnalytics.Initialize(client);

            RudderAnalytics.Client.Succeeded += Client_Succeeded;
            RudderAnalytics.Client.Failed += Client_Failed;

			int trials = 100;

			DateTime start = DateTime.Now;

			RunTests(RudderAnalytics.Client, trials);

            RudderAnalytics.Client.Flush();

			TimeSpan duration = DateTime.Now.Subtract(start);

			Assert.AreEqual(trials, RudderAnalytics.Client.Statistics.Submitted);
			Assert.AreEqual(trials, RudderAnalytics.Client.Statistics.Succeeded);
			Assert.AreEqual(0, RudderAnalytics.Client.Statistics.Failed);

			Assert.IsTrue(duration.CompareTo(TimeSpan.FromSeconds(20)) < 0);
		}

		private void RunTests(RudderClient client, int trials)
		{
			for (int i = 0; i < trials; i += 1)
			{
				Actions.Random(client);
			}
		}

		void Client_Failed(BaseAction action, System.Exception e)
		{
			Debug.WriteLine(String.Format("Action [{0}] {1} failed : {2}",
				action.MessageId, action.Type, e.Message));
		}

		void Client_Succeeded(BaseAction action)
		{
			Debug.WriteLine(String.Format("Action [{0}] {1} succeeded.",
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
			Debug.WriteLine(String.Format("[FlushTests] [{0}] {1}", level, message));
		}
	}
}

