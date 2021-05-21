using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonyte.Converters.Collections;
using Jsonyte.Converters.Objects;
using Jsonyte.Serialization.Contracts;
using Jsonyte.Serialization.Metadata;

namespace Jsonyte.Converters
{
    internal class JsonApiResourceConverterFactory : JsonApiConverterFactory
    {
        private static readonly Dictionary<Type, JsonConverter> JsonApiConverters = new()
        {
            {typeof(AnonymousResource), new JsonApiAnonymousResourceConverter()}
        };

        public override bool CanConvert(Type typeToConvert)
        {
            if (IsIgnoredType(typeToConvert))
            {
                return false;
            }

            if (JsonApiConverters.ContainsKey(typeToConvert))
            {
                return true;
            }

            if (typeToConvert.IsCollection())
            {
                var elementType = typeToConvert.GetCollectionElementType();

                if (elementType != null && elementType.IsResource())
                {
                    return true;
                }
            }

            return typeToConvert.IsResource();
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (JsonApiConverters.TryGetValue(typeToConvert, out var converter))
            {
                return converter;
            }

            if (typeToConvert.IsCollection())
            {
                var elementType = typeToConvert.GetCollectionElementType();

                if (elementType != null)
                {
                    var collectionConverterType = typeof(JsonApiResourceObjectCollectionConverter<,>);

                    return CreateConverter(collectionConverterType, typeToConvert, elementType);
                }
            }

            var converterType = typeof(JsonApiResourceObjectConverter<>);

            return CreateConverter(converterType, typeToConvert);
        }
    }
}
