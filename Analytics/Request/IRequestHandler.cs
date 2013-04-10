using System;
using System.Collections.Generic;
using System.Text;

using Segmentio.Model;

namespace Segmentio.Request
{
    internal interface IRequestHandler
    {
		void MakeRequest(Batch batch); 
    }
}
