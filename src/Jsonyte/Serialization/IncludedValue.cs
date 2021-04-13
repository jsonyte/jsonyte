namespace JsonApi.Serialization
{
    internal ref struct IncludedValue
    {
        public ResourceIdentifier Identifier;

        public IJsonObjectConverter Converter;

        public object Value;

        public IncludedValue(ResourceIdentifier identifier, IJsonObjectConverter converter, object value)
        {
            Identifier = identifier;
            Converter = converter;
            Value = value;
        }

        public bool HasIdentifier(ResourceIdentifier identifier)
        {
            return Identifier.Equals(identifier);
        }
    }
}
