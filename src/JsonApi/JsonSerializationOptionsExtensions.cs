using System;
using System.Linq;
using System.Text.Json;
using JsonApi.Converters;
using JsonApi.Serialization;
using JsonApi.Serialization.Reflection;

namespace JsonApi
{
    /*
        https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-ignore-properties?pivots=dotnet-5-0

        [EditorBrowsable(EditorBrowsableState.Never)]
        DONE public bool IgnoreNullValues { get; set; }

        public JsonIgnoreCondition DefaultIgnoreCondition { get; set; }

        public bool IgnoreReadOnlyProperties { get; set; }

        public bool IgnoreReadOnlyFields { get; set; }

        public bool IncludeFields { get; set; }

     */
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

            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

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
            var converters = GetState(options).RelationshipConverters;

            return (JsonApiRelationshipDetailsConverter<T>) converters.GetOrAdd(typeof(T), JsonApiConverterFactory.GetRelationshipConverter);
        }

        internal static IJsonObjectConverter GetObjectConverter<T>(this JsonSerializerOptions options)
        {
            return GetState(options).ObjectConverters.GetOrAdd(typeof(T), _ => new JsonObjectConverter<T>(options.GetWrappedConverter<T>()));
        }

        internal static JsonTypeInfo GetTypeInfo(this JsonSerializerOptions options, Type type)
        {
            return GetState(options).Types.GetOrAdd(type, x => new JsonTypeInfo(x, options));
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
