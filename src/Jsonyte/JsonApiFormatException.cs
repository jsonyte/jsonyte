namespace Jsonyte
{
    /// <summary>
    /// The exception that is thrown when the format of a <see href="https://jsonapi.org/">JSON:API</see> document is invalid or when a document is not well formed.
    /// </summary>
    public class JsonApiFormatException : JsonApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiException"/> class with the specified error message.
        /// </summary>
        /// <param name="message">The message the describes the error.</param>
        public JsonApiFormatException(string? message)
            : base(message)
        {
        }

        internal JsonApiFormatException(JsonApiMemberCode code, string? value = null)
            : this(GetMessage(code, value))
        {
        }

        internal JsonApiFormatException(JsonApiArrayCode code)
            : this(GetMessage(code))
        {
        }

        private static string GetMessage(JsonApiMemberCode code, string? value = null)
        {
            var description = code switch
            {
                JsonApiMemberCode.TopLevel => "top-level",
                JsonApiMemberCode.Relationship => "relationship",
                JsonApiMemberCode.Resource => "resource",
                JsonApiMemberCode.ResourceAttributes => "resource attributes",
                JsonApiMemberCode.ResourceIdentifier => "resource identifier",
                JsonApiMemberCode.Document => "document",
                JsonApiMemberCode.Error => "error",
                _ => throw new JsonApiException("Invalid JSON:API member code")
            };

            return value == null
                ? $"Invalid JSON:API {description} object, expected JSON object"
                : $"Expected JSON:API {description} member but found '{value}'";
        }

        private static string GetMessage(JsonApiArrayCode code)
        {
            var description = code switch
            {
                JsonApiArrayCode.Errors => "errors",
                JsonApiArrayCode.Relationships => "relationships",
                JsonApiArrayCode.Resources => "resources",
                JsonApiArrayCode.Included => "included",
                JsonApiArrayCode.ResourceIdentifiers => "resource identifiers",
                _ => throw new JsonApiException("Invalid JSON:API array code")
            };

            return $"Invalid JSON:API {description} array, expected array";
        }
    }
}
