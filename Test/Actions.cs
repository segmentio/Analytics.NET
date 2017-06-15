using System;
using System.Threading.Tasks;
using Segment.Model;

namespace Segment.Test
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

		public static Options Options()
		{
			return new Options()
				.SetTimestamp(DateTime.Now)
				.SetAnonymousId(Guid.NewGuid().ToString())
				.SetIntegration("all", false)
				.SetIntegration("Mixpanel", true)
				.SetIntegration("Salesforce", true)
				.SetContext(new Context()
					.Add("ip", "12.212.12.49")
					.Add("language", "en-us")
			);
		}

		public static async Task Identify(Client client)
		{
			await client.Identify("user", Traits(), Options());
			Analytics.Client.Flush();
		}

		public static async Task Group(Client client)
		{
			await client.Group("user", "group", Traits(), Options());
			Analytics.Client.Flush();
		}

		public static async Task Track(Client client)
		{
			await client.Track("user", "Ran .NET test", Properties(), Options());
		}

		public static async Task Alias(Client client)
		{
			await client.Alias("previousId", "to");
		}

		public static async Task Page(Client client)
		{
			await client.Page("user", "name", "category", Properties(), Options());
		}

		public static async Task Screen(Client client)
		{
			await client.Screen("user", "name", "category", Properties(), Options());
		}

		public static async Task Random(Client client)
		{
			switch (random.Next(0, 6))
			{
				case 0:
					await Identify(client);
					return;
				case 1:
					await Track(client);
					return;
				case 2:
					await Alias(client);
					return;
				case 3:
					await Group(client);
					return;
				case 4:
					await Page(client);
					return;
				case 5:
					await Screen(client);
					return;
			}
		}
	}
}
