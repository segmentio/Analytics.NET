using System.Threading.Tasks;
using Segment.Model;

namespace Segment.Request
{
    internal interface IRequestHandler
    {
        Task MakeRequest(Batch batch);
    }
}
