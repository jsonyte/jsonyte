using System.Text.Json.Serialization;

namespace JsonApi
{
    public class JsonApiRelationship
    {
        [JsonPropertyName("data")]
        public JsonApiResourceIdentifier[] Data { get; set; }

        [JsonPropertyName("links")]
        public JsonApiRelationshipLinks Links { get; set; }

        [JsonPropertyName("meta")]
        public JsonApiMeta Meta { get; set; }
    }

    public class JsonApiRelationship<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonPropertyName("links")]
        public JsonApiRelationshipLinks Links { get; set; }

        [JsonPropertyName("meta")]
        public JsonApiMeta Meta { get; set; }
    }
}
