//-----------------------------------------------------------------------
// <copyright file="Defaults.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment
{
    using System;

    public class Defaults
    {
        public static readonly string Host = "https://api.segment.io";
        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);
        public static readonly int MaxQueueCapacity = 10000;
        public static readonly bool Async = true;
    }
}
