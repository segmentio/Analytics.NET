using System;
using System.Collections.Generic;

namespace Segment.E2ETest
{
    public sealed class Tests : IDisposable
    {
        AxiosClient client;

        public Tests(string writeKey)
        {
            // Segment Write Key for https://app.segment.com/segment-libraries/sources/analytics_net_e2e_test/overview.
            // This source is configured to send events to a Runscope bucket used by this test.
            Analytics.Initialize(writeKey, new Config().SetAsync(false));
        }

        public void Dispose()
        {
            Analytics.Dispose();
        }

        public void Track(string userId, string eventName, IDictionary<string, object> properties)
        {
            Analytics.Client.Track(userId, eventName, properties);
        }

        public void Identify(string userId, IDictionary<string, object> traits)
        {
            Analytics.Client.Identify(userId, traits);
        }

        public void Page(string userId, string name, IDictionary<string, object> properties)
        {
            Analytics.Client.Page(userId, name, properties);
        }

        public void Group(string userId, string groupId, IDictionary<string, object> traits)
        {
            Analytics.Client.Group(userId, groupId, traits);
        }

        public void Alias(string previousId, string userId)
        {
            Analytics.Client.Alias(previousId, userId);
        }
    }
}
