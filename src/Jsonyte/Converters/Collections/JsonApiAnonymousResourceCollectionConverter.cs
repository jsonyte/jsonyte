using System;
using System.Collections;
using System.Text.Json;
using Jsonyte.Serialization;

namespace Jsonyte.Converters.Collections
{
    internal class JsonApiAnonymousResourceCollectionConverter : WrappedJsonConverter<ResourceCollectionContainer>
    {
        private WrappedJsonConverter<ResourceContainer>? wrappedConverter;

        public override ResourceCollectionContainer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override ResourceCollectionContainer ReadWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, Type typeToConvert, ResourceCollectionContainer existingValue, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, ResourceCollectionContainer value, JsonSerializerOptions options)
        {
            var tracked = new TrackedResources();

            writer.WriteStartObject();
            writer.WritePropertyName(JsonApiMembers.DataEncoded);

            WriteWrapped(writer, ref tracked, value, options);

            if (tracked.Count > 0)
            {
                writer.WritePropertyName(JsonApiMembers.IncludedEncoded);
                writer.WriteStartArray();

                var index = 0;

                while (index < tracked.Count)
                {
                    var included = tracked.Get(index);
                    included.Converter.Write(writer, ref tracked, included.Value, options);

                    index++;
                }

                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }

        public override void WriteWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, ResourceCollectionContainer container, JsonSerializerOptions options)
        {
            var value = container.Value;

            if (value == null)
            {
                writer.WriteNullValue();
            }
            else if (value is IEnumerable collection)
            {
                var converter = GetWrappedConverter(options);

                writer.WriteStartArray();

                foreach (var element in collection)
                {
                    converter.WriteWrapped(writer, ref tracked, new ResourceContainer(element), options);
                }

                writer.WriteEndArray();
            }
            else
            {
                throw new JsonApiFormatException($"JSON:API resources collection of type '{value.GetType()}' must be an enumerable");
            }
        }

        private WrappedJsonConverter<ResourceContainer> GetWrappedConverter(JsonSerializerOptions options)
        {
            return wrappedConverter ??= options.GetWrappedConverter<ResourceContainer>();
        }
    }
}
