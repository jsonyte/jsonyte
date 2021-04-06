namespace JsonApi.Validation
{
    internal ref struct RelationshipState
    {
        private RelationshipFlags flags;

        public void AddFlag(string? member)
        {
            var memberFlag = member switch
            {
                JsonApiMembers.Links => RelationshipFlags.Links,
                JsonApiMembers.Data => RelationshipFlags.Data,
                JsonApiMembers.Meta => RelationshipFlags.Meta,
                _ => RelationshipFlags.Unknown
            };

            if (memberFlag != RelationshipFlags.Unknown && flags.HasFlag(memberFlag))
            {
                throw new JsonApiFormatException($"Invalid JSON:API relationship, duplicate '{member}' member");
            }

            flags |= memberFlag;
        }

        public void Validate()
        {
            if (!flags.HasFlag(RelationshipFlags.Links) &&
                !flags.HasFlag(RelationshipFlags.Data) &&
                !flags.HasFlag(RelationshipFlags.Meta))
            {
                throw new JsonApiFormatException("JSON:API relationship must contain a 'links', 'data' or 'meta' member");
            }
        }
    }
}
