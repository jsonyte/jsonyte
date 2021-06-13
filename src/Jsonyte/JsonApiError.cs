using System.Text.Json.Serialization;

namespace Jsonyte
{
    /// <summary>
    /// Represents an error in a <see href="https://jsonapi.org/">JSON:API</see> document response.
    /// </summary>
    public sealed class JsonApiError
    {
        /// <summary>
        /// Gets a unique identifier for this particular occurrence of the error.
        /// </summary>
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Id { get; set; }

        /// <summary>
        /// Gets the links that contain further details about this particular error.
        /// </summary>
        [JsonPropertyName("links")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiErrorLinks? Links { get; set; }

        /// <summary>
        /// Gets the HTTP status code applicable to this error.
        /// </summary>
        [JsonPropertyName("status")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Status { get; set; }

        /// <summary>
        /// Gets the application-specific error code for this error.
        /// </summary>
        [JsonPropertyName("code")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Code { get; set; }

        /// <summary>
        /// Gets a short, human-readable summary of the error.
        /// </summary>
        /// <remarks>
        /// The title of the error should not change for different occurrences of the same error.
        /// </remarks>
        [JsonPropertyName("title")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Title { get; set; }

        /// <summary>
        /// Gets the human-readable explanation of the error.
        /// </summary>
        [JsonPropertyName("detail")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Detail { get; set; }

        /// <summary>
        /// Gets an object containing references to the source of the error, including a pointer to the
        /// associated entity in the request document, and a value indicating the URI query parameter
        /// causing the error.
        /// </summary>
        [JsonPropertyName("source")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiErrorSource? Source { get; set; }

        /// <summary>
        /// Gets a meta object containing non-standard meta-information about the error.
        /// </summary>
        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiMeta? Meta { get; set; }
    }
}
