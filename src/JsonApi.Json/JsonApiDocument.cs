using System.Text.Json.Serialization;

namespace JsonApi
{
    public class JsonApiDocument
    {
        [JsonPropertyName("data")]
        public Resource Data { get; set; }

        [JsonPropertyName("errors")]
        public Error[] Errors { get; set; }

        [JsonPropertyName("meta")]
        public Meta Meta { get; set; }

        [JsonPropertyName("jsonapi")]
        public JsonApiVersion Jsonapi { get; set; }

        [JsonPropertyName("links")]
        public Links Links { get; set; }

        [JsonPropertyName("included")]
        public object[] Included { get; set; }
    }
}