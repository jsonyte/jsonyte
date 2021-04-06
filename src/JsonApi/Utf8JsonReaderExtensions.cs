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

        public static DocumentState ReadDocument(this ref Utf8JsonReader reader)
        {
            reader.ReadObject(JsonApiMemberCode.Document);

            return new DocumentState();
        }

        public static ResourceState ReadResource(this ref Utf8JsonReader reader)
        {
            reader.ReadObject(JsonApiMemberCode.Resource);

            return new ResourceState();
        }

        public static RelationshipState ReadRelationship(this ref Utf8JsonReader reader)
        {
            reader.ReadObject(JsonApiMemberCode.Relationship);

            return new RelationshipState();
        }

        public static void ReadObject(this ref Utf8JsonReader reader, JsonApiMemberCode code)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonApiFormatException(code);
            }

            reader.Read();
        }

        public static string? ReadMember(this ref Utf8JsonReader reader, ref DocumentState state)
        {
            var name = reader.ReadMember(JsonApiMemberCode.TopLevel);

            state.AddFlag(name);

            return name;
        }

        public static string? ReadMember(this ref Utf8JsonReader reader, ref ResourceState state)
        {
            var name = reader.ReadMember(JsonApiMemberCode.Resource);

            state.AddFlag(name);

            return name;
        }

        public static string? ReadMember(this ref Utf8JsonReader reader, ref RelationshipState state)
        {
            var name = reader.ReadMember(JsonApiMemberCode.Relationship);

            state.AddFlag(name);

            return name;
        }

        public static string? ReadMember(this ref Utf8JsonReader reader, JsonApiMemberCode code)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonApiFormatException(code, reader.GetString());
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

        public static T? ReadWrapped<T>(this ref Utf8JsonReader reader, ref TrackedResources tracked, JsonSerializerOptions options)
        {
            if (options.GetConverter(typeof(T)) is not WrappedJsonConverter<T> converter)
            {
                throw new JsonApiException($"Could not find converter for type '{typeof(T)}'");
            }

            return converter.ReadWrapped(ref reader, ref tracked, typeof(T), default, options);
        }

        public static JsonApiResourceIdentifier ReadAheadIdentifier(this Utf8JsonReader reader)
        {
            string? id = null;
            string? type = null;

            reader.ReadObject(JsonApiMemberCode.ResourceIdentifier);

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
