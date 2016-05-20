
using System.Collections.Generic;
using Segment.Request;
using Segment.Model;

namespace Segment.Flush
{
    internal class BlockingFlushHandler : IFlushHandler
    {
        /// <summary>
        /// Creates a series of actions into a batch that we can send to the server
        /// </summary>
        private IBatchFactory _batchFactory;

        /// <summary>
        /// Performs the actual HTTP request to our server
        /// </summary>
        private IRequestHandler _requestHandler;

        internal BlockingFlushHandler(IBatchFactory batchFactory, IRequestHandler requestHandler)
        {
            this._batchFactory = batchFactory;
            this._requestHandler = requestHandler;
        }

        public void Process(BaseAction action)
        {
            Batch batch = _batchFactory.Create(new List<BaseAction>() { action });
            _requestHandler.MakeRequest(batch);
        }

        /// <summary>
        /// Returns immediately since the blocking flush handler does not queue
        /// </summary>
        public void Flush()
        {
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

