namespace JsonApi
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
    }
}
