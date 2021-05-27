using Jsonyte.Converters;

namespace Jsonyte.Serialization
{
    internal readonly struct IncludedRef
    {
        public readonly ulong IdKey;

        public readonly ulong TypeKey;

        public readonly byte[] Id;

        public readonly byte[] Type;

        public readonly string IdString;

        public readonly string TypeString;

        public readonly JsonObjectConverter Converter;

        public readonly object Value;

        public readonly int? RelationshipId;

        public IncludedRef(
            ulong idKey,
            ulong typeKey,
            byte[] id,
            byte[] type,
            string idString,
            string typeString,
            JsonObjectConverter converter,
            object value,
            int? relationshipId = null)
        {
            IdKey = idKey;
            TypeKey = typeKey;
            Id = id;
            Type = type;
            IdString = idString;
            TypeString = typeString;
            Converter = converter;
            Value = value;
            RelationshipId = relationshipId;
        }

        public ResourceIdentifier Identifier => new(Id, Type);

        public bool IsUnwritten()
        {
            return RelationshipId != null;
        }
    }
}
