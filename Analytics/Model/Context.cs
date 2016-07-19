//-----------------------------------------------------------------------
// <copyright file="Context.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment.Model
{
    public class Context : Dict
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Context" /> class.
        /// Provides additional information about the context of an analytics call, 
        /// such as the visitor's IP or language.
        /// </summary>
        public Context()
        {
            // default the context library
            this.Add("library", new Dict() { { "name", "Analytics.NET" }, { "version", Analytics.VERSION } });
        }

        public string IP
        {
            get { return this["ip"] as string; }
        }

        public string Language
        {
            get { return this["language"] as string; }
        }

        public new Context Add(string key, object val)
        {
            base.Add(key, val);
            return this;
        }
    }
}
