using System;
using System.Runtime.InteropServices;

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
            if (member.Length == 0)
            {
                return DocumentFlags.None;
            }

            ref var initial = ref MemoryMarshal.GetReference(member);

            if (initial == 0x64 && JsonApiMembers.DataEncoded.EncodedUtf8Bytes.SequenceEqual(member))
            {
                return DocumentFlags.Data;
            }

            if (initial == 0x65 && JsonApiMembers.ErrorsEncoded.EncodedUtf8Bytes.SequenceEqual(member))
            {
                return DocumentFlags.Errors;
            }

            if (initial == 0x6d && JsonApiMembers.MetaEncoded.EncodedUtf8Bytes.SequenceEqual(member))
            {
                return DocumentFlags.Meta;
            }

            if (initial == 0x6a && JsonApiMembers.JsonApiEncoded.EncodedUtf8Bytes.SequenceEqual(member))
            {
                return DocumentFlags.Jsonapi;
            }

            if (initial == 0x6c && JsonApiMembers.LinksEncoded.EncodedUtf8Bytes.SequenceEqual(member))
            {
                return DocumentFlags.Links;
            }

            if (initial == 0x69 && JsonApiMembers.IncludedEncoded.EncodedUtf8Bytes.SequenceEqual(member))
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
