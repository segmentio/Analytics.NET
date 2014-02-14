using Segmentio;
using Segmentio.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Segmentio
{
    public static class AnalyticsHelper
    {
        public static void Initialize(string writeKey, Options options = null)
        {
            // Make sure the options are initialized
            options = options ?? new Options();
#if DEBUG
            // Run synchronously when debugging
            options = options.SetAsync(false);
#endif

            Analytics.Initialize(writeKey, options);
        }

        public static void Identify(string userId, dynamic traits)
        {
            Analytics.Client.Identify(userId, SegmentIoTraits(traits));
        }

        public static void Track(string userId, string eventName)
        {
            Track(userId, eventName, null, null, null);
        }

        public static void Track(string userId, string eventName, dynamic properties)
        {
            Track(userId, eventName, properties, null, null);
        }

        public static void Track(string userId, string eventName, dynamic properties, DateTime? timestamp)
        {
            Track(userId, eventName, properties, timestamp, null);
        }

        public static void Track(string userId, string eventName, dynamic properties, DateTime? timestamp, Context context)
        {
            Analytics.Client.Track(userId, eventName, SegmentIoProperties(properties), timestamp, context);
        }

        private static Segmentio.Model.Properties SegmentIoProperties(dynamic obj)
        {
            return PropDictionary<Segmentio.Model.Properties>(obj);
        }

        private static Segmentio.Model.Traits SegmentIoTraits(dynamic obj)
        {
            return PropDictionary<Segmentio.Model.Traits>(obj);
        }

        private static T PropDictionary<T>(dynamic obj)
            where T : Dictionary<string, object>, new()
        {
            // Null stays null
            if (obj == null)
                return null;

            // Everything else we convert to a dictionary
            var dictionary = new T();
            foreach (var propertyInfo in obj.GetType().GetProperties())
                if (propertyInfo.CanRead && propertyInfo.GetIndexParameters().Length == 0)
                    dictionary[propertyInfo.Name] = propertyInfo.GetValue(obj, null);
            return dictionary;
        }
    }
}
