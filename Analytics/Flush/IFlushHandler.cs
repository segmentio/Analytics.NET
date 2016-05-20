//-----------------------------------------------------------------------
// <copyright file="IFlushHandler.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment.Flush
{
    using System;
    using Segment.Model;

    /// <summary>
    /// A component responsible for flushing an action to the server.
    /// </summary>
    public interface IFlushHandler : IDisposable
    {
        /// <summary>
        /// Validates an action and begins the process of flushing it to the server.
        /// </summary>
        /// <param name="action">The base action.</param>
        void Process(BaseAction action);

        /// <summary>
        /// Blocks until all processing messages are flushed to the server.
        /// </summary>
        void Flush();
    }
}
