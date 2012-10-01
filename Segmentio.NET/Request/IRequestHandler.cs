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
        /// <param name="apiKey"></param>
        void Initialize(Client client, string apiKey);

        /// <summary>
        /// Processes the Segment.io base action.
        /// </summary>
        /// <param name="action"></param>
        void Process(BaseAction action);

    }
}
