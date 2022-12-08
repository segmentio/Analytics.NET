using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RudderStack.Request;
using RudderStack.Model;
using System.Threading.Tasks;

namespace RudderStack.Test
{
	[TestClass]
	public class ConnectionTests
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
		public void RetryErrorTestPortable()
		{
			Stopwatch watch = new Stopwatch();

			// Set invalid host address and make timeout to 1s
			var config = new RudderConfig().SetAsync(false);
			config.SetHost("https://fake.rudder-server.com");
			config.SetTimeout(new TimeSpan(0, 0, 1));
			config.SetMaxRetryTime(new TimeSpan(0, 0, 10));
            RudderAnalytics.Initialize(Constants.WRITE_KEY, config);

			// Calculate working time for Identiy message with invalid host address
			watch.Start();
			Actions.Identify(RudderAnalytics.Client);
			watch.Stop();

			Assert.AreEqual(1, RudderAnalytics.Client.Statistics.Submitted);
			Assert.AreEqual(0, RudderAnalytics.Client.Statistics.Succeeded);
			Assert.AreEqual(1, RudderAnalytics.Client.Statistics.Failed);

			// Handling Identify message will take more than 10s even though the timeout is 1s.
			// That's because it retries submit when it's failed.
			Assert.AreEqual(true, watch.ElapsedMilliseconds > 10000);
		}

		[TestMethod]
		public void RetryErrorWithDefaultMaxRetryTimeTestPortable()
		{
			Stopwatch watch = new Stopwatch();

			// Set invalid host address and make timeout to 1s
			var config = new RudderConfig().SetAsync(false);
			config.SetHost("https://fake.rudder-server.com");
			config.SetTimeout(new TimeSpan(0, 0, 1));
            RudderAnalytics.Initialize(Constants.WRITE_KEY, config);

			// Calculate working time for Identiy message with invalid host address
			watch.Start();
			Actions.Identify(RudderAnalytics.Client);
			watch.Stop();

			Assert.AreEqual(1, RudderAnalytics.Client.Statistics.Submitted);
			Assert.AreEqual(0, RudderAnalytics.Client.Statistics.Succeeded);
			Assert.AreEqual(1, RudderAnalytics.Client.Statistics.Failed);

			// Handling Identify message will take more than 10s even though the timeout is 1s.
			// That's because it retries submit when it's failed.
			Assert.AreEqual(true, watch.ElapsedMilliseconds > 10000);
		}

		[TestMethod]
		public void ProxyTestNetPortable()
		{
			// Set proxy address, like as "http://localhost:8888"
			var client = new RudderClient(Constants.WRITE_KEY, new RudderConfig().SetAsync(false).SetProxy(""), _mockRequestHandler.Object);
            RudderAnalytics.Initialize(client);

			Actions.Identify(RudderAnalytics.Client);

			Assert.AreEqual(1, RudderAnalytics.Client.Statistics.Submitted);
			Assert.AreEqual(1, RudderAnalytics.Client.Statistics.Succeeded);
			Assert.AreEqual(0, RudderAnalytics.Client.Statistics.Failed);
		}

		[TestMethod]
		public void GZipTestNetPortable()
		{
			// Set proxy address, like as "http://localhost:8888"
			var client = new RudderClient(Constants.WRITE_KEY, new RudderConfig().SetAsync(false).SetGzip(true), _mockRequestHandler.Object);
            RudderAnalytics.Initialize(client);

			Actions.Identify(RudderAnalytics.Client);

			Assert.AreEqual(1, RudderAnalytics.Client.Statistics.Submitted);
			Assert.AreEqual(1, RudderAnalytics.Client.Statistics.Succeeded);
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
			Debug.WriteLine(String.Format("[ConnectionTests] [{0}] {1}", level, message));
		}
	}
}
