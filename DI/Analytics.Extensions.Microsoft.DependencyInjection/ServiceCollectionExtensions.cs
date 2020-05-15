using System;
using Microsoft.Extensions.DependencyInjection;

namespace Segment
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAnalytics(this IServiceCollection services, string writeKey) =>
            AddAnalytics(services, writeKey, null);

        public static IServiceCollection AddAnalytics(this IServiceCollection services, string writeKey,
            Action<Config> configuration)
        {
            var config = new Config();
            configuration?.Invoke(config);

            var client = new Client(writeKey, config);
            services.AddSingleton<IAnalyticsClient>(client);
            return services;
        }
    }
}
