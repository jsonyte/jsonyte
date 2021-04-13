namespace Jsonyte
{
    public class JsonApiFormatException : JsonApiException
    {
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
