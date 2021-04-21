using System;
using System.Runtime.InteropServices;

namespace Jsonyte.Validation
{
    internal ref struct ResourceState
    {
        private ResourceFlags flags;

        public ResourceFlags AddFlag(ReadOnlySpan<byte> member)
        {
            var memberFlag = GetFlag(member);

            if (memberFlag != ResourceFlags.None && flags.IsSet(memberFlag))
            {
                throw new JsonApiFormatException($"Invalid JSON:API resource, duplicate '{member.GetString()}' member");
            }

            flags |= memberFlag;

            return memberFlag;
        }

        private ResourceFlags GetFlag(ReadOnlySpan<byte> member)
        {
            if (member.Length == 0)
            {
                return ResourceFlags.None;
            }

            ref var initial = ref MemoryMarshal.GetReference(member);

            if (initial == 0x69 && JsonApiMembers.IdEncoded.EncodedUtf8Bytes.SequenceEqual(member))
            {
                return ResourceFlags.Id;
            }

            if (initial == 0x74 && JsonApiMembers.TypeEncoded.EncodedUtf8Bytes.SequenceEqual(member))
            {
                return ResourceFlags.Type;
            }

            if (initial == 0x72 && JsonApiMembers.RelationshipsEncoded.EncodedUtf8Bytes.SequenceEqual(member))
            {
                return ResourceFlags.Relationships;
            }

            if (initial == 0x6d && JsonApiMembers.MetaEncoded.EncodedUtf8Bytes.SequenceEqual(member))
            {
                return ResourceFlags.Meta;
            }

            if (initial == 0x61 && JsonApiMembers.AttributesEncoded.EncodedUtf8Bytes.SequenceEqual(member))
            {
                return ResourceFlags.Attributes;
            }

            return ResourceFlags.None;
        }

        public void Validate()
        {
            if (!flags.IsSet(ResourceFlags.Type))
            {
                throw new JsonApiFormatException("JSON:API resource must contain a 'type' member");
            }
        }
    }
}
