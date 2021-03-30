using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Converters.Collections;
using JsonApi.Converters.Objects;

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
                var collectionType = typeToConvert.GetCollectionType();

                if (collectionType != null && collectionType.IsResource())
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
                var collectionType = typeToConvert.GetCollectionType();

                if (collectionType != null)
                {
                    var collectionTypeInfo = options.GetClassInfo(collectionType);

                    var collectionConverterType = collectionTypeInfo.ParameterCount == 0
                        ? typeof(JsonApiResourceCollectionConverter<,>)
                        : typeof(JsonApiResourceCollectionConstructorConverter<,>);

                    return CreateConverter(collectionConverterType, typeToConvert, collectionType);
                }
            }

            var info = options.GetClassInfo(typeToConvert);

            var converterType = info.ParameterCount == 0
                ? typeof(JsonApiResourceConverter<>)
                : typeof(JsonApiResourceConstructorConverter<>);

            return CreateConverter(converterType, typeToConvert);
        }
    }
}
