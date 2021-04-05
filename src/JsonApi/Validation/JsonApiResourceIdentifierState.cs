namespace JsonApi.Validation
{
    internal ref struct JsonApiResourceIdentifierState
    {
        private JsonApiResourceIdentifierFlags flags;

        public void AddFlag(string? member)
        {
            var memberFlag = member switch
            {
                JsonApiMembers.Id => JsonApiResourceIdentifierFlags.Id,
                JsonApiMembers.Type => JsonApiResourceIdentifierFlags.Type,
                _ => JsonApiResourceIdentifierFlags.Unknown
            };

            if (memberFlag != JsonApiResourceIdentifierFlags.Unknown && flags.HasFlag(memberFlag))
            {
                throw new JsonApiException($"Invalid JSON:API resource, duplicate '{member}' member");
            }

            flags |= memberFlag;
        }

        public void Validate()
        {
            if (!flags.HasFlag(JsonApiResourceIdentifierFlags.Id) && !flags.HasFlag(JsonApiResourceIdentifierFlags.Type))
            {
                throw new JsonApiException("JSON:API resource identifier must contain 'id' and 'type' members");
            }
        }
    }
}
