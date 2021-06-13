using System.Text.Json.Serialization;

namespace Jsonyte
{
    /// <summary>
    /// Represents a link URL that relates to a <see href="https://jsonapi.org/">JSON:API</see> document or member.
    /// </summary>
    public sealed class JsonApiLink
    {
        /// <summary>
        /// Converts a string containing a link URL to an <see cref="JsonApiLink"/>.
        /// </summary>
        /// <param name="href">A string that contains a link URL.</param>
        public static implicit operator JsonApiLink(string href)
        {
            return new()
            {
                Href = href
            };
        }

        /// <summary>
        /// Converts a <see cref="JsonApiLink"/> containing a link URL to a string.
        /// </summary>
        /// <param name="value">A <see cref="JsonApiLink"/> that contains a link URL.</param>
        public static implicit operator string?(JsonApiLink value)
        {
            return value.Href;
        }

        /// <summary>
        /// Gets a string containing the link URL.
        /// </summary>
        [JsonPropertyName("href")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Href { get; set; }

        /// <summary>
        /// Gets a meta object containing non-standard meta-information about the link.
        /// </summary>
        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiMeta? Meta { get; set; }
    }
}
