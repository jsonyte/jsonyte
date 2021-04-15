using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonyte.Converters.Collections;
using Jsonyte.Converters.Objects;
using Jsonyte.Serialization;

namespace Jsonyte.Converters
{
    internal class JsonApiResourceConverterFactory : JsonApiConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (IsIgnoredType(typeToConvert))
            {
                return false;
            }

            if (typeToConvert == typeof(ResourceContainer) || typeToConvert == typeof(ResourceCollectionContainer))
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
            if (typeToConvert == typeof(ResourceContainer))
            {
                return new JsonApiAnonymousResourceConverter();
            }

            if (typeToConvert == typeof(ResourceCollectionContainer))
            {
                return new JsonApiAnonymousResourceCollectionConverter();
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

            if (typeToConvert != typeof(object))
            {
                ValidateResource(info);
            }

            var converterType = typeof(JsonApiResourceObjectConverter<>);

            return CreateConverter(converterType, info, typeToConvert);
        }

        private void ValidateResource(JsonTypeInfo info)
        {
            var idProperty = info.GetMember(JsonApiMembers.Id);

            if (!string.IsNullOrEmpty(idProperty.Name) && idProperty.MemberType != typeof(string))
            {
                throw new JsonApiFormatException("JSON:API resource id must be a string");
            }

            var typeProperty = info.GetMember(JsonApiMembers.Type);

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
