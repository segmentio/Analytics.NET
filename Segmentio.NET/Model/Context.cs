using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Segmentio.Model
{
    [DataContract]
    public class Context
    {
        [DataMember]
        internal string language;

        [DataMember]
        internal string ip;

        [DataMember]
        internal Location location;

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
        /// Location of the visitor provided as country code / optional region
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Context SetLocation(Location location)
        {
            this.location = location;
            return this;
        }
    }
}
