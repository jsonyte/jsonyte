using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Converters;

namespace JsonApi
{
    internal static class Utf8JsonWriterExtensions
    {
        public static void Write<T>(this Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (options.GetConverter(typeof(T)) is not JsonConverter<T> converter)
            {
                throw new JsonApiException($"Could not find converter for type '{typeof(T)}'");
            }

            converter.Write(writer, value, options);
        }

        public static void WriteWrapped<T>(this Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (options.GetConverter(typeof(T)) is not JsonApiConverter<T> converter)
            {
                throw new JsonApiException($"Could not find converter for type '{typeof(T)}'");
            }

            converter.WriteWrapped(writer, value, options);
        }
    }
}
