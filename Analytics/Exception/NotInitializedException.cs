
namespace Segment.Exception
{
    public class NotInitializedException : System.Exception
    {
        public NotInitializedException() : base("Please initialize Segment.io first before using.")
        {
        }
    }
}
