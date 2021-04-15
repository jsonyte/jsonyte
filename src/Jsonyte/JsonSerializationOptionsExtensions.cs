﻿using System;
using System.Linq;
using System.Text.Json;
using Jsonyte.Converters;
using Jsonyte.Serialization;
using Jsonyte.Serialization.Reflection;

namespace Jsonyte
{
    public static class JsonSerializationOptionsExtensions
    {
        public static JsonSerializerOptions AddJsonApi(this JsonSerializerOptions options)
        {
            if (!options.Converters.OfType<JsonApiStateConverter>().Any())
            {
                options.Converters.Add(new JsonApiStateConverter());
            }

            if (!options.Converters.OfType<JsonApiConverterFactory>().Any())
            {
                options.Converters.Add(new JsonApiConverterFactory());
            }

            if (!options.Converters.OfType<JsonApiResourceConverterFactory>().Any())
            {
                options.Converters.Add(new JsonApiResourceConverterFactory());
            }

            options.PropertyNamingPolicy ??= JsonNamingPolicy.CamelCase;

            return options;
        }

        internal static StringComparer GetPropertyComparer(this JsonSerializerOptions options)
        {
            return options.PropertyNameCaseInsensitive
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal;
        }

        internal static WrappedJsonConverter<T> GetWrappedConverter<T>(this JsonSerializerOptions options)
        {
            var converter = options.GetConverter(typeof(T));

            if (converter is not WrappedJsonConverter<T> jsonApiConverter)
            {
                throw new JsonApiException($"Converter not found for type {typeof(T)}");
            }

            return jsonApiConverter;
        }

        internal static JsonApiRelationshipDetailsConverter<T> GetRelationshipConverter<T>(this JsonSerializerOptions options)
        {
            var converter = options.GetConverter(typeof(RelationshipResource<T>));

            if (converter is not JsonApiRelationshipDetailsConverter<T> jsonApiConverter)
            {
                throw new JsonApiException($"Converter not found for type {typeof(T)}");
            }

            return jsonApiConverter;
        }

        internal static IJsonObjectConverter GetObjectConverter<T>(this JsonSerializerOptions options)
        {
            return GetState(options).ObjectConverters.GetOrAdd(typeof(T), _ => new JsonObjectConverter<T>(options.GetWrappedConverter<T>()));
        }

        internal static JsonTypeInfo GetTypeInfo(this JsonSerializerOptions options, Type type)
        {
            return GetState(options).GetTypeInfo(type, options);
        }

        internal static IMemberAccessor GetMemberAccessor(this JsonSerializerOptions options)
        {
            return GetState(options).MemberAccessor;
        }

        private static JsonApiStateConverter GetState(JsonSerializerOptions options)
        {
            if (options.GetConverter(typeof(JsonApiStateConverter)) is not JsonApiStateConverter state)
            {
                throw new JsonApiException("JSON:API extensions not initialized, please use 'AddJsonApi' on 'JsonSerializerOptions' first");
            }

            return state;
        }
    }
}
