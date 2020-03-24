using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Segment.Request;
using Segment.Model;
using System.Threading.Tasks;

namespace Segment.Test
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
					Analytics.Client.Statistics.Succeeded += b.batch.Count;
					return Task.CompletedTask;
				});

			Analytics.Dispose();
			Logger.Handlers += LoggingHandler;
		}

		[TestMethod]
		public void ProxyTestNetPortable()
		{
			// Set proxy address, like as "http://localhost:8888"
			var client = new Client(Constants.WRITE_KEY, new Config().SetAsync(false).SetProxy(""), _mockRequestHandler.Object);
			Analytics.Initialize(client);

			Actions.Identify(Analytics.Client);

			Assert.AreEqual(1, Analytics.Client.Statistics.Submitted);
			Assert.AreEqual(1, Analytics.Client.Statistics.Succeeded);
			Assert.AreEqual(0, Analytics.Client.Statistics.Failed);
		}

		[TestMethod]
		public void GZipTestNetPortable()
		{
			// Set proxy address, like as "http://localhost:8888"
			var client = new Client(Constants.WRITE_KEY, new Config().SetAsync(false).SetRequestCompression(true), _mockRequestHandler.Object);
			Analytics.Initialize(client);

			Actions.Identify(Analytics.Client);

			Assert.AreEqual(1, Analytics.Client.Statistics.Submitted);
			Assert.AreEqual(1, Analytics.Client.Statistics.Succeeded);
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
			Debug.WriteLine(String.Format("[ConnectionTests] [{0}] {1}", level, message));
		}
	}
}
