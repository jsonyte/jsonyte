namespace JsonApi.Validation
{
    internal ref struct JsonApiResourceState
    {
        private JsonApiResourceFlags flags;

        public void AddFlag(string? member)
        {
            var memberFlag = member switch
            {
                JsonApiMembers.Id => JsonApiResourceFlags.Id,
                JsonApiMembers.Type => JsonApiResourceFlags.Type,
                JsonApiMembers.Relationships => JsonApiResourceFlags.Relationships,
                _ => JsonApiResourceFlags.Unknown
            };

            if (memberFlag != JsonApiResourceFlags.Unknown && flags.HasFlag(memberFlag))
            {
                throw new JsonApiException($"Invalid JSON:API resource, duplicate '{member}' member");
            }

            flags |= memberFlag;
        }

        public void Validate()
        {
            if (!flags.HasFlag(JsonApiResourceFlags.Type))
            {
                throw new JsonApiException("JSON:API resource must contain a 'type' member");
            }
        }
    }
}
