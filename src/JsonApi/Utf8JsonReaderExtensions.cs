using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Converters;
using JsonApi.Validation;

namespace JsonApi
{
    internal static class Utf8JsonReaderExtensions
    {
        public static bool IsObject(this ref Utf8JsonReader reader)
        {
            return reader.TokenType == JsonTokenType.StartObject;
        }

        public static bool IsInObject(this ref Utf8JsonReader reader)
        {
            return reader.TokenType != JsonTokenType.EndObject;
        }

        public static bool IsArray(this ref Utf8JsonReader reader)
        {
            return reader.TokenType == JsonTokenType.StartArray;
        }

        public static bool IsInArray(this ref Utf8JsonReader reader)
        {
            return reader.TokenType != JsonTokenType.EndArray;
        }

        public static void ReadArray(this ref Utf8JsonReader reader, string description)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonApiException($"Invalid JSON:API {description} array, expected array");
            }

            reader.Read();
        }

        public static JsonApiDocumentState ReadDocument(this ref Utf8JsonReader reader)
        {
            reader.ReadObject("document");

            return new JsonApiDocumentState();
        }

        public static JsonApiResourceState ReadResource(this ref Utf8JsonReader reader)
        {
            reader.ReadObject("resource");

            return new JsonApiResourceState();
        }

        public static JsonApiRelationshipState ReadRelationship(this ref Utf8JsonReader reader)
        {
            reader.ReadObject("relationship");

            return new JsonApiRelationshipState();
        }

        public static void ReadObject(this ref Utf8JsonReader reader, string description)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonApiException($"Invalid JSON:API {description} object, expected JSON object");
            }

            reader.Read();
        }

        public static string? ReadMember(this ref Utf8JsonReader reader, ref JsonApiDocumentState state)
        {
            var name = reader.ReadMember("top-level");

            state.AddFlag(name);

            return name;
        }

        public static string? ReadMember(this ref Utf8JsonReader reader, ref JsonApiResourceState state)
        {
            var name = reader.ReadMember("resource object");

            state.AddFlag(name);

            return name;
        }

        public static string? ReadMember(this ref Utf8JsonReader reader, ref JsonApiRelationshipState state)
        {
            var name = reader.ReadMember("relationship");

            state.AddFlag(name);

            return name;
        }

        public static string? ReadMember(this ref Utf8JsonReader reader, string description)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonApiException($"Expected JSON:API {description} object property but found '{reader.GetString()}'");
            }

            var name = reader.GetString();
            reader.Read();

            return name;
        }

        public static T? Read<T>(this ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (options.GetConverter(typeof(T)) is not JsonConverter<T> converter)
            {
                throw new JsonApiException($"Could not find converter for type '{typeof(T)}'");
            }

            return converter.Read(ref reader, typeof(T), options);
        }

        public static T? ReadWrapped<T>(this ref Utf8JsonReader reader, ref JsonApiState state, JsonSerializerOptions options)
        {
            if (options.GetConverter(typeof(T)) is not JsonApiConverter<T> converter)
            {
                throw new JsonApiException($"Could not find converter for type '{typeof(T)}'");
            }

            return converter.ReadWrapped(ref reader, ref state, typeof(T), default, options);
        }

        public static JsonApiResourceIdentifier ReadAheadIdentifier(this Utf8JsonReader reader, JsonSerializerOptions options)
        {
            string? id = null;
            string? type = null;

            reader.ReadObject("resource identifier");

            while (reader.IsInObject())
            {
                var name = reader.GetString();
                reader.Read();

                if (name == JsonApiMembers.Id)
                {
                    id = reader.GetString();
                }
                else if (name == JsonApiMembers.Type)
                {
                    type = reader.GetString();
                }
                else
                {
                    reader.Skip();
                }

                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(type))
                {
                    break;
                }

                reader.Read();
            }

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(type))
            {
                throw new JsonApiException("JSON:API resource identifier must contain 'id' and 'type' members");
            }

            return new JsonApiResourceIdentifier(id!, type!);
        }
    }
}
