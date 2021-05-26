using System;

namespace Jsonyte.Serialization
{
    internal readonly struct ResourceRef : IEquatable<ResourceRef>
    {
        public readonly string Id;

        public readonly string Type;

        public ResourceRef(string id, string type)
        {
            Id = id;
            Type = type;
        }

        public override bool Equals(object? obj)
        {
            return obj is ResourceRef other && Equals(other);
        }

        public bool Equals(ResourceRef other)
        {
            return Id == other.Id && Type == other.Type;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;

                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + Type.GetHashCode();

                return hash;
            }
        }
    }
}
