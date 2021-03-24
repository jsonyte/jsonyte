namespace JsonApi
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
                _ => JsonApiResourceFlags.Unknown
            };

            if (flags.HasFlag(memberFlag))
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
