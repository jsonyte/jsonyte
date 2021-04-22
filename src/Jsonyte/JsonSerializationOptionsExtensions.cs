using System;
using System.Linq;
using System.Text.Json;
using Jsonyte.Converters;
using Jsonyte.Converters.Objects;
using Jsonyte.Serialization.Contracts;
using Jsonyte.Serialization.Metadata;
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

            if (!options.Converters.OfType<JsonApiRelationshipConverterFactory>().Any())
            {
                options.Converters.Add(new JsonApiRelationshipConverterFactory());
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

        internal static JsonObjectConverter GetObjectConverter<T>(this JsonSerializerOptions options)
        {
            return GetState(options).ObjectConverters.GetOrAdd(typeof(T), _ => new JsonObjectConverter<T>(options.GetWrappedConverter<T>()));
        }

        internal static JsonObjectConverter GetObjectConverter(this JsonSerializerOptions options, Type type)
        {
            return GetState(options).ObjectConverters.GetOrAdd(type, x =>
            {
                var converterType = typeof(JsonObjectConverter<>).MakeGenericType(x);
                var converter = options.GetConverter(type);

                return (JsonObjectConverter) Activator.CreateInstance(converterType, converter);
            });
        }

        internal static AnonymousRelationshipConverter GetAnonymousRelationshipConverter(this JsonSerializerOptions options, Type type)
        {
            return GetState(options).AnonymousConverters.GetOrAdd(type, x =>
            {
                var converterType = typeof(JsonApiAnonymousRelationshipConverter<>).MakeGenericType(x);

                return (AnonymousRelationshipConverter) Activator.CreateInstance(converterType, options);
            });
        }

        internal static JsonTypeInfo GetTypeInfo(this JsonSerializerOptions options, Type type)
        {
            return GetState(options).GetTypeInfo(type, options);
        }

        internal static MemberAccessor GetMemberAccessor(this JsonSerializerOptions options)
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
