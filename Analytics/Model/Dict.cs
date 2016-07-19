//-----------------------------------------------------------------------
// <copyright file="Dict.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// An API object wrapper over a string/object Dictionary.
    /// </summary>
    public class Dict : Dictionary<string, object>
    {
        /// <summary>
        /// Adds the key/val pair to the Props API dictionary. Used for chaining API purposes.
        /// </summary>
        /// <param name="key">The dictionary key.</param>
        /// <param name="val">The dictionary value.</param>
        /// <returns>An instance of the Props for chaining.</returns>
        public new Dict Add(string key, object val) 
        {
            base.Add(key, val);
            return this;
        }
    }
}
