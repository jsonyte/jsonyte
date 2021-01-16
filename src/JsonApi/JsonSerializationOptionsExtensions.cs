using System;
using System.Linq;
using System.Text.Json;
using JsonApi.Converters;
using JsonApi.Serialization;

namespace JsonApi
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

            return options;
        }

        internal static JsonClassInfo GetClassInfo(this JsonSerializerOptions options, Type type)
        {
            return GetState(options).Classes.GetOrAdd(type, x => new JsonClassInfo(x, options));
        }

        internal static MemberAccessor GetMemberAccessor(this JsonSerializerOptions options)
        {
            return GetState(options).MemberAccessor;
        }

        private static JsonApiStateConverter GetState(JsonSerializerOptions options)
        {
            var state = options.GetConverter(typeof(JsonApiStateConverter)) as JsonApiStateConverter;

            if (state == null)
            {
                throw new JsonApiException("JSON:API extensions not initialized, please call use 'AddJsonApi' on 'JsonSerializerOptions' first");
            }

            return state;
        }
    }
}
