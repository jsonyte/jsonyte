using System;

namespace Jsonyte.Validation
{
    internal ref struct RelationshipState
    {
        private RelationshipFlags flags;

        public RelationshipFlags AddFlag(scoped ReadOnlySpan<byte> member)
        {
            var memberFlag = GetFlag(member);

            if (memberFlag != RelationshipFlags.None && flags.IsSet(memberFlag))
            {
                throw new JsonApiFormatException($"Invalid JSON:API relationship, duplicate '{member.GetString()}' member");
            }

            flags |= memberFlag;

            return memberFlag;
        }

        private RelationshipFlags GetFlag(scoped ReadOnlySpan<byte> member)
        {
            var key = member.GetKey();

            if (key == JsonApiMembers.LinksKey)
            {
                return RelationshipFlags.Links;
            }

            if (key == JsonApiMembers.DataKey)
            {
                return RelationshipFlags.Data;
            }

            if (key == JsonApiMembers.MetaKey)
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
