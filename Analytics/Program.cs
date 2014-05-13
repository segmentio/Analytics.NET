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

				analytics.Track ("user123", "Engaged a bear", new Properties () {
					{ "bearWeight", 865 }
				});

				analytics.Track ("user123", "Engaged a bear", new Options ()
					.SetContext (new Context () {
						{ "app", new Dict() {
							{ "name", "test" } 
						}}
				}));

				analytics.Track ("user123", "Is Not Alive", new Properties () {
					{ "why", "bear attack" }
				}, new Options()
					.SetIntegration("Salesforce", true)
				);

				analytics.Track ("user123", "Is Not Alive", new Properties () {
					{ "why", "bear attack" }
				}, new Options()
					.SetIntegration("Salesforce", true)
					.SetAnonymousId("cookie-id")
					.SetContext(new Context()
						.Add("ip", "192.144.23.2"))
				);

				analytics.Flush ();
			}
		}
	}
}
