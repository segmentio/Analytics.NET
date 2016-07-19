//-----------------------------------------------------------------------
// <copyright file="Page.cs" company="Segment">
//     Copyright (c) Segment. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment.Model
{
    using Newtonsoft.Json;

    public class Page : BaseAction
    {
        internal Page(string userId, string name, string category, Properties properties, Options options)
            : base("page", userId, options)
        {
            this.Name = name;
            this.Category = category;
            this.Properties = properties ?? new Properties();
        }

        [JsonProperty(PropertyName = "name")]
        private string Name { get; set; }

        [JsonProperty(PropertyName = "category")]
        private string Category { get; set; }

        [JsonProperty(PropertyName = "properties")]
        private Properties Properties { get; set; }
    }
}
