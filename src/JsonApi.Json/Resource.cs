using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JsonApi
{
    public class Resource
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("attributes")]
        public Dictionary<string, object> Attributes { get; set; }
    }
}