using System;
using System.Collections.Generic;
using System.Text;

using Segment.Model;

namespace Segment.Request
{
    internal interface IRequestHandler
    {
		void MakeRequest(Batch batch); 
    }
}
