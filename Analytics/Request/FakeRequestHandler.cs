using System.Threading.Tasks;
using Segment.Model;

namespace Segment.Request
{
    internal class FakeRequestHandler : IRequestHandler
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
