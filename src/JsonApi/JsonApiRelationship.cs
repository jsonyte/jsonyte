using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi
{
    public class JsonApiRelationship
    {
        [JsonPropertyName("links")]
        public JsonApiRelationshipLinks Links { get; set; }

        [JsonPropertyName("data")]
        public JsonApiResourceIdentifier[] Data { get; set; }

        [JsonPropertyName("meta")]
        public Dictionary<string, JsonElement> Meta { get; set; }
    }
}
