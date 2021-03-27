using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Serialization
{
    internal class JsonParameterInfo<T> : IJsonParameterInfo
    {
        public JsonParameterInfo(ParameterInfo parameter, IJsonMemberInfo member, JsonSerializerOptions options)
        {
            Position = parameter.Position;
            TypedConverter = (JsonConverter<T>) member.Converter;
            Options = options;
        }

        public int Position { get; }

        public JsonConverter<T> TypedConverter { get; }

        public JsonSerializerOptions Options { get; }

        public object? Read(ref Utf8JsonReader reader)
        {
            return TypedConverter.Read(ref reader, typeof(object), Options);
        }
    }
}
