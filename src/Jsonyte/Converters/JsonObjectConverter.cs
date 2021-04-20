using System.Text.Json;
using Jsonyte.Serialization;

namespace Jsonyte.Converters
{
    internal class JsonObjectConverter<T> : IJsonObjectConverter
    {
        private readonly WrappedJsonConverter<T> converter;

        public JsonObjectConverter(WrappedJsonConverter<T> converter)
        {
            this.converter = converter;
        }

        public void Read(ref Utf8JsonReader reader, ref TrackedResources tracked, object existingValue, JsonSerializerOptions options)
        {
            converter.ReadWrapped(ref reader, ref tracked, typeof(T), (T) existingValue, options);
        }

        public void Write(Utf8JsonWriter writer, ref TrackedResources tracked, object value, JsonSerializerOptions options)
        {
            converter.WriteWrapped(writer, ref tracked, (T) value, options);
        }
    }
}
