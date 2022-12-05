using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RudderStack;
using RudderStack.Extensions;

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
            var analyticsInstance = provider.GetRequiredService(typeof(IRudderAnalyticsClient));

            Assert.IsNotNull(analyticsInstance);

            var client = analyticsInstance as RudderClient;
            Assert.AreEqual("writeKey", client.WriteKey);
        }

        [Test]
        public void TestInicializationOfClientWithServicesCollectionAndConfig()
        {
            var writeKey = "writeKey";
            var threadCount = 10;
            var config = new RudderConfig { Threads = threadCount };

            var services = new ServiceCollection();
            services.AddAnalytics(writeKey, config);

            var provider = services.BuildServiceProvider();
            var analyticsInstance = provider.GetRequiredService(typeof(IRudderAnalyticsClient));

            Assert.IsNotNull(analyticsInstance);

            var client = analyticsInstance as RudderClient;
            Assert.AreEqual("writeKey", client.WriteKey);
            Assert.AreEqual(threadCount, client.Config.Threads);
        }
    }
}
