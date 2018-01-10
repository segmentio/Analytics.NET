using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Segment.E2ETest
{
    class AxiosClient
    {
        HttpClient client = null;
        int RetryCount = 1;

        public AxiosClient(string BaseAddress, int timeout, string authorization = "")
        {
            client = new HttpClient()
            {
                BaseAddress = new Uri("https://api.runscope.com"),
                Timeout = new TimeSpan(0, 0, 0, 0, timeout),
            };

            // Set authorization header
            if (!string.IsNullOrEmpty(authorization))
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + authorization);

            // Set retry count
            this.RetryCount = 1;
        }

        public void SetRetryCount(int retries)
        {
            this.RetryCount = retries < 1 ? 1 : retries;
        }

        public async Task<HttpResponseMessage> Get(string url)
        {
            for (int i = 0; i < this.RetryCount; i++)
            {
                try
                {
                    return await this.client.GetAsync(url);
                }
                catch (System.Exception /*ex*/)
                {
                }
            }
            return null;
        }
    }
}
