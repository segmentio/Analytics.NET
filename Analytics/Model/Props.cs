using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Segment.Model
{
	/// <summary>
	/// An API object wrapper over Dictionary<string, object>
	/// </summary>
    public class Props : Dictionary<string, object>
    {
		/// <summary>
		/// Adds the key/val pair to the Props API dictionary. Used for chaining API purposes.
		/// </summary>
		/// <param name="key">Key</param>
		/// <param name="val">Value</param>
		/// <returns>An instance of the Props for chaining</returns>
		public Props Put(string key, object val) 
		{
			this.Add (key, val);
			return this;
		}
    }
}
