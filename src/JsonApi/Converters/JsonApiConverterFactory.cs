using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Converters.Collections;
using JsonApi.Converters.Objects;
using JsonApi.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiConverterFactory : JsonConverterFactory
    {
        private static readonly HashSet<Type> IgnoredTypes = new()
        {
            typeof(JsonApiDocumentLinks),
            typeof(JsonApiErrorLinks),
            typeof(JsonApiErrorSource),
            typeof(JsonApiLinks),
            typeof(JsonApiMeta),
            typeof(JsonApiObject),
            typeof(JsonApiRelationshipLinks),
            typeof(JsonApiResource),
            typeof(JsonApiResourceIdentifier),
            typeof(JsonApiResourceLinks),
            typeof(JsonApiResourceIdentifier[]),
            typeof(JsonElement),
            typeof(string),
            typeof(Dictionary<string, JsonElement>),
            typeof(Dictionary<string, JsonApiRelationship>)
        };

        private static readonly Dictionary<Type, JsonConverter> JsonApiConverters = new()
        {
            {typeof(JsonApiError), new JsonApiErrorConverter()},
            {typeof(JsonApiDocument), new JsonApiDocumentDataConverter()},
            {typeof(JsonApiLink), new JsonApiLinkConverter()},
            {typeof(JsonApiPointer), new JsonApiPointerConverter()},
            {typeof(JsonApiRelationship), new JsonApiRelationshipConverter()},
            {typeof(JsonApiResource[]), new JsonApiResourceArrayConverter()},
            {typeof(JsonApiError[]), new JsonApiErrorsCollectionConverter<JsonApiError[]>()},
            {typeof(List<JsonApiError>), new JsonApiErrorsCollectionConverter<List<JsonApiError>>()}
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

            if (typeToConvert.IsDocument())
            {
                return true;
            }

            if (typeToConvert.IsRelationship())
            {
                return true;
            }

            if (typeToConvert.IsRelationshipResource())
            {
                return true;
            }

            if (typeToConvert.IsCollection())
            {
                var elementType = typeToConvert.GetCollectionElementType();

                if (elementType != null && elementType.IsError())
                {
                    return true;
                }
            }

            return false;
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (JsonApiConverters.TryGetValue(typeToConvert, out var converter))
            {
                return converter;
            }

            if (typeToConvert.IsDocument())
            {
                return CreateConverter(typeof(JsonApiDocumentDataConverter<>), typeToConvert.GenericTypeArguments.First());
            }

            //if (typeToConvert.IsRelationship())
            //{
            //    return CreateConverter(typeof(JsonApiRelationshipConverter<>), typeToConvert.GenericTypeArguments.First());
            //}

            if (typeToConvert.IsRelationshipResource())
            {
                var relationshipType = typeToConvert.GenericTypeArguments.First();

                if (relationshipType.IsCollection())
                {
                    var elementType = relationshipType.GetCollectionElementType();

                    if (elementType != null)
                    {
                        return CreateConverter(typeof(JsonApiRelationshipCollectionConverter<,>), relationshipType, elementType);
                    }
                }

                var info = options.GetTypeInfo(relationshipType);

                return CreateConverter(typeof(JsonApiResourceObjectRelationshipConverter<>), info, relationshipType);
            }

            if (typeToConvert.IsCollection())
            {
                var elementType = typeToConvert.GetCollectionElementType();

                if (elementType != null && elementType.IsError())
                {
                    return CreateConverter(typeof(JsonApiErrorsCollectionConverter<>), typeToConvert);
                }

                if (elementType != null)
                {
                    var collectionTypeInfo = options.GetTypeInfo(elementType);

                    var collectionConverterType = collectionTypeInfo.ParameterCount == 0
                        ? typeof(JsonApiResourceObjectCollectionConverter<,>)
                        : typeof(JsonApiResourceObjectCollectionConstructorConverter<,>);

                    return CreateConverter(collectionConverterType, typeToConvert, elementType);
                }
            }

            return null;
        }

        protected bool IsIgnoredType(Type typeToConvert)
        {
            if (typeToConvert.IsPrimitive)
            {
                return true;
            }

            return IgnoredTypes.Contains(typeToConvert);
        }

        protected JsonConverter? CreateConverter(Type converterType, params Type[] typesToConvert)
        {
            var genericType = converterType.MakeGenericType(typesToConvert);

            return Activator.CreateInstance(genericType) as JsonConverter;
        }

        protected JsonConverter? CreateConverter(Type converterType, JsonTypeInfo info, params Type[] typesToConvert)
        {
            var genericType = converterType.MakeGenericType(typesToConvert);

            return Activator.CreateInstance(genericType, info) as JsonConverter;
        }

        protected JsonConverter? CreateConverter(Type converterType, IJsonObjectConverter converter, params Type[] typesToConvert)
        {
            var genericType = converterType.MakeGenericType(typesToConvert);

            return Activator.CreateInstance(genericType, converter) as JsonConverter;
        }
    }
}
