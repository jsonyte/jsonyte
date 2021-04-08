using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Converters;
using JsonApi.Serialization;
using JsonApi.Validation;

namespace JsonApi
{
    internal static class Utf8JsonReaderExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsObject(this ref Utf8JsonReader reader)
        {
            return reader.TokenType == JsonTokenType.StartObject;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInObject(this ref Utf8JsonReader reader)
        {
            return reader.TokenType != JsonTokenType.EndObject;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsArray(this ref Utf8JsonReader reader)
        {
            return reader.TokenType == JsonTokenType.StartArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInArray(this ref Utf8JsonReader reader)
        {
            return reader.TokenType != JsonTokenType.EndArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReadArray(this ref Utf8JsonReader reader, JsonApiArrayCode code)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonApiFormatException(code);
            }

            reader.Read();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DocumentState ReadDocument(this ref Utf8JsonReader reader)
        {
            reader.ReadObject(JsonApiMemberCode.Document);

            return new DocumentState();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResourceState ReadResource(this ref Utf8JsonReader reader)
        {
            reader.ReadObject(JsonApiMemberCode.Resource);

            return new ResourceState();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RelationshipState ReadRelationship(this ref Utf8JsonReader reader)
        {
            reader.ReadObject(JsonApiMemberCode.Relationship);

            return new RelationshipState();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReadObject(this ref Utf8JsonReader reader, JsonApiMemberCode code)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonApiFormatException(code);
            }

            reader.Read();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? ReadMember(this ref Utf8JsonReader reader, ref DocumentState state)
        {
            var name = reader.ReadMember(JsonApiMemberCode.TopLevel);

            state.AddFlag(name);

            return name;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? ReadMember(this ref Utf8JsonReader reader, ref ResourceState state)
        {
            var name = reader.ReadMember(JsonApiMemberCode.Resource);

            state.AddFlag(name);

            return name;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? ReadMember(this ref Utf8JsonReader reader, ref RelationshipState state)
        {
            var name = reader.ReadMember(JsonApiMemberCode.Relationship);

            state.AddFlag(name);

            return name;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? Read<T>(this ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (options.GetConverter(typeof(T)) is not JsonConverter<T> converter)
            {
                throw new JsonApiException($"Could not find converter for type '{typeof(T)}'");
            }

            return converter.Read(ref reader, typeof(T), options);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                throw new JsonApiFormatException("JSON:API resource identifier must contain 'id' and 'type' members");
            }

            return new JsonApiResourceIdentifier(id!, type!);
        }
    }
}
