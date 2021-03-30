using System.Text.Json.Serialization;

namespace JsonApi
{
    public class JsonApiErrorLinks : JsonApiLinks
    {
        private const string AboutKey = "about";

        [JsonPropertyName(AboutKey)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? About
        {
            get => GetOrNull(AboutKey);
            set => SetOrRemove(AboutKey, value);
        }
    }
}
