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
        #if UNITY_5_3_OR_NEWER
        System.Collections.IEnumerator MakeRequest(Batch batch); 
        #else
        void MakeRequest(Batch batch); 
        #endif
    }
}
