using System;
using Segment;
using Segment.Model;

namespace Segment
{
	class MainClass
	{

		public static void Main (string[] args)
		{
			using (Client analytics = new Client("r7bxis28wy")) 
			{
				analytics.Identify ("user123", new Traits () {
					{ "hey", 123 }
				});

				analytics.Track ("user123", "Ran away");

				analytics.Track ("user123", "Came back", DateTime.Now);

				analytics.Track ("user123", "Engaged a bear", new Properties () {
					{ "bearWeight", 865 }
				});

				analytics.Track ("user123", "Is Not Alive", new Properties () {
					{ "why", "bear attack" }
				}, new Options()
					.Integration("Salesforce", true)
				);

				analytics.Track ("user123", "Is Not Alive", new Properties () {
					{ "why", "bear attack" }
				}, new Options()
					.Integration("Salesforce", true)
					.SetAnonymousId("cookie-id")
					.SetContext(new Context()
						.SetIp("192.144.23.2"))
				);

				analytics.Flush ();
			}
		}
	}
}
