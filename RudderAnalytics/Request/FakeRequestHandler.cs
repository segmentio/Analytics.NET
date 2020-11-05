using System.Threading.Tasks;
using RudderStack.Model;

namespace RudderStack.Request
{
    internal class FakeRequestHandler : IRequestHandler
    {
        private readonly RudderClient _client;

        public FakeRequestHandler(RudderClient client)
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
