using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Segment.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddAnalytics(this IServiceCollection services, string writeKey, Config config = null)
        {
            var client = new Client(writeKey, config);
            services.AddSingleton<IAnalyticsClient>(client);
        }
    }
}
