using System.Text.Json.Serialization;

namespace Jsonyte
{
    /// <summary>
    /// Represents the links that relate to the errors of a <see href="https://jsonapi.org/">JSON:API</see> document.
    /// </summary>
    public class JsonApiErrorLinks : JsonApiLinks
    {
        private const string AboutKey = "about";

        /// <summary>
        /// Gets a link that leads to further details about an error.
        /// </summary>
        [JsonPropertyName(AboutKey)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? About
        {
            get => GetOrNull(AboutKey);
            set => SetOrRemove(AboutKey, value);
        }
    }
}
