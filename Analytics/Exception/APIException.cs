//-----------------------------------------------------------------------
// <copyright file="APIException.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment.Exception
{
    public class APIException : System.Exception
    {
        public APIException(string code, string message) : base(message)
        {
            this.Code = code;
        }

        public string Code { get; private set; }
    }
}
