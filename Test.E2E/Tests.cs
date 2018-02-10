using System;
using System.Collections.Generic;
using System.Text;
using Segment;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Segment.E2ETest
{
    public sealed class Tests : IDisposable
    {
        AxiosClient client;
        string id;

        /// <summary>
        /// Generate unique id
        /// </summary>
        /// <param name="len">Length of id string</param>
        /// <returns></returns>
        string Uid(int len)
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

        public Tests()
        {
            this.id = Uid(16);

            // Segment Write Key for https://segment.com/segment-libraries/sources/analytics_node_e2e_test/overview.
            // This source is configured to send events to a Runscope bucket used by this test.
            Analytics.Initialize(Constants.WRITE_KEY);
            Analytics.Client.Track("prateek", "Item Purchased", new Model.Options().SetAnonymousId(this.id));
            Analytics.Client.Flush();

            // Give some time for events to be delivered from the API to destinations.
            Task.Delay(5 * 1000).Wait();   // 5 seconds.
        }

        public void Dispose()
        {
            Analytics.Dispose();
        }

        [Fact()]
        public void Test()
        {
            var token = Environment.GetEnvironmentVariable("RUNSCOPE_TOKEN");

            this.client = new AxiosClient("https://api.runscope.com", 10 * 1000, token);
            this.client.SetRetryCount(3);

            for (int i = 0; i < 5; i++)
            {
                // Runscope Bucket for https://www.runscope.com/stream/pwb8mcmfks0f.
                var messageResponse = client.Get("buckets/" + Constants.RUNSCOPE_BUCKET + "/messages?count=20").Result;
                Assert.True(messageResponse.StatusCode == System.Net.HttpStatusCode.OK);

                var content = messageResponse.Content.ReadAsStringAsync().Result;
                var data = JObject.Parse(content);

                var messages = Task.WhenAll(data["data"].Children().Select(async item =>
                {
                    var response = client.Get("buckets/" + Constants.RUNSCOPE_BUCKET + "/messages/" + item["uuid"]).Result;
                    Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);

                    var content2 = response.Content.ReadAsStringAsync().Result;
                    return JObject.Parse(content2)["data"]["request"]["body"];
                })).Result;

                var count = messages.Count(m =>
                {
                    try
                    {
                        var body = m.ToString();
                        return JObject.Parse(body)["anonymousId"].ToString() == this.id;
                    }
                    catch (System.Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        return false;
                    }
                });

                if (count > 0)
                    return;

                Task.Delay(5000).Wait();
            }

            Assert.True(false);
        }
    }
}