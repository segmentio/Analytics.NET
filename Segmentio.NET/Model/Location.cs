using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Segmentio.Model
{
    public class Location
    {
        [JsonProperty(PropertyName = "region")]
        public string Region { get; set; }

        [JsonProperty(PropertyName = "countryCode")]
        public string CountryCode { get; set; }

        internal Location() { }

        /// <summary>
        /// A user location context
        /// </summary>
        /// <param name="countryCode">2-letter country code such as "US". 
        /// Spec: ISO 3166-1 alpha-2. http://en.wikipedia.org/wiki/ISO_3166-1_alpha-2 </param>
        public Location(string countryCode)
        {
            this.CountryCode = countryCode;
        }

        /// <summary>
        /// A user location context
        /// </summary>
        /// <param name="countryCode">2-letter country code such as "US". 
        /// Spec: ISO 3166-1 alpha-2. http://en.wikipedia.org/wiki/ISO_3166-1_alpha-2 </param>
        /// <param name="region">2-Letter Region Code
        /// If State, then "PA" would work for Pennsylvania</param>
        public Location(string countryCode, string region)
            : this(countryCode)
        {
            this.Region = region;
        }

    }
}
