using System;
using System.Text.Json;
using JsonApi.Converters;
using JsonApi.Serialization;

namespace JsonApi
{
    public static class JsonSerializationOptionsExtensions
    {
        public static void AddJsonApi(this JsonSerializerOptions options)
        {
            options.Converters.Add(new JsonApiStateConverter());
            options.Converters.Add(new JsonApiConverterFactory());
        }

        internal static JsonClassInfo GetClassInfo(this JsonSerializerOptions options, Type type)
        {
            var state = options.GetConverter(typeof(JsonApiStateConverter)) as JsonApiStateConverter;

            if (state == null)
            {
                throw new JsonApiException("JSON:API extensions not initialized, please call use 'AddJsonApi' on 'JsonSerializerOptions' first");
            }

            return state.Classes.GetOrAdd(type, x => new JsonClassInfo(x, options));
        }
    }
}
