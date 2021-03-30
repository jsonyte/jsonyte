namespace JsonApi.Validation
{
    internal ref struct JsonApiDocumentState
    {
        private JsonApiDocumentFlags flags;

        public void AddFlag(string? member)
        {
            var memberFlag = member switch
            {
                JsonApiMembers.Data => JsonApiDocumentFlags.Data,
                JsonApiMembers.Errors => JsonApiDocumentFlags.Errors,
                JsonApiMembers.Meta => JsonApiDocumentFlags.Meta,
                JsonApiMembers.JsonApi => JsonApiDocumentFlags.Jsonapi,
                JsonApiMembers.Links => JsonApiDocumentFlags.Links,
                JsonApiMembers.Included => JsonApiDocumentFlags.Included,
                _ => JsonApiDocumentFlags.None
            };

            if (flags.HasFlag(memberFlag))
            {
                throw new JsonApiException($"Invalid JSON:API document, duplicate '{member}' member");
            }

            flags |= memberFlag;
        }

        public void Validate()
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

        public bool HasFlag(JsonApiDocumentFlags flag)
        {
            return flags.HasFlag(flag);
        }

        public bool IsEmpty()
        {
            return flags == JsonApiDocumentFlags.None;
        }
    }
}
