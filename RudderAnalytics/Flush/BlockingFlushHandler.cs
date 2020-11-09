using System.Collections.Generic;
using System.Threading.Tasks;
using RudderStack.Model;
using RudderStack.Request;

namespace RudderStack.Flush
{
    internal class BlockingFlushHandler : IAsyncFlushHandler
    {
        /// <summary>
        /// Creates a series of actions into a batch that we can send to the server
        /// </summary>
        private IBatchFactory _batchFactory;
        /// <summary>
        /// Performs the actual HTTP request to our server
        /// </summary>
        private IRequestHandler _requestHandler;

        internal BlockingFlushHandler(IBatchFactory batchFactory,
                                 IRequestHandler requestHandler)
        {

            this._batchFactory = batchFactory;
            this._requestHandler = requestHandler;
        }

        public async Task Process(BaseAction action)
        {
            Batch batch = _batchFactory.Create(new List<BaseAction>() { action });
            await _requestHandler.MakeRequest(batch).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns immediately since the blocking flush handler does not queue
        /// </summary>
        public void Flush()
        {
            // do nothing
        }

        public async Task FlushAsync()
        {
            // do nothing
        }

        /// <summary>
        /// Does nothing, as nothing needs to be disposed here
        /// </summary>
        public void Dispose()
        {
            // do nothing
        }

    }
}
