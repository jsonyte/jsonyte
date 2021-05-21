using Jsonyte.Converters;

namespace Jsonyte.Serialization
{
    internal readonly struct IncludedRef
    {
        public readonly ulong IdKey;

        public readonly ulong TypeKey;

        public readonly byte[] Id;

        public readonly byte[] Type;

        public readonly JsonObjectConverter Converter;

        public readonly object Value;

        public readonly int? RelationshipId;

        public readonly bool EmitIincluded;

        public IncludedRef(ulong idKey, ulong typeKey, byte[] id, byte[] type, JsonObjectConverter converter, object value, int? relationshipId = null, bool emitIncluded = true)
        {
            IdKey = idKey;
            TypeKey = typeKey;
            Id = id;
            Type = type;
            Converter = converter;
            Value = value;
            RelationshipId = relationshipId;
            EmitIincluded = emitIncluded;
        }

        public ResourceIdentifier Identifier => new(Id, Type);

        public bool IsUnwritten()
        {
            return RelationshipId != null;
        }
    }
}
