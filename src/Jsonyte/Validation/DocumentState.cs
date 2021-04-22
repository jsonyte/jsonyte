using System;

namespace Jsonyte.Validation
{
    internal ref struct DocumentState
    {
        private DocumentFlags flags;

        public DocumentFlags AddFlag(ReadOnlySpan<byte> member)
        {
            var memberFlag = GetFlag(member);

            if (memberFlag != DocumentFlags.None && flags.IsSet(memberFlag))
            {
                throw new JsonApiFormatException($"Invalid JSON:API document, duplicate '{member.GetString()}' member");
            }

            flags |= memberFlag;

            return memberFlag;
        }

        private DocumentFlags GetFlag(ReadOnlySpan<byte> member)
        {
            var key = member.GetKey();

            if (key == JsonApiMembers.DataKey)
            {
                return DocumentFlags.Data;
            }

            if (key == JsonApiMembers.ErrorsKey)
            {
                return DocumentFlags.Errors;
            }

            if (key == JsonApiMembers.MetaKey)
            {
                return DocumentFlags.Meta;
            }

            if (key == JsonApiMembers.JsonApiKey)
            {
                return DocumentFlags.Jsonapi;
            }

            if (key == JsonApiMembers.LinksKey)
            {
                return DocumentFlags.Links;
            }

            if (key == JsonApiMembers.IncludedKey && member.SequenceEqual(JsonApiMembers.IncludedEncoded.EncodedUtf8Bytes))
            {
                return DocumentFlags.Included;
            }

            return DocumentFlags.None;
        }

        public void Validate()
        {
            if (!flags.IsSet(DocumentFlags.Data) &&
                !flags.IsSet(DocumentFlags.Errors) &&
                !flags.IsSet(DocumentFlags.Meta))
            {
                throw new JsonApiFormatException("JSON:API document must contain 'data', 'errors' or 'meta' members");
            }

            if (flags.IsSet(DocumentFlags.Data) && flags.IsSet(DocumentFlags.Errors))
            {
                throw new JsonApiFormatException("JSON:API document must not contain both 'data' and 'errors' members");
            }

            if (flags.IsSet(DocumentFlags.Included) && !flags.IsSet(DocumentFlags.Data))
            {
                throw new JsonApiFormatException("JSON:API document must contain 'data' member if 'included' member is specified");
            }
        }

        public bool HasFlag(DocumentFlags flag)
        {
            return flags.IsSet(flag);
        }

        public bool IsEmpty()
        {
            return flags == DocumentFlags.None;
        }
    }
}
