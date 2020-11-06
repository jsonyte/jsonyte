using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Converters;

namespace JsonApi
{
    public class JsonApiDocument<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonPropertyName("errors")]
        public JsonApiError[] Errors { get; set; }

        [JsonPropertyName("meta")]
        public Dictionary<string, JsonElement> Meta { get; set; }

        [JsonPropertyName("jsonapi")]
        public JsonApiObject Version { get; set; }

        [JsonPropertyName("links")]
        public JsonApiLinks Links { get; set; }

        [JsonPropertyName("included")]
        public JsonApiResource[] Included { get; set; }
    }

    [JsonConverter(typeof(JsonApiDocumentConverter<JsonApiDocument>))]
    public class JsonApiDocument
    {
        [JsonPropertyName("data")]
        [JsonConverter(typeof(JsonApiResourcesConverter))]
        public JsonApiResource[] Data { get; set; }

        [JsonPropertyName("errors")]
        public JsonApiError[] Errors { get; set; }

        [JsonPropertyName("meta")]
        public Dictionary<string, JsonElement> Meta { get; set; }

        [JsonPropertyName("jsonapi")]
        public JsonApiObject Version { get; set; }

        [JsonPropertyName("links")]
        public JsonApiLinks Links { get; set; }

        [JsonPropertyName("included")]
        public JsonApiResource[] Included { get; set; }
    }
}
