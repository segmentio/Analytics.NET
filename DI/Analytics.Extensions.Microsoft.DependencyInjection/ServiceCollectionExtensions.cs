using System;
using RudderStack;
using Microsoft.Extensions.DependencyInjection;

namespace RudderStack
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAnalytics(this IServiceCollection services, string writeKey) =>
            AddAnalytics(services, writeKey, null);

        public static IServiceCollection AddAnalytics(this IServiceCollection services, string writeKey,
            Action<RudderConfig> configuration)
        {
            var config = new RudderConfig();
            configuration?.Invoke(config);

            var client = new RudderClient(writeKey, config);
            services.AddSingleton<IRudderAnalyticsClient>(client);
            return services;
        }
    }
}
