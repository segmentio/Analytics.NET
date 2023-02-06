using System.Threading.Tasks;
using RudderStack.Model;

namespace RudderStack.Request
{
    internal interface IRequestHandler
    {
        Task MakeRequest(Batch batch);
    }
}
