using System.Text.Json.Serialization;

namespace Jsonyte
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
