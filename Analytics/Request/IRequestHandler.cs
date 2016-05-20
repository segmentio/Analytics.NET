
using Segment.Model;

namespace Segment.Request
{
    internal interface IRequestHandler
    {
		void MakeRequest(Batch batch); 
    }
}
