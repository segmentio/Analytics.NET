
using Newtonsoft.Json;

namespace Segment.Model
{
	public class Screen : BaseAction
    {
		[JsonProperty(PropertyName = "name")]
        private string Name { get; set; }

		[JsonProperty(PropertyName = "category")]
		private string Category { get; set; }

        [JsonProperty(PropertyName = "properties")]
        private Properties Properties { get; set; }

		internal Screen(string userId, string name, string category, Properties properties, Options options) : base("screen", userId, options)
		{
			this.Name = name;
			this.Category = category;
            this.Properties = properties ?? new Properties();
        }
    }
}
