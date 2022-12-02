using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Segment;
using Segment.Extensions;

namespace Test.Net50.Extensions
{
    [TestFixture]
    public class ServiceCollectionExtensionTests
    {
        [Test]
        public void TestInicializationOfClientWithServicesCollection()
        {
            var writeKey = "writeKey";
            var services = new ServiceCollection();
            services.AddAnalytics(writeKey);

            var provider = services.BuildServiceProvider();
            var analyticsInstance = provider.GetRequiredService(typeof(IAnalyticsClient));

            Assert.IsNotNull(analyticsInstance);

            var client = analyticsInstance as Client;
            Assert.AreEqual("writeKey", client.WriteKey);
        }

        [Test]
        public void TestInicializationOfClientWithServicesCollectionAndConfig()
        {
            var writeKey = "writeKey";
            var threadCount = 10;
            var config = new Config { Threads = threadCount };

            var services = new ServiceCollection();
            services.AddAnalytics(writeKey, config);

            var provider = services.BuildServiceProvider();
            var analyticsInstance = provider.GetRequiredService(typeof(IAnalyticsClient));

            Assert.IsNotNull(analyticsInstance);

            var client = analyticsInstance as Client;
            Assert.AreEqual("writeKey", client.WriteKey);
            Assert.AreEqual(threadCount, client.Config.Threads);
        }
    }
}
