using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Segmentio.Model
{
    public class Context : Props
    {
        [JsonProperty(PropertyName = "language")]
        internal string language;

        [JsonProperty(PropertyName = "ip")]
        internal string ip;

		[JsonProperty(PropertyName = "providers")]
		internal Providers providers;

        /// <summary>
        /// Provides additional information about a 
        /// user during identify.
        /// </summary>
        public Context() { }

        /// <summary>
        /// IETF Language Tag, such as "en-us"
        /// http://en.wikipedia.org/wiki/IETF_language_tag
        /// </summary>
        /// <param name="language">Examples: "en-us" for America English or "pt-BR" for Brazilian Portuguese</param>
        /// <returns></returns>
        public Context SetLanguage(string language)
        {
            this.language = language;
            return this;
        }

        /// <summary>
        /// IP-v4 Address such as "12.212.12.49"
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public Context SetIp(string ip)
        {
            this.ip = ip;
            return this;
        }

		/// <summary>
		/// Sets the enabled providers for this call.
		/// </summary>
		/// <param name="providers">The container object for provider info</param>
		/// <returns>Context for chaining</returns>
		public Context SetProviders(Providers providers)
		{
			this.providers = providers;
			return this;
		}
    }
}
