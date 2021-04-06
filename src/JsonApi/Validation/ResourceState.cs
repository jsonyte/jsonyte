namespace JsonApi.Validation
{
    internal ref struct ResourceState
    {
        private ResourceFlags flags;

        public void AddFlag(string? member)
        {
            var memberFlag = member switch
            {
                JsonApiMembers.Id => ResourceFlags.Id,
                JsonApiMembers.Type => ResourceFlags.Type,
                JsonApiMembers.Relationships => ResourceFlags.Relationships,
                _ => ResourceFlags.Unknown
            };

            if (memberFlag != ResourceFlags.Unknown && flags.HasFlag(memberFlag))
            {
                throw new JsonApiException($"Invalid JSON:API resource, duplicate '{member}' member");
            }

            flags |= memberFlag;
        }

        public void Validate()
        {
            if (!flags.HasFlag(ResourceFlags.Type))
            {
                throw new JsonApiException("JSON:API resource must contain a 'type' member");
            }
        }
    }
}
