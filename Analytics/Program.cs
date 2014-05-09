using System;
using Segment;
using Segment.Model;

namespace Test2
{
	class MainClass
	{
		private static readonly Random _rng = new Random();
		private const  string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		private static string RandomString(int size)
		{
			char[] buffer = new char[size];

			for (int i = 0; i < size; i++)
			{
				buffer[i] = _chars[_rng.Next(_chars.Length)];
			}
			return new string(buffer);
		}

		public static void Main (string[] args)
		{

			Analytics.Initialize ("testwriteKey");

			/*
			string email = RandomString (10) + "@gmail.com";
			DateTime date = new DateTime (2013, 9, 10, 14, 32, 12);

			Properties props = new Properties();
			props.Add ("woo", 1);

			Traits traits = new Traits ();
			traits.Add ("trait1", "woo");

			Console.WriteLine (email);

			Analytics.Client.Identify (email, traits);

			Analytics.Client.Track (email, "Test Event", props, date);
			*/

			string userId = "85ab9792b1344b0b899acb406f048146";
	
			Traits traits = new Traits ();
			Analytics.Client.Identify (userId, traits);

			Analytics.Client.Track(userId, "Test Event", new Properties () {
				{"Test Property",1969009135},
				{"Test Property 2",499343901}
			}, DateTime.Parse("2013-09-16T16:52:07.1694552Z"));

			Analytics.Client.Flush ();
		}
	}
}
