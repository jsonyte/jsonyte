using System.Text.Json;
using JsonApi.Converters;

namespace JsonApi.Serialization
{
    internal class JsonValueConverter<T> : IJsonValueConverter
    {
        private readonly WrappedJsonConverter<T> converter;

        public JsonValueConverter(WrappedJsonConverter<T> converter)
        {
            this.converter = converter;
        }

        public void Read(ref Utf8JsonReader reader, ref TrackedResources tracked, object existingValue, JsonSerializerOptions options)
        {
            converter.ReadWrapped(ref reader, ref tracked, typeof(T), (T) existingValue, options);
        }
    }
}
