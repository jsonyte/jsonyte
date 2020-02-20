using System.Text.Json.Serialization;

namespace JsonApi
{
    public sealed class JsonApiErrorSource
    {
        [JsonPropertyName("pointer")]
        public JsonApiPointer Pointer { get; set; }

        [JsonPropertyName("parameter")]
        public string Parameter { get; set; }
    }
}
