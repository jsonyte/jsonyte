using System.Text.Json.Serialization;

namespace Jsonyte
{
    /// <summary>
    /// Represents the links that relate to the resource in a <see href="https://jsonapi.org/">JSON:API</see> document.
    /// </summary>
    public class JsonApiResourceLinks : JsonApiLinks
    {
        private const string SelfKey = "self";

        /// <summary>
        /// Gets a link that identifies the resource.
        /// </summary>
        [JsonPropertyName(SelfKey)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLink? Self
        {
            get => GetOrNull(SelfKey);
            set => SetOrRemove(SelfKey, value);
        }
    }
}
