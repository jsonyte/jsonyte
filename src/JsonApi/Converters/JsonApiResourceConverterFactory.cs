using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiResourceConverterFactory : JsonApiConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (IsDefaultType(typeToConvert))
            {
                return false;
            }

            return typeToConvert.IsResource();
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var info = options.GetClassInfo(typeToConvert);

            var converterType = info.ParameterCount == 0
                ? typeof(JsonApiResourceConverter<>)
                : typeof(JsonApiResourceConstructorConverter<>);

            return CreateConverter(converterType, typeToConvert);
        }
    }
}
