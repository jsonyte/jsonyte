using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jsonyte.Converters.Collections;

namespace Jsonyte.Converters
{
    internal class JsonApiRelationshipConverterFactory : JsonApiConverterFactory
    {
        private static readonly Dictionary<Type, JsonConverter> JsonApiConverters = new()
        {
            {typeof(AnonymousResourceCollection), new JsonApiAnonymousResourceCollectionConverter()},
            {typeof(PotentialRelationshipCollection), new JsonApiPotentialRelationshipCollectionConverter()}
        };

        public override bool CanConvert(Type typeToConvert)
        {
            if (JsonApiConverters.ContainsKey(typeToConvert))
            {
                return true;
            }

            return false;
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (JsonApiConverters.TryGetValue(typeToConvert, out var converter))
            {
                return converter;
            }

            return null;
        }
    }
}
