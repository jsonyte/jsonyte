using System;

namespace Jsonyte.Validation
{
    internal ref struct ResourceState
    {
        private ResourceFlags flags;

        public ResourceFlags AddFlag(scoped ReadOnlySpan<byte> member)
        {
            var memberFlag = GetFlag(member);

            if (memberFlag != ResourceFlags.None && flags.IsSet(memberFlag))
            {
                throw new JsonApiFormatException($"Invalid JSON:API resource, duplicate '{member.GetString()}' member");
            }

            flags |= memberFlag;

            return memberFlag;
        }

        private ResourceFlags GetFlag(scoped ReadOnlySpan<byte> member)
        {
            var key = member.GetKey();

            if (key == JsonApiMembers.IdKey)
            {
                return ResourceFlags.Id;
            }

            if (key == JsonApiMembers.TypeKey)
            {
                return ResourceFlags.Type;
            }

            if (key == JsonApiMembers.RelationshipsKey && member.SequenceEqual(JsonApiMembers.RelationshipsEncoded.EncodedUtf8Bytes))
            {
                return ResourceFlags.Relationships;
            }

            if (key == JsonApiMembers.MetaKey)
            {
                return ResourceFlags.Meta;
            }

            if (key == JsonApiMembers.LinksKey)
            {
                return ResourceFlags.Links;
            }

            if (key == JsonApiMembers.AttributesKey && member.SequenceEqual(JsonApiMembers.AttributesEncoded.EncodedUtf8Bytes))
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
