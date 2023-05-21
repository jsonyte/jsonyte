using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonyte.Validation;

namespace Jsonyte
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

        public static void ReadArray(this ref Utf8JsonReader reader, JsonApiArrayCode code)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonApiFormatException(code);
            }

            reader.Read();
        }

        public static void ReadDocument(this ref Utf8JsonReader reader)
        {
            reader.ReadObject(JsonApiMemberCode.Document);
        }

        public static void ReadResource(this ref Utf8JsonReader reader)
        {
            reader.ReadObject(JsonApiMemberCode.Resource);
        }

        public static void ReadRelationship(this ref Utf8JsonReader reader)
        {
            reader.ReadObject(JsonApiMemberCode.Relationship);
        }

        public static void ReadError(this ref Utf8JsonReader reader)
        {
            reader.ReadObject(JsonApiMemberCode.Error);
        }

        public static void ReadObject(this ref Utf8JsonReader reader, JsonApiMemberCode code)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonApiFormatException(code);
            }

            reader.Read();
        }

        public static DocumentFlags ReadMember(this ref Utf8JsonReader reader, scoped ref DocumentState state)
        {
            var name = reader.ReadMember(JsonApiMemberCode.TopLevel);

            return state.AddFlag(name);
        }

        public static ResourceFlags ReadMember(this ref Utf8JsonReader reader, scoped ref ResourceState state)
        {
            var name = reader.ReadMember(JsonApiMemberCode.Resource);

            return state.AddFlag(name);
        }

        public static RelationshipFlags ReadMember(this ref Utf8JsonReader reader, scoped ref RelationshipState state)
        {
            var name = reader.ReadMember(JsonApiMemberCode.Relationship);

            return state.AddFlag(name);
        }

        public static ErrorFlags ReadMember(this ref Utf8JsonReader reader, scoped ref ErrorState state)
        {
            var name = reader.ReadMember(JsonApiMemberCode.Error);

            return state.AddFlag(name);
        }

        public static ReadOnlySpan<byte> ReadMember(this ref Utf8JsonReader reader, JsonApiMemberCode code)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonApiFormatException(code, reader.GetString());
            }

            var name = reader.ValueSpan;
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

        public static ResourceIdentifier ReadAheadIdentifier(this Utf8JsonReader reader)
        {
            ReadOnlySpan<byte> id = default;
            ReadOnlySpan<byte> type = default;

            reader.ReadObject(JsonApiMemberCode.ResourceIdentifier);

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(JsonApiMemberCode.ResourceIdentifier);
                var key = name.GetKey();

                if (key == JsonApiMembers.IdKey)
                {
                    id = reader.ValueSpan;
                }
                else if (key == JsonApiMembers.TypeKey)
                {
                    type = reader.ValueSpan;
                }
                else
                {
                    reader.Skip();
                }

                if (!id.IsEmpty && !type.IsEmpty)
                {
                    break;
                }

                reader.Read();
            }

            return new ResourceIdentifier(id, type);
        }

        public static ResourceIdentifier ReadResourceIdentifier(this ref Utf8JsonReader reader)
        {
            ReadOnlySpan<byte> id = default;
            ReadOnlySpan<byte> type = default;

            reader.ReadObject(JsonApiMemberCode.ResourceIdentifier);

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(JsonApiMemberCode.ResourceIdentifier);
                var key = name.GetKey();

                if (key == JsonApiMembers.IdKey)
                {
                    id = reader.ValueSpan;
                }
                else if (key == JsonApiMembers.TypeKey)
                {
                    type = reader.ValueSpan;
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            if (id.IsEmpty || type.IsEmpty)
            {
                throw new JsonApiFormatException("JSON:API resource identifier must contain 'id' and 'type' members");
            }

            return new ResourceIdentifier(id, type);
        }
    }
}
