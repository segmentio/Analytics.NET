using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Segment;
using Segment.Model;
using Segment.Request;

namespace Analytics.Request
{
    class FakeRequestHandler : IRequestHandler
    {
        private readonly Client _client;

        public FakeRequestHandler(Client client)
        {
            _client = client;
        }

        public async Task MakeRequest(Batch batch)
        {
            foreach (var action in batch.batch)
            {
                _client.Statistics.IncrementSucceeded();
                _client.RaiseSuccess(action);
            }
        }
    }
}
