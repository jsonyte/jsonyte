using System.Text.Json.Serialization;

namespace Jsonyte
{
    /// <summary>
    /// Represents a reference to the source of an error.
    /// </summary>
    /// <remarks>
    /// The <see cref="JsonApiErrorSource"/> can optionally include the following members:
    ///
    /// <list type="bullet">
    /// <item>
    /// <term>Pointer</term>
    /// <description>A JSON pointer <see href="https://tools.ietf.org/html/rfc6901">RFC6901</see> to the associated entity in the request document.</description>
    /// </item>
    /// <item>
    /// <term>Parameter</term>
    /// <description>A string indicating which URI query parameter caused the error</description>
    /// </item>
    /// </list>
    ///
    /// <example>
    /// The pointer to an entity in the request document may look like the below:
    ///
    /// <list type="bullet">
    /// <item>
    /// <term>/data</term>
    /// <description>A pointer to primary data</description>
    /// </item>
    /// <item>
    /// <term>/data/attributes/title</term>
    /// <description>A pointer to a specific attribute</description>
    /// </item>
    /// </list>
    /// </example>
    /// </remarks>
    public sealed class JsonApiErrorSource
    {
        /// <summary>
        /// Gets a JSON pointer <see href="https://tools.ietf.org/html/rfc6901">RFC6901</see> to the associated entity in the request document.
        /// </summary>
        [JsonPropertyName("pointer")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiPointer? Pointer { get; set; }

        /// <summary>
        /// Gets a string indicating which URI query parameter caused the error.
        /// </summary>
        [JsonPropertyName("parameter")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Parameter { get; set; }
    }
}
