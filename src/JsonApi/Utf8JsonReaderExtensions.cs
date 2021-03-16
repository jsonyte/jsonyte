﻿using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Converters;

namespace JsonApi
{
    internal static class Utf8JsonReaderExtensions
    {
        public static bool IsObject(this ref Utf8JsonReader reader)
        {
            return reader.TokenType != JsonTokenType.EndObject;
        }

        public static bool IsArray(this ref Utf8JsonReader reader)
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

        public static T? Read<T>(this ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (options.GetConverter(typeof(T)) is not JsonConverter<T> converter)
            {
                throw new JsonApiException($"Could not find converter for type '{typeof(T)}'");
            }

            return converter.Read(ref reader, typeof(T), options);
        }

        public static T? ReadWrapped<T>(this ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (options.GetConverter(typeof(T)) is not JsonApiConverter<T> converter)
            {
                throw new JsonApiException($"Could not find converter for type '{typeof(T)}'");
            }

            return converter.ReadWrapped(ref reader, typeof(T), options);
        }
    }
}
