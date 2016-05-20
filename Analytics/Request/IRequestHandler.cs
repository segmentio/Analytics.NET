//-----------------------------------------------------------------------
// <copyright file="IRequestHandler.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment.Request
{
    using Segment.Model;

    internal interface IRequestHandler
    {
        void MakeRequest(Batch batch); 
    }
}
