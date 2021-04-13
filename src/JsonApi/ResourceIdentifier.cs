using System;

namespace JsonApi
{
    internal ref struct ResourceIdentifier
    {
        public ResourceIdentifier(ReadOnlySpan<byte> id, ReadOnlySpan<byte> type)
        {
            Id = id;
            Type = type;
        }

        public ReadOnlySpan<byte> Id { get; }

        public ReadOnlySpan<byte> Type { get; }

        public bool Equals(ResourceIdentifier other)
        {
            return Id.SequenceEqual(other.Id) && Type.SequenceEqual(other.Type);
        }
    }
}
