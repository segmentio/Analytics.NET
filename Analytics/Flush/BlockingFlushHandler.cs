//-----------------------------------------------------------------------
// <copyright file="BlockingFlushHandler.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment.Flush
{
    using System.Collections.Generic;
    using Segment.Model;
    using Segment.Request;

    internal class BlockingFlushHandler : IFlushHandler
    {
        /// <summary>
        /// Creates a series of actions into a batch that we can send to the server.
        /// </summary>
        private IBatchFactory batchFactory;

        /// <summary>
        /// Performs the actual HTTP request to our server.
        /// </summary>
        private IRequestHandler requestHandler;
        
        internal BlockingFlushHandler(IBatchFactory batchFactory, IRequestHandler requestHandler)
        {
            this.batchFactory = batchFactory;
            this.requestHandler = requestHandler;
        }
        
        public void Process(BaseAction action)
        {
            Batch batch = this.batchFactory.Create(new List<BaseAction>() { action });
            this.requestHandler.MakeRequest(batch);
        }
        
        /// <summary>
        /// Returns immediately since the blocking flush handler does not queue.
        /// </summary>
        public void Flush() 
        {
            // do nothing
        }
        
        /// <summary>
        /// Does nothing, as nothing needs to be disposed here.
        /// </summary>
        public void Dispose() 
        {
            // do nothing
        }
    }
}
