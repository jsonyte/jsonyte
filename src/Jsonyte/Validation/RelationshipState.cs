using System;
using System.Runtime.InteropServices;

namespace Jsonyte.Validation
{
    internal ref struct RelationshipState
    {
        private RelationshipFlags flags;

        public void AddFlag(ReadOnlySpan<byte> member)
        {
            var memberFlag = GetFlag(member);

            if (memberFlag != RelationshipFlags.None && flags.IsSet(memberFlag))
            {
                throw new JsonApiFormatException($"Invalid JSON:API relationship, duplicate '{member.GetString()}' member");
            }

            flags |= memberFlag;
        }

        private RelationshipFlags GetFlag(ReadOnlySpan<byte> member)
        {
            if (member.Length == 0)
            {
                return RelationshipFlags.None;
            }

            ref var initial = ref MemoryMarshal.GetReference(member);

            if (initial == 0x6c && JsonApiMembers.LinksEncoded.EncodedUtf8Bytes.SequenceEqual(member))
            {
                return RelationshipFlags.Links;
            }

            if (initial == 0x64 && JsonApiMembers.DataEncoded.EncodedUtf8Bytes.SequenceEqual(member))
            {
                return RelationshipFlags.Data;
            }

            if (initial == 0x6d && JsonApiMembers.MetaEncoded.EncodedUtf8Bytes.SequenceEqual(member))
            {
                return RelationshipFlags.Meta;
            }

            return RelationshipFlags.None;
        }

        public void Validate()
        {
            if (!flags.IsSet(RelationshipFlags.Links) &&
                !flags.IsSet(RelationshipFlags.Data) &&
                !flags.IsSet(RelationshipFlags.Meta))
            {
                throw new JsonApiFormatException("JSON:API relationship must contain a 'links', 'data' or 'meta' member");
            }
        }
    }
}
