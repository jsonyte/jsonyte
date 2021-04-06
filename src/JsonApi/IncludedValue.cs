using JsonApi.Serialization;

namespace JsonApi
{
    internal struct IncludedValue
    {
        public JsonApiResourceIdentifier Identifier;

        public IJsonValueConverter Converter;

        public object Value;

        public IncludedValue(JsonApiResourceIdentifier identifier, IJsonValueConverter converter, object value)
        {
            Identifier = identifier;
            Converter = converter;
            Value = value;
        }
    }
}
