using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi
{
    public sealed class JsonApiError
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("links")]
        public JsonApiErrorLinks Links { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("detail")]
        public string Detail { get; set; }

        [JsonPropertyName("source")]
        public JsonApiErrorSource Source { get; set; }

        [JsonPropertyName("meta")]
        public Dictionary<string, JsonElement> Meta { get; set; }
    }
}
