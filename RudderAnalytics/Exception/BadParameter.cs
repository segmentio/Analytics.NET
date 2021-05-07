using System;
using System.Collections.Generic;
using System.Text;

namespace RudderStack.Exception
{
    public class APIException : System.Exception
    {
        public string Code { get; set; }

        public APIException(string code, string message) : base($"Status Code: {code}, Message: {message}")
        {
            Code = code;
        }
    }
}
