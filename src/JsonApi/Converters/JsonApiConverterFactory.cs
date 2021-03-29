using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiConverterFactory : JsonConverterFactory
    {
        private static readonly HashSet<Type> DefaultTypes = new()
        {
            typeof(JsonApiResource),
            typeof(JsonApiResourceIdentifier)
        };

        protected bool IsDefaultType(Type typeToConvert)
        {
            return DefaultTypes.Contains(typeToConvert);
        }

        public override bool CanConvert(Type typeToConvert)
        {
            if (IsDefaultType(typeToConvert))
            {
                return false;
            }

            if (typeToConvert.IsError())
            {
                return true;
            }

            if (typeToConvert.IsDocument())
            {
                return true;
            }

            if (typeToConvert.IsCollection())
            {
                var collectionType = typeToConvert.GetCollectionType();

                if (collectionType != null && collectionType.IsError())
                {
                    return true;
                }

                if (collectionType != null && collectionType == typeof(JsonApiResource))
                {
                    return true;
                }
            }

            return false;
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert.IsError())
            {
                return new JsonApiErrorConverter();
            }

            if (typeToConvert.IsDocument())
            {
                return typeToConvert.IsGenericType
                    ? CreateConverter(typeof(JsonApiDocumentConverter<>), typeToConvert.GenericTypeArguments.First())
                    : new JsonApiDocumentConverter();
            }

            if (typeToConvert.IsCollection())
            {
                var collectionType = typeToConvert.GetCollectionType();

                if (collectionType != null && collectionType.IsError())
                {
                    return CreateConverter(typeof(JsonApiErrorsConverter<>), typeToConvert);
                }

                if (collectionType != null && collectionType == typeof(JsonApiResource))
                {
                    return new JsonApiResourcesConverter();
                }
            }

            return null;
        }

        protected JsonConverter? CreateConverter(Type converterType, params Type[] typesToConvert)
        {
            var genericType = converterType.MakeGenericType(typesToConvert);

            return Activator.CreateInstance(genericType) as JsonConverter;
        }
    }
}
