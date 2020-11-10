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
        public JsonApiMeta Meta { get; set; }
    }
}
