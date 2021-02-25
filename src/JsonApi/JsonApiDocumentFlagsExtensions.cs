namespace JsonApi
{
    internal static class JsonApiDocumentFlagsExtensions
    {
        public static JsonApiDocumentFlags AddFlag(this JsonApiDocumentFlags flags, string name)
        {
            var memberFlag = name switch
            {
                JsonApiMembers.Data => JsonApiDocumentFlags.Data,
                JsonApiMembers.Errors => JsonApiDocumentFlags.Errors,
                JsonApiMembers.Meta => JsonApiDocumentFlags.Meta,
                JsonApiMembers.Version => JsonApiDocumentFlags.Jsonapi,
                JsonApiMembers.Links => JsonApiDocumentFlags.Links,
                JsonApiMembers.Included => JsonApiDocumentFlags.Included,
                _ => JsonApiDocumentFlags.None
            };

            if (flags.HasFlag(memberFlag))
            {
                throw new JsonApiException($"Invalid JSON:API document, duplicate '{name}' member");
            }

            return flags | memberFlag;
        }

        public static void Validate(this JsonApiDocumentFlags flags)
        {
            if (!flags.HasFlag(JsonApiDocumentFlags.Data) &&
                !flags.HasFlag(JsonApiDocumentFlags.Errors) &&
                !flags.HasFlag(JsonApiDocumentFlags.Meta))
            {
                throw new JsonApiException("JSON:API document must contain 'data', 'errors' or 'meta' members");
            }

            if (flags.HasFlag(JsonApiDocumentFlags.Data) && flags.HasFlag(JsonApiDocumentFlags.Errors))
            {
                throw new JsonApiException("JSON:API document must not contain both 'data' and 'errors' members");
            }

            if (flags.HasFlag(JsonApiDocumentFlags.Included) && !flags.HasFlag(JsonApiDocumentFlags.Data))
            {
                throw new JsonApiException("JSON:API document must contain 'data' member if 'included' member is specified");
            }
        }
    }
}
