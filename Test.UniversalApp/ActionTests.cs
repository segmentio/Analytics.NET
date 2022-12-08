using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RudderStack.Model;
using RudderStack.Request;

namespace RudderStack.Test
{
	[TestClass]
	public class ActionTests
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
			var client = new RudderClient(Constants.WRITE_KEY, new RudderConfig().SetAsync(false), _mockRequestHandler.Object);
            RudderAnalytics.Initialize(client);
		}

        [TestCleanup]
        public void CleanUp()
        {
            Logger.Handlers -= LoggingHandler;
        }

		[TestMethod]
		public void IdentifyTestNetPortable()
		{
			Actions.Identify(RudderAnalytics.Client);
			FlushAndCheck(1);
		}

		[TestMethod]
		public void TrackTestNetPortable()
		{
			Actions.Track(RudderAnalytics.Client);
			FlushAndCheck(1);
		}

		[TestMethod]
		public void AliasTestNetPortable()
		{
			Actions.Alias(RudderAnalytics.Client);
			FlushAndCheck(1);
		}

		[TestMethod]
		public void GroupTestNetPortable()
		{
			Actions.Group(RudderAnalytics.Client);
			FlushAndCheck(1);
		}

		[TestMethod]
		public void PageTestNetPortable()
		{
			Actions.Page(RudderAnalytics.Client);
			FlushAndCheck(1);
		}

		[TestMethod]
		public void ScreenTestNetPortable()
		{
			Actions.Screen(RudderAnalytics.Client);
			FlushAndCheck(1);
		}

		private void FlushAndCheck(int messages)
		{
            RudderAnalytics.Client.Flush();
			Assert.AreEqual(messages, RudderAnalytics.Client.Statistics.Submitted);
			Assert.AreEqual(messages, RudderAnalytics.Client.Statistics.Succeeded);
			Assert.AreEqual(0, RudderAnalytics.Client.Statistics.Failed);
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

