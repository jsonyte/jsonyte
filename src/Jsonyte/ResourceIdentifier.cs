using System;

namespace Jsonyte
{
    internal readonly ref struct ResourceIdentifier
    {
        public readonly ReadOnlySpan<byte> Id;

        public readonly ReadOnlySpan<byte> Type;

        public ResourceIdentifier(ReadOnlySpan<byte> id, ReadOnlySpan<byte> type)
        {
            Id = id;
            Type = type;
        }
    }
}
