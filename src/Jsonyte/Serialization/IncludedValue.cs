namespace Jsonyte.Serialization
{
    internal ref struct IncludedValue
    {
        public ResourceIdentifier Identifier;

        public IJsonObjectConverter Converter;

        public object Value;

        public int? RelationshipId;

        public IncludedValue(ResourceIdentifier identifier, IJsonObjectConverter converter, object value, int? relationshipId = null)
        {
            Identifier = identifier;
            Converter = converter;
            Value = value;
            RelationshipId = relationshipId;
        }

        public bool HasIdentifier(ResourceIdentifier identifier)
        {
            return Identifier.Equals(identifier);
        }

        public bool IsUnwritten()
        {
            return RelationshipId != null;
        }
    }
}
