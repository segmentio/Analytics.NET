using System;
using System.Collections.Generic;
using System.Text;

using Segmentio.Model;

namespace Segmentio.Request
{
    internal interface IRequestHandler
    {
        /// <summary>
        /// Initialize a request handler with an API KEY.
        /// </summary>
        /// <param name="secret"></param>
        void Initialize(Client client, string secret);

        /// <summary>
        /// Processes the Segment.io base action.
        /// </summary>
        /// <param name="action"></param>
        void Process(BaseAction action);

        /// <summary>
        /// Triggers a Flush
        /// </summary>
        void Flush();
    }
}
