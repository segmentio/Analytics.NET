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


				analytics.Track("user", "Test Event", new Properties () {
					{"Test Property",1969009135},
					{"Test Property 2",499343901}
				}, DateTime.Parse("2013-09-16T16:52:07.1694552Z"));

				analytics.Flush ();
			}
		}
	}
}
