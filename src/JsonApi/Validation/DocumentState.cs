namespace JsonApi.Validation
{
    internal ref struct DocumentState
    {
        private DocumentFlags flags;

        public void AddFlag(string? member)
        {
            var memberFlag = member switch
            {
                JsonApiMembers.Data => DocumentFlags.Data,
                JsonApiMembers.Errors => DocumentFlags.Errors,
                JsonApiMembers.Meta => DocumentFlags.Meta,
                JsonApiMembers.JsonApi => DocumentFlags.Jsonapi,
                JsonApiMembers.Links => DocumentFlags.Links,
                JsonApiMembers.Included => DocumentFlags.Included,
                _ => DocumentFlags.None
            };

            if (memberFlag != DocumentFlags.None && flags.HasFlag(memberFlag))
            {
                throw new JsonApiFormatException($"Invalid JSON:API document, duplicate '{member}' member");
            }

            flags |= memberFlag;
        }

        public void Validate()
        {
            if (!flags.HasFlag(DocumentFlags.Data) &&
                !flags.HasFlag(DocumentFlags.Errors) &&
                !flags.HasFlag(DocumentFlags.Meta))
            {
                throw new JsonApiFormatException("JSON:API document must contain 'data', 'errors' or 'meta' members");
            }

            if (flags.HasFlag(DocumentFlags.Data) && flags.HasFlag(DocumentFlags.Errors))
            {
                throw new JsonApiFormatException("JSON:API document must not contain both 'data' and 'errors' members");
            }

            if (flags.HasFlag(DocumentFlags.Included) && !flags.HasFlag(DocumentFlags.Data))
            {
                throw new JsonApiFormatException("JSON:API document must contain 'data' member if 'included' member is specified");
            }
        }

        public bool HasFlag(DocumentFlags flag)
        {
            return flags.HasFlag(flag);
        }

        public bool IsEmpty()
        {
            return flags == DocumentFlags.None;
        }
    }
}
