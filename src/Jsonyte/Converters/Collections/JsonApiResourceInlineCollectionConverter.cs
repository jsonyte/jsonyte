using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonyte.Serialization.Contracts;
using Jsonyte.Serialization.Metadata;

namespace Jsonyte.Converters.Collections
{
    internal class JsonApiResourceInlineCollectionConverter<T, TElement> : JsonConverter<InlineResource<T>>
    {
        private JsonConverter<InlineResource<TElement>>? resourceConverter;

        public JsonTypeCategory TypeCategory { get; } = typeof(T).GetTypeCategory();

        public override InlineResource<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            reader.ReadArray(JsonApiArrayCode.Resources);

            var resources = new List<TElement>();

            var converter = GetResourceConverter(options);

            while (reader.IsInArray())
            {
                var resource = converter.Read(ref reader, typeof(TElement), options).Resource;

                if (resource != null)
                {
                    resources.Add(resource);
                }

                reader.Read();
            }

            return new InlineResource<T>((T) GetCollection(resources));
        }

        public override void Write(Utf8JsonWriter writer, InlineResource<T> value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        private JsonConverter<InlineResource<TElement>> GetResourceConverter(JsonSerializerOptions options)
        {
            return resourceConverter ??= options.GetConverter<InlineResource<TElement>>();
        }

        private object GetCollection(List<TElement> resources)
        {
            return TypeCategory == JsonTypeCategory.Array
                ? resources.ToArray()
                : resources;
        }
    }
}
