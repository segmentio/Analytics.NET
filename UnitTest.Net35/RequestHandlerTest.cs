using NUnit.Framework;
using Segment.Model;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Segment.Test
{
	[TestFixture()]
	public class RequestHandlerTest
	{
		[SetUp]
		public void Init()
		{
			Analytics.Dispose();
		}

		[Test()]
		public void HeaderTestNet35()
		{
			// Arrange: Init SDK:
			Analytics.Initialize(Constants.MOCK_WRITE_KEY, new Config().SetAsync(false));

			// Act: Perform some tracking events:
			for (int i = 0; i < 10; i += 1)
			{
				Analytics.Client.Track("mockUserId", "mockEvent", new Properties(), new Options());
			}

			var flushHandler = GetPrivateFieldByReflection(Analytics.Client, "_flushHandler");
			var requestHandler = GetPrivateFieldByReflection(flushHandler, "_requestHandler");
			var httpClient = GetPrivateFieldByReflection(requestHandler, "_httpClient") as WebClient;
			var authorizationHeader = httpClient.Headers.Get("Authorization");
			
			// Assert: Verify that "Basic" appears only once in the Authorization header:
			Assert.AreEqual(1, Regex.Matches(authorizationHeader, "Basic").Count);
		}

		// Obtains a private field contained by the source object using reflection:
		private object GetPrivateFieldByReflection(object source, string fieldName) =>
			source.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(source);
	}
}
