using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Segment.Model;

namespace Segment.Test
{
    class End2EndTests
    {
        [SetUp]
        public void Init()
        {
            Logger.Handlers += LoggingHandler;
        }

        [TearDown]
        public void Dispose()
        {
            Analytics.Dispose();
            Logger.Handlers -= LoggingHandler;
        }

        [Test()]
        public void BatchSendTestNet35()
        {
            string szRunTests = Environment.GetEnvironmentVariable("RUN_E2E_TESTS");
            if (string.Compare(szRunTests, "True", StringComparison.CurrentCultureIgnoreCase) != 0)
                return;

            string username = Environment.GetEnvironmentVariable("WEBHOOK_AUTH_USERNAME");
            if (string.IsNullOrEmpty(username))
                username = Environment.GetEnvironmentVariable("TESTER_WEBHOOK_AUTH_USERNAME");

            string bucket = "dotnet";

            Analytics.Initialize(Constants.WRITE_KEY);

            int trials = 1000;
            for (int i = 1; i <= trials; i++)
            {
                string uid = Uid(16);

                Analytics.Client.Track("E2E Test User", "Item Purchased", new Properties()
                {
                    { "item", i }
                },
                new Options().SetAnonymousId(uid));
                Analytics.Client.Flush();

                // Check message was sent successfully
                Assert.AreEqual(i, Analytics.Client.Statistics.Submitted);
                Assert.AreEqual(i, Analytics.Client.Statistics.Succeeded);
                Assert.AreEqual(0, Analytics.Client.Statistics.Failed);

                // Retrieve server messages and see whether message is existing
                bool bMessageHandled = false;
                for (int retries = 0; retries < 30; retries++)
                {
                    var messages = GetRunscopeMessages(bucket, username);
                    int count = messages.Count(m =>
                    {
                        try
                        {
                            var body = m.ToString();
                            return JObject.Parse(body)["anonymousId"].ToString() == uid;
                        }
                        catch (System.Exception /*ex*/)
                        {
                            return false;
                        }
                    });

                    if (count > 0)
                    {
                        bMessageHandled = true;
                        break;
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }

                string szMessageDetail = string.Format("Finding message \"{0}\" on webhook (item: {1}", "Item Purchased", i);
                Assert.IsTrue(bMessageHandled, szMessageDetail);

                Thread.Sleep(500);
            }

            Analytics.Dispose();
        }

        private IList<JToken> GetRunscopeMessages(string bucket, string username)
        {
            using (WebClient httpClient = new WebClient())
            {
                // Add authentication data
                string auth = username + ":";
                string authData = Convert.ToBase64String(Encoding.Default.GetBytes(auth));
                httpClient.Headers.Add("Authorization", "Basic " + authData);

                // Send request to server and retrieve response
                // Retry 3 times for invalid response
                string url = "https://webhook-e2e.segment.com/buckets/" + bucket + "?limit=30";

                for (int i = 0; i < 3; i++)
                {
                    int backoff = 100;  // Set initial waiting time to 100ms

                    try
                    {
                        var response = Encoding.UTF8.GetString(httpClient.DownloadData(url));
                        var data = JArray.Parse(response);
                        return data.Children().ToList();
                    }
                    catch (WebException ex)
                    {
                        var response = (HttpWebResponse)ex.Response;
                        if (response != null)
                        {
                            int statusCode = (int)response.StatusCode;
                            if ((statusCode >= 500 && statusCode <= 600) || statusCode == 429)
                            {
                                // If status code is greater than 500 and less than 600, it indicates server error
                                // Error code 429 indicates rate limited.
                                // Retry uploading in these cases.
                                Thread.Sleep(backoff);
                                backoff *= 2;
                                continue;
                            }
                            else if (statusCode >= 400)
                            {
                                break;
                            }
                        }
                    }
                    catch (System.Exception /*ex*/)
                    {
                        break;
                    }
                }
            }

            return null;
        }

        private static string Uid(int len)
        {
            if (len <= 0)
                return "";

            string uid = "";
            while (len > 0)
            {
                int n = Math.Min(32, len);
                uid += Guid.NewGuid().ToString("N").Substring(0, n);
                len -= n;
            }
            return uid;
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
            Console.WriteLine(String.Format("[End2EndTests] [{0}] {1}", level, message));
        }
    }
}
