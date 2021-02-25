﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiConverterFactory : JsonConverterFactory
    {
        private static readonly HashSet<Type> DefaultTypes = new()
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

            if (typeToConvert.IsDocument())
            {
                return true;
            }

            if (typeToConvert.IsCollection())
            {
                var collectionType = typeToConvert.GetCollectionType();

                if (collectionType.IsError() || collectionType.IsResource())
                {
                    return true;
                }
            }

            return typeToConvert.IsResource();
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert.IsDocument())
            {
                return CreateConverter(typeof(JsonApiDocumentConverter<>), typeToConvert);
            }

            if (typeToConvert.IsCollection())
            {
                var collectionType = typeToConvert.GetCollectionType();

                if (collectionType.IsError())
                {
                    return CreateConverter(typeof(NewJsonApiErrorsConverter<>), typeToConvert);
                }

                if (collectionType.IsResource())
                {
                    return CreateConverter(typeof(JsonApiResourceCollectionConverter<,>), typeToConvert, collectionType);
                }
            }

            return CreateConverter(typeof(JsonApiResourceConverter<>), typeToConvert);
        }

        private JsonConverter CreateConverter(Type converterType, params Type[] typesToConvert)
        {
            var genericType = converterType.MakeGenericType(typesToConvert);

            return Activator.CreateInstance(genericType) as JsonConverter;
        }
    }
}
