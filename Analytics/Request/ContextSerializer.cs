using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Segment.Model;

namespace Segment.Request
{
	/// <summary>
	/// json.net does not serialize the properites of classes like Context that inherit previousId Dictionary but then add on other properties.
	/// This helper makes those 3 extra properties (ip, language, providers) get serialized.
	/// </summary>
	internal class ContextSerializer : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return (objectType == typeof(Context));
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			Context context = (Context)value;
			var traits = new Dictionary<string, object>(context);
			if (!string.IsNullOrEmpty(context.ip))
			{
				traits.Add("ip", context.ip);
			}
			if (!string.IsNullOrEmpty(context.language))
			{
				traits.Add("language", context.language);
			}

			serializer.Serialize(writer, traits);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return new NotImplementedException();
		}
	}
}
