#if NETSTANDARD2_0 || NET461 || NET5_0
using Microsoft.Extensions.DependencyInjection;
#endif

namespace Segment.Extensions
{
    public static class ServiceCollectionExtension
    {
#if NETSTANDARD2_0 || NET461 || NET5_0
        public static void AddAnalytics(this IServiceCollection services, string writeKey, Config config = null)
        {
            Config configuration;

            if(config == null)
                configuration = new Config();
            else
                configuration = config;

            var client = new Client(writeKey, configuration);
            services.AddSingleton<IAnalyticsClient>(client);
        }
#endif
    }
}
