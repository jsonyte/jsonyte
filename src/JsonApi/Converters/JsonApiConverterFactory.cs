using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiConverterFactory : JsonConverterFactory
    {
        private static readonly HashSet<Type> DefaultTypes = new HashSet<Type>
        {
            typeof(JsonApiError),
            typeof(JsonApiResource),
            typeof(JsonApiResourceIdentifier)
        };

        public override bool CanConvert(Type typeToConvert)
        {
            if (DefaultTypes.Contains(typeToConvert))
            {
                return false;
            }

            if (typeToConvert.IsCollection() && typeToConvert.GetCollectionType() == typeof(JsonApiError))
            {
                return true;
            }

            return typeToConvert.IsResource();
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert.IsCollection() && typeToConvert.GetCollectionType() == typeof(JsonApiError))
            {
                return CreateConverter(typeof(JsonApiErrorsConverter<>), typeToConvert);
            }

            return CreateConverter(typeof(JsonApiResourceConverter<>), typeToConvert);
        }

        private JsonConverter CreateConverter(Type converterType, Type typeToConvert)
        {
            var genericType = converterType.MakeGenericType(typeToConvert);

            return Activator.CreateInstance(genericType) as JsonConverter;
        }
    }
}
