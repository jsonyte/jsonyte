using System.Text.Json.Serialization;

namespace JsonApi
{
    public sealed class JsonApiErrorSource
    {
        [JsonPropertyName("pointer")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiPointer Pointer { get; set; }

        [JsonPropertyName("parameter")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Parameter { get; set; }
    }
}
