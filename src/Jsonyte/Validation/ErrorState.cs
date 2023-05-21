using System;

namespace Jsonyte.Validation
{
    internal ref struct ErrorState
    {
        private ErrorFlags flags;

        public ErrorFlags AddFlag(scoped ReadOnlySpan<byte> member)
        {
            var memberFlag = GetFlag(member);

            if (memberFlag != ErrorFlags.None && flags.IsSet(memberFlag))
            {
                throw new JsonApiFormatException($"Invalid JSON:API document, duplicate '{member.GetString()}' member");
            }

            flags |= memberFlag;

            return memberFlag;
        }

        private ErrorFlags GetFlag(scoped ReadOnlySpan<byte> member)
        {
            var key = member.GetKey();

            if (key == JsonApiMembers.IdKey)
            {
                return ErrorFlags.Id;
            }

            if (key == JsonApiMembers.LinksKey)
            {
                return ErrorFlags.Links;
            }

            if (key == JsonApiMembers.StatusKey)
            {
                return ErrorFlags.Status;
            }

            if (key == JsonApiMembers.CodeKey)
            {
                return ErrorFlags.Code;
            }

            if (key == JsonApiMembers.TitleKey)
            {
                return ErrorFlags.Title;
            }

            if (key == JsonApiMembers.DetailKey)
            {
                return ErrorFlags.Detail;
            }

            if (key == JsonApiMembers.SourceKey)
            {
                return ErrorFlags.Source;
            }

            if (key == JsonApiMembers.MetaKey)
            {
                return ErrorFlags.Meta;
            }

            return ErrorFlags.None;
        }
    }
}
