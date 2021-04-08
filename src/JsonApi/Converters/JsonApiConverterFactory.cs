using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Converters.Collections;
using JsonApi.Converters.Objects;

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

        protected bool IsIgnoredType(Type typeToConvert)
        {
            if (typeToConvert.IsPrimitive)
            {
                return true;
            }

            return IgnoredTypes.Contains(typeToConvert);
        }

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

                    var converterType = typeof(JsonApiRelationshipCollectionConverter<,>).MakeGenericType(relationshipType, elementType);

                    return (JsonConverter) Activator.CreateInstance(converterType);
                }

                var type = typeof(JsonApiResourceObjectRelationshipConverter<>).MakeGenericType(relationshipType);

                return (JsonConverter) Activator.CreateInstance(type);
            }

            if (typeToConvert.IsCollection())
            {
                var elementType = typeToConvert.GetCollectionElementType();

                if (elementType != null && elementType.IsError())
                {
                    return CreateConverter(typeof(JsonApiErrorsCollectionConverter<>), typeToConvert);
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
