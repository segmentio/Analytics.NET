using RudderStack;
using RudderStack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Net50
{
    public class Actions
    {
        private static Random random = new Random();

        public static Properties Properties()
        {
            return new Properties() {
                { "Success", true },
                { "When", DateTime.Now }
            };
        }

        public static Traits Traits()
        {
            return new Traits() {
                { "Subscription Plan", "Free" },
                { "Friends", 30 },
                { "Joined", DateTime.Now },
                { "Cool", true },
                { "Company", new Dict () { { "name", "Initech, Inc " } } },
                { "Revenue", 40.32 },
                { "Don't Submit This, Kids", new UnauthorizedAccessException () }
            };
        }

        public static RudderOptions Options()
        {
            return new RudderOptions()
                .SetTimestamp(DateTime.Now)
                .SetAnonymousId(Guid.NewGuid().ToString())
                .SetIntegration("all", false)
                .SetIntegration("Mixpanel", true)
                .SetIntegration("Salesforce", true)
                .SetContext(new RudderContext()
                    .Add("ip", "12.212.12.49")
                    .Add("language", "en-us")
            );
        }

        public static void Identify(RudderClient client)
        {
            client.Identify("user", Traits(), Options());
            RudderAnalytics.Client.Flush();
        }

        public static void Group(RudderClient client)
        {
            client.Group("user", "group", Traits(), Options());
            RudderAnalytics.Client.Flush();
        }

        public static void Track(RudderClient client)
        {
            client.Track("user", "Ran .NET test", Properties(), Options());
        }

        public static void Alias(RudderClient client)
        {
            client.Alias("previousId", "to");
        }

        public static void Page(RudderClient client)
        {
            client.Page("user", "name", "category", Properties(), Options());
        }

        public static void Screen(RudderClient client)
        {
            client.Screen("user", "name", "category", Properties(), Options());
        }

        public static void Random(RudderClient client)
        {
            switch (random.Next(0, 6))
            {
                case 0:
                    Identify(client);
                    return;
                case 1:
                    Track(client);
                    return;
                case 2:
                    Alias(client);
                    return;
                case 3:
                    Group(client);
                    return;
                case 4:
                    Page(client);
                    return;
                case 5:
                    Screen(client);
                    return;
            }
        }
    }
}
