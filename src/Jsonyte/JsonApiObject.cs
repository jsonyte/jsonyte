using System.Text.Json.Serialization;

namespace Jsonyte
{
    /// <summary>
    /// Represents an object describing the implementation characteristics of the server.
    /// </summary>
    public sealed class JsonApiObject
    {
        /// <summary>
        /// Gets a string indicating the highest version of the <see href="https://jsonapi.org">JSON:API</see> specification supported.
        /// </summary>
        [JsonPropertyName("version")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Version { get; set; } = "1.0";

        /// <summary>
        /// Gets a meta object containing non-standard meta-information about the server.
        /// </summary>
        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiMeta? Meta { get; set; }
    }
}
