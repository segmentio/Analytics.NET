using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Segment;
using Segment.Model;

namespace Segment.Test
{
	public class Actions
	{
		private static Random random = new Random();

		public static Properties Properties () 
		{
			return new Properties () {
				{ "Success", true },
				{ "When", DateTime.Now }
			};
		}

		public static Traits Traits () 
		{
			return new Traits () {
				{ "Subscription Plan", "Free" },
				{ "Friends", 30 },
				{ "Joined", DateTime.Now },
				{ "Cool", true },
				{ "Company", new Dict () { { "name", "Initech, Inc " } } },
				{ "Revenue", 40.32 },
				{ "Don't Submit This, Kids", new UnauthorizedAccessException () }
			};
		}

		public static Options Options ()
		{
			return new Options () 
				.SetTimestamp(DateTime.Now)
				.SetIntegration ("all", false)
				.SetIntegration ("Mixpanel", true)
				.SetIntegration ("Salesforce", true)
				.SetContext (new Context ()
					.Add ("ip", "12.212.12.49")
					.Add ("language", "en-us")
			);
		}
			
		public static void Identify(Client client)
		{
			client.Identify("user", Traits(), Options());
			Analytics.Client.Flush();
		}

		public static void Group(Client client)
		{
			client.Group("user", "group", Traits(), Options());
			Analytics.Client.Flush();
		}

		public static void Track(Client client)
		{
			client.Track("user", "Ran .NET test", Properties(), Options());
		}

		public static void Alias(Client client)
		{
			client.Alias("previousId", "to");
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