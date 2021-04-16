using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonyte.Converters.Collections;
using Jsonyte.Converters.Objects;
using Jsonyte.Serialization;

namespace Jsonyte.Converters
{
    internal class JsonApiResourceConverterFactory : JsonApiConverterFactory
    {
        private static readonly Dictionary<Type, JsonConverter> JsonApiConverters = new()
        {
            {typeof(AnonymousResource), new JsonApiAnonymousResourceConverter()},
            {typeof(AnonymousResourceCollection), new JsonApiAnonymousResourceCollectionConverter()}
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

            var info = options.GetTypeInfo(typeToConvert);

            ValidateResource(info);

            var converterType = typeof(JsonApiResourceObjectConverter<>);

            return CreateConverter(converterType, info, typeToConvert);
        }

        private void ValidateResource(JsonTypeInfo info)
        {
            var idProperty = info.IdMember;

            if (!string.IsNullOrEmpty(idProperty.Name) && idProperty.MemberType != typeof(string))
            {
                throw new JsonApiFormatException("JSON:API resource id must be a string");
            }

            var typeProperty = info.TypeMember;

            if (string.IsNullOrEmpty(typeProperty.Name))
            {
                throw new JsonApiFormatException("JSON:API resource must have a 'type' member");
            }

            if (typeProperty.MemberType != typeof(string))
            {
                throw new JsonApiFormatException("JSON:API resource type must be a string");
            }
        }
    }
}
