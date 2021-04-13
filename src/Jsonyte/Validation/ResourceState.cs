using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Jsonyte.Validation
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
                _ => ResourceFlags.None
            };

            if (memberFlag != ResourceFlags.None && flags.IsSet(memberFlag))
            {
                throw new JsonApiFormatException($"Invalid JSON:API resource, duplicate '{member}' member");
            }

            flags |= memberFlag;
        }

        public void AddFlag(ReadOnlySpan<byte> member)
        {
            var memberFlag = GetFlag(member);

            if (memberFlag != ResourceFlags.None && flags.IsSet(memberFlag))
            {
                var value = Encoding.UTF8.GetString(member.ToArray());

                throw new JsonApiFormatException($"Invalid JSON:API resource, duplicate '{value}' member");
            }

            flags |= memberFlag;
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
