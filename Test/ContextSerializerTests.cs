using System;
using System.Collections.Generic;
using Segmentio.Request;
using Newtonsoft.Json;
using Segmentio.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Analytics.Test
{
	/// <summary>
	/// Without a special json.net converter, we were getting json that omitted all the built-in properties of Context (ip, language, providers). This tests out that converter, ContextSerializer
	/// </summary>
	[TestClass]
	public class ContextSerializerTests
	{
		[TestMethod]
		public void DemonstrateWhyContextSerializerIsNeeded()
		{
			var identify = new Identify("99",
				 new Traits() { },
						DateTime.UtcNow,
						new Context()
							.SetIp("12.212.12.49"));

			var settings = new JsonSerializerSettings
			{
				//notice, we are failing to specify a special converter
			};
			string json = JsonConvert.SerializeObject(identify, settings);
			Assert.IsFalse(json.Contains("12.212.12.49"));
		}

		[TestMethod]
		public void SerializationOfIdentifyIncludesContextWithIPAddress()
		{
			var identify = new Identify("99",
				 new Traits() {},
                        DateTime.UtcNow,
                        new Context()
                            .SetIp("12.212.12.49"));

			var settings = new JsonSerializerSettings
			{
				Converters = new List<JsonConverter> { new ContextSerializer() }
			};
			string json = JsonConvert.SerializeObject(identify,settings);
			Assert.IsTrue(json.Contains("\"ip\":\"12.212.12.49\""));
		}

		[TestMethod]
		public void SerializationOfIdentifyIncludesContextWithLanguage()
		{
			var identify = new Identify("99",
				 new Traits() { },
						DateTime.UtcNow,
						new Context()
							.SetLanguage("fr"));

			var settings = new JsonSerializerSettings
			{
				Converters = new List<JsonConverter> { new ContextSerializer() }
			};
			string json = JsonConvert.SerializeObject(identify, settings);
			Assert.IsTrue(json.Contains("\"language\":\"fr\""));
		}

		[TestMethod]
		public void SerializationOfIdentifyIncludesContextWithProviders()
		{
			var identify = new Identify("99",
				 new Traits() { },
						DateTime.UtcNow,
						new Context()
							.SetProviders(new Providers() {
                                { "all", false },
                                { "Mixpanel", true },
                                { "Salesforce", true }}));

			var settings = new JsonSerializerSettings
			{
				Converters = new List<JsonConverter> { new ContextSerializer() }
			};
			string json = JsonConvert.SerializeObject(identify, settings);
			Assert.IsTrue(json.Contains("\"providers\""));
		}



        [TestMethod]
        public void SerializationOfTrackIncludesContextWithIPAddress()
        {
            var track = new Track(
                "99",
                "Ran .NET test",
                new Properties() {
                        { "Success", true },
                        { "When", DateTime.Now }
                    },
                DateTime.Now,
                        new Context()
                            .SetIp("12.212.12.49")
                );

            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new ContextSerializer() }
            };
            string json = JsonConvert.SerializeObject(track, settings);
            Assert.IsTrue(json.Contains("\"ip\":\"12.212.12.49\""));
        }

        [TestMethod]
        public void SerializationOfTrackIncludesContextWithLanguage()
        {
            var track = new Track(
                "99",
                "Ran .NET test",
                new Properties() {
                        { "Success", true },
                        { "When", DateTime.Now }
                    },
                DateTime.Now,
                        new Context()
                            .SetLanguage("fr")
                );

            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new ContextSerializer() }
            };
            string json = JsonConvert.SerializeObject(track, settings);
            Assert.IsTrue(json.Contains("\"language\":\"fr\""));
        }

        [TestMethod]
        public void SerializationOfTrackIncludesContextWithProviders()
        {
            var track = new Track(
                "99", 
                "Ran .NET test", 
                new Properties() {
                        { "Success", true },
                        { "When", DateTime.Now }
                    }, 
                DateTime.Now,
                        new Context()
                            .SetProviders(new Providers() {
                                    { "all", false },
                                    { "Mixpanel", true },
                                    { "Salesforce", true }
                                })
                );

            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new ContextSerializer() }
            };
            string json = JsonConvert.SerializeObject(track, settings);
            Assert.IsTrue(json.Contains("\"providers\""));
        }
	}

	
}
