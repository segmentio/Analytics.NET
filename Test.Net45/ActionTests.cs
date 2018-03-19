﻿using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Segment.Test
{
	[TestFixture()]
	public class ActionTests
	{

		[SetUp]
		public void Init()
		{
			Analytics.Dispose();
			Logger.Handlers += LoggingHandler;
			Analytics.Initialize(Constants.WRITE_KEY, new Config().SetAsync(false));
		}

		[Test ()]
		public void IdentifyTestNet45()
		{
			Actions.Identify(Analytics.Client);
			FlushAndCheck(1);
		}

		[Test()]
		public void IdentifyWithCustomOptionsTestNet45()
		{
			var traits = new Model.Traits() {
				{ "email", "friends@segment.com" }
			};
			var options = new Model.Options()
				.SetIntegration("Vero", new Model.Dict() {
					{
						"tags", new Model.Dict() {
							{ "id", "235FAG" },
							{ "action", "add" },
							{ "values", new string[] {"warriors", "giants", "niners"} }
						}
					}
				});

			Actions.Identify(Analytics.Client, traits, options);
			FlushAndCheck(1);
		}

		[Test()]
		public void TrackTestNet45()
		{
			Actions.Track(Analytics.Client);
			FlushAndCheck(1);
		}

		[Test()]
		public void AliasTestNet45()
		{
			Actions.Alias(Analytics.Client);
			FlushAndCheck(1);
		}

		[Test()]
		public void GroupTestNet45()
		{
			Actions.Group(Analytics.Client);
			FlushAndCheck(1);
		}

		[Test()]
		public void PageTestNet45()
		{
			Actions.Page(Analytics.Client);
			FlushAndCheck(1);
		}

		[Test()]
		public void ScreenTestNet45()
		{
			Actions.Screen(Analytics.Client);
			FlushAndCheck(1);
		}

		private void FlushAndCheck(int messages)
		{
			Analytics.Client.Flush();
			Assert.AreEqual(messages, Analytics.Client.Statistics.Submitted);
			Assert.AreEqual(messages, Analytics.Client.Statistics.Succeeded);
			Assert.AreEqual(0, Analytics.Client.Statistics.Failed);
		}

		static void LoggingHandler(Logger.Level level, string message, IDictionary<string, object> args)
		{
			if (args != null)
			{
				foreach (string key in args.Keys)
				{
					message += String.Format(" {0}: {1},", "" + key, "" + args[key]);
				}
			}
			Console.WriteLine(String.Format("[ActionTests] [{0}] {1}", level, message));
		}
	}


}

