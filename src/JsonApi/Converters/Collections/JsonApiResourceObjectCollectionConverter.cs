using System;
using System.Collections.Generic;
using System.Text.Json;
using JsonApi.Serialization;

namespace JsonApi.Converters.Collections
{
    internal class JsonApiResourceObjectCollectionConverter<T, TElement> : WrappedJsonConverter<T>
    {
        public Type? ElementType { get; } = typeof(TElement);

        public JsonTypeCategory TypeCategory { get; } = typeof(T).GetTypeCategory();

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var resources = default(T);

            var state = reader.ReadDocument();
            var tracked = new TrackedResources();

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref state);

                if (name == JsonApiMembers.Data)
                {
                    resources = ReadWrapped(ref reader, ref tracked, typeToConvert, default, options);
                }
                else if (name == JsonApiMembers.Included)
                {
                    ReadIncluded(ref reader, ref tracked, options);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            tracked.Release();

            return resources;
        }

        public override T? ReadWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, T? existingValue, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            reader.ReadArray(JsonApiArrayCode.Resources);

            var resources = new List<TElement>();

            var converter = options.GetWrappedConverter<TElement>();

            while (reader.IsInArray())
            {
                var resource = converter.ReadWrapped(ref reader, ref tracked, ElementType!, default, options);

                if (resource != null)
                {
                    resources.Add(resource);
                }

                reader.Read();
            }

            return (T) GetCollection(resources);
        }

        private void ReadIncluded(ref Utf8JsonReader reader, ref TrackedResources tracked, JsonSerializerOptions options)
        {
            reader.ReadArray(JsonApiArrayCode.Included);

            while (reader.IsInArray())
            {
                var identifier = reader.ReadAheadIdentifier();

                if (tracked.TryGetIncluded(identifier, out var included))
                {
                    included.Converter.Read(ref reader, ref tracked, included.Value, options);
                }

                reader.Read();
            }
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var tracked = new TrackedResources();

            writer.WriteStartObject();
            writer.WritePropertyName(JsonApiMembers.Data);

            WriteWrapped(writer, ref tracked, value, options);

            if (tracked.Count > 0)
            {
                writer.WritePropertyName(JsonApiMembers.Included);
                writer.WriteStartArray();

                foreach (var identifier in tracked.Identifiers)
                {
                    if (tracked.TryGetIncluded(identifier, out var included))
                    {
                        included.Converter.Write(writer, ref tracked, included.Value, options);
                    }
                }

                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }

        public override void WriteWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, T value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else if (value is IEnumerable<TElement> collection)
            {
                var converter = options.GetWrappedConverter<TElement>();

                writer.WriteStartArray();

                foreach (var element in collection)
                {
                    converter.WriteWrapped(writer, ref tracked, element, options);
                }

                writer.WriteEndArray();
            }
            else
            {
                throw new JsonApiFormatException($"JSON:API resources collection of type '{typeof(T).Name}' must be an enumerable");
            }
        }

        private object GetCollection(List<TElement> resources)
        {
            return TypeCategory == JsonTypeCategory.Array
                ? resources.ToArray()
                : resources;
        }
    }
}
