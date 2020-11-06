using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Converters;

namespace JsonApi
{
    public class JsonApiObject
    {
        [JsonPropertyName("version")]
        [JsonConverter(typeof(JsonApiVersionConverter))]
        public Version Version { get; set; }

        [JsonPropertyName("meta")]
        public Dictionary<string, JsonElement> Meta { get; set; }
    }
}
