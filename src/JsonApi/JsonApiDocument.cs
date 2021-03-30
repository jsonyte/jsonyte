using System.Text.Json.Serialization;

namespace JsonApi
{
    public sealed class JsonApiDocument<T> : IJsonApiDocument
    {
        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }

        [JsonPropertyName("errors")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiError[]? Errors { get; set; }

        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiMeta? Meta { get; set; }

        [JsonPropertyName("jsonapi")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiObject? JsonApi { get; set; }

        [JsonPropertyName("links")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiDocumentLinks? Links { get; set; }

        [JsonPropertyName("included")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiResource[]? Included { get; set; }
    }

    public sealed class JsonApiDocument : IJsonApiDocument
    {
        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiResource[]? Data { get; set; }

        [JsonPropertyName("errors")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiError[]? Errors { get; set; }

        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiMeta? Meta { get; set; }

        [JsonPropertyName("jsonapi")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiObject? JsonApi { get; set; }

        [JsonPropertyName("links")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiDocumentLinks? Links { get; set; }

        [JsonPropertyName("included")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiResource[]? Included { get; set; }
    }
}
