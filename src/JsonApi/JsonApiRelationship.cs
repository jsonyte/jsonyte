using System.Text.Json.Serialization;

namespace JsonApi
{
    public class JsonApiRelationship
    {
        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiResourceIdentifier[]? Data { get; set; }

        [JsonPropertyName("links")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiRelationshipLinks? Links { get; set; }

        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiMeta? Meta { get; set; }
    }

    public class JsonApiRelationship<T>
    {
        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }

        [JsonPropertyName("links")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiRelationshipLinks? Links { get; set; }

        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiMeta? Meta { get; set; }
    }
}
