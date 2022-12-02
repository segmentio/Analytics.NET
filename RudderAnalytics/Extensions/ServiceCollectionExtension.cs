#if NETSTANDARD2_0 || NET461 || NET5_0
using Microsoft.Extensions.DependencyInjection;
#endif

namespace RudderStack.Extensions
{
    public static class ServiceCollectionExtension
    {
#if NETSTANDARD2_0 || NET461 || NET5_0
        public static void AddAnalytics(this IServiceCollection services, string writeKey, RudderConfig config = null)
        {
            RudderConfig configuration;

            if(config == null)
                configuration = new RudderConfig();
            else
                configuration = config;

            var client = new RudderClient(writeKey, configuration);
            services.AddSingleton<IRudderAnalyticsClient>(client);
        }
#endif
    }
}
