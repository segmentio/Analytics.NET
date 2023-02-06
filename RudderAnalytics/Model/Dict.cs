using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace RudderStack.Model
{
    /// <summary>
    /// An API object wrapper over Dictionary<string, object>
    /// </summary>
    public class Dict : Dictionary<string, object>
    {
        /// <summary>
        /// Adds the key/val pair to the Props API dictionary. Used for chaining API purposes.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="val">Value</param>
        /// <returns>An instance of the Props for chaining</returns>
        public new Dict Add(string key, object val)
        {
            base.Add(key, val);
            return this;
        }
    }
}
