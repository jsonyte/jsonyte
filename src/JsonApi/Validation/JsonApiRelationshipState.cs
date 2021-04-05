namespace JsonApi.Validation
{
    internal ref struct JsonApiRelationshipState
    {
        private JsonApiRelationshipFlags flags;

        public void AddFlag(string? member)
        {
            var memberFlag = member switch
            {
                JsonApiMembers.Links => JsonApiRelationshipFlags.Links,
                JsonApiMembers.Data => JsonApiRelationshipFlags.Data,
                JsonApiMembers.Meta => JsonApiRelationshipFlags.Meta,
                _ => JsonApiRelationshipFlags.Unknown
            };

            if (memberFlag != JsonApiRelationshipFlags.Unknown && flags.HasFlag(memberFlag))
            {
                throw new JsonApiException($"Invalid JSON:API relationship, duplicate '{member}' member");
            }

            flags |= memberFlag;
        }

        public void Validate()
        {
            if (!flags.HasFlag(JsonApiRelationshipFlags.Links) &&
                !flags.HasFlag(JsonApiRelationshipFlags.Data) &&
                !flags.HasFlag(JsonApiRelationshipFlags.Meta))
            {
                throw new JsonApiException("JSON:API relationship must contain a 'links', 'data' or 'meta' member");
            }
        }
    }
}
