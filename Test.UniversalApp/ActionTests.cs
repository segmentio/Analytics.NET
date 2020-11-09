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
					b.batch.ForEach(_ => Analytics.Client.Statistics.IncrementSucceeded());
					return Task.CompletedTask;
				});

			Analytics.Dispose();
			Logger.Handlers += LoggingHandler;
			var client = new Client(Constants.WRITE_KEY, new Config().SetAsync(false), _mockRequestHandler.Object);
			Analytics.Initialize(client);
		}

        [TestCleanup]
        public void CleanUp()
        {
            Logger.Handlers -= LoggingHandler;
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

