using System.Text.Json.Serialization;

namespace JsonApi
{
    internal class JsonApiResourceDocument<T>
    {
        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }
}
