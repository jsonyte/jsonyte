using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Converters.Collections;
using JsonApi.Converters.Objects;
using JsonApi.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiResourceConverterFactory : JsonApiConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (IsIgnoredType(typeToConvert))
            {
                return false;
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
            if (typeToConvert.IsCollection())
            {
                var elementType = typeToConvert.GetCollectionElementType();

                if (elementType != null)
                {
                    var collectionTypeInfo = options.GetTypeInfo(elementType);

                    var collectionConverterType = collectionTypeInfo.ParameterCount == 0
                        ? typeof(JsonApiResourceObjectCollectionConverter<,>)
                        : typeof(JsonApiResourceObjectCollectionConstructorConverter<,>);

                    return CreateConverter(collectionConverterType, typeToConvert, elementType);
                }
            }

            var info = options.GetTypeInfo(typeToConvert);

            ValidateResource(info);

            var converterType = info.ParameterCount == 0
                ? typeof(JsonApiResourceObjectConverter<>)
                : typeof(JsonApiResourceObjectConstructorConverter<>);

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
