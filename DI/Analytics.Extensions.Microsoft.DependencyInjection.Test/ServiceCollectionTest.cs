using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;
using Segment;

namespace Analytics.Extensions.Microsoft.DependencyInjection.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var services = new ServiceCollection();
            services.AddAnalytics("writeKey");
            var sp = services.BuildServiceProvider();

            IAnalyticsClient actual;

            //act
            using (var scope = sp.CreateScope())
            {
                actual = scope.ServiceProvider.GetService<IAnalyticsClient>();
            }

            Assert.IsNotNull(actual);

            actual.Track("UserId", "EventName");
            actual.Flush();

            Assert.AreEqual(1, actual.Statistics.Submitted);
        }
    }
}