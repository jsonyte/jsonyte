using System;
using System.Collections.Generic;
using System.Text.Json;
using Jsonyte.Serialization;
using Jsonyte.Serialization.Metadata;
using Jsonyte.Validation;

namespace Jsonyte.Converters.Collections
{
    internal class JsonApiResourceObjectCollectionConverter<T, TElement> : WrappedJsonConverter<T>
    {
        private WrappedJsonConverter<TElement>? wrappedConverter;

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

                if (name == DocumentFlags.Data)
                {
                    resources = ReadWrapped(ref reader, ref tracked, typeToConvert, default, options);
                }
                else if (name == DocumentFlags.Included)
                {
                    ReadIncluded(ref reader, ref tracked, options);
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

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

            var converter = GetWrappedConverter(options);

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
            writer.WritePropertyName(JsonApiMembers.DataEncoded);

            WriteWrapped(writer, ref tracked, value, options);

            if (tracked.Count > 0)
            {
                var nameWritten = false;
                var index = 0;

                while (index < tracked.Count)
                {
                    var included = tracked.Get(index);

                    if (!tracked.IsEmitted(included))
                    {
                        if (!nameWritten)
                        {
                            writer.WritePropertyName(JsonApiMembers.IncludedEncoded);
                            writer.WriteStartArray();

                            nameWritten = true;
                        }

                        included.Converter.Write(writer, ref tracked, included.Value, options);
                    }

                    index++;
                }

                if (nameWritten)
                {
                    writer.WriteEndArray();
                }
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
                var converter = GetWrappedConverter(options);

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

        private WrappedJsonConverter<TElement> GetWrappedConverter(JsonSerializerOptions options)
        {
            return wrappedConverter ??= options.GetWrappedConverter<TElement>();
        }
    }
}
