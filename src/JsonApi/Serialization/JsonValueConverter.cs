using System.Text.Json;
using JsonApi.Converters;

namespace JsonApi.Serialization
{
    internal class JsonValueConverter<T> : IJsonValueConverter
    {
        private readonly JsonApiConverter<T> converter;

        public JsonValueConverter(JsonApiConverter<T> converter)
        {
            this.converter = converter;
        }

        public void Read(ref Utf8JsonReader reader, ref JsonApiState state, object existingValue, JsonSerializerOptions options)
        {
            converter.ReadWrapped(ref reader, ref state, typeof(T), (T) existingValue, options);
        }
    }
}
