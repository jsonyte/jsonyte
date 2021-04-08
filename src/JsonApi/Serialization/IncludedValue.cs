namespace JsonApi.Serialization
{
    internal struct IncludedValue
    {
        public JsonApiResourceIdentifier Identifier;

        public IJsonObjectConverter Converter;

        public object Value;

        public IncludedValue(JsonApiResourceIdentifier identifier, IJsonObjectConverter converter, object value)
        {
            Identifier = identifier;
            Converter = converter;
            Value = value;
        }
    }
}
