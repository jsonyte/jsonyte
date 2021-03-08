using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi
{
    internal static class Utf8JsonReaderExtensions
    {
        public static JsonApiDocumentState BeginDocument(this ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonApiException("Invalid JSON:API document, expected JSON object");
            }

            reader.Read();

            return new JsonApiDocumentState();
        }

        public static string? ReadToMember(this ref Utf8JsonReader reader, ref JsonApiDocumentState state)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonApiException($"Expected top-level JSON:API property but found '{reader.GetString()}'");
            }

            var name = reader.GetString();
            state.AddFlag(name);

            reader.Read();

            return name;
        }

        public static bool IsDocument(this Utf8JsonReader reader)
        {
            if (reader.CurrentDepth > 0)
            {
                return false;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                return false;
            }

            return true;
        }

        public static T? ReadWithConverter<T>(this ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (options.GetConverter(typeof(T)) is not JsonConverter<T> converter)
            {
                throw new JsonApiException($"Could not find converter for type '{typeof(T)}'");
            }

            return converter.Read(ref reader, typeof(T), options);
        }
    }
}
