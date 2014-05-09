using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Segmentio;
using Segmentio.Model;

namespace Segment.Test
{
	public class Actions
	{
		private static Random random = new Random();

		private static string userId = "test";

		public static void Identify(Client client)
		{
			client.Identify(userId, new Traits() {
				{ "Subscription Plan", "Free" },
				{ "Friends", 30 },
				{ "Joined", DateTime.Now },
				{ "Cool", true },
				{ "Company", new Props() { { "name", "Initech, Inc " } } },
				{ "Revenue", 40.32 },
				{ "Don't Submit This, Kids", new UnauthorizedAccessException() } },
				new DateTime(),
				new Context()
				.SetIp("12.212.12.49")
				.SetLanguage("en-us")
				.SetProviders(new Providers() {
					{ "all", false },
					{ "Mixpanel", true },
					{ "Salesforce", true }
				})
			);
		}

		public static void Track(Client client)
		{
			client.Track(userId, "Ran .NET test", new Properties() {
				{ "Success", true },
				{ "When", DateTime.Now }
			}, DateTime.Now);
		}

		public static void Alias(Client client)
		{
			client.Alias("anonymous", userId);
		}

		public static void Random(Client client)
		{
			switch (random.Next(0, 3))
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
			}

		}

	}
}