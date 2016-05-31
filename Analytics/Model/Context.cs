using System;

namespace Segment.Model
{
    using Dict = System.Collections.Generic.Dictionary<string, object>;
    public class Context : Dict
    {
        /// <summary>
		/// Provides additional information about the context of an analytics call, 
		/// such as the visitor's ip or language.
        /// </summary>
        public Context() {
            // default the context library
            this.Add("library", new Dict {
                { "name", "Analytics.NET" },
			    { "version", Analytics.VERSION }
		    });
        }

		public new Context Add(string key, object val) {
			base.Add (key, val);
			return this;
		}
	}
}
