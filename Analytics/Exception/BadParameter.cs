
namespace Segment.Exception
{
    public class APIException : System.Exception
    {
        public string Code { get; set; }

        public string message { get; set; }

        public APIException(string code, string message) : base(message)
        {
            this.Code = code;
            this.message = message;
        }
    }
}
