using System.Text.Json;

namespace Jsonyte
{
    /// <summary>
    /// The exception that is thrown when an error serializing or deserializing a <see href="https://jsonapi.org/">JSON:API</see> document occurs.
    /// </summary>
    public class JsonApiException : JsonException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiException"/> class with the specified error message.
        /// </summary>
        /// <param name="message">The message the describes the error.</param>
        public JsonApiException(string? message)
            : base(message)
        {
        }
    }
}
