namespace JsonApi.Validation
{
    internal ref struct ResourceIdentifierState
    {
        private ResourceIdentifierFlags flags;

        public void AddFlag(string? member)
        {
            var memberFlag = member switch
            {
                JsonApiMembers.Id => ResourceIdentifierFlags.Id,
                JsonApiMembers.Type => ResourceIdentifierFlags.Type,
                _ => ResourceIdentifierFlags.Unknown
            };

            if (memberFlag != ResourceIdentifierFlags.Unknown && flags.HasFlag(memberFlag))
            {
                throw new JsonApiFormatException($"Invalid JSON:API resource, duplicate '{member}' member");
            }

            flags |= memberFlag;
        }

        public void Validate()
        {
            if (!flags.HasFlag(ResourceIdentifierFlags.Id) && !flags.HasFlag(ResourceIdentifierFlags.Type))
            {
                throw new JsonApiFormatException("JSON:API resource identifier must contain 'id' and 'type' members");
            }
        }
    }
}
