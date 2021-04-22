using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonyte.Serialization.Metadata
{
    internal abstract class JsonParameterInfo
    {
        public abstract int Position { get; }

        public abstract object? Read(ref Utf8JsonReader reader);
    }

    internal class JsonParameterInfo<T> : JsonParameterInfo
    {
        public JsonParameterInfo(ParameterInfo parameter, JsonMemberInfo member, JsonSerializerOptions options)
        {
            Position = parameter.Position;
            TypedConverter = (JsonConverter<T>) member.Converter;
            Options = options;
        }

        public override int Position { get; }

        public JsonConverter<T> TypedConverter { get; }

        public JsonSerializerOptions Options { get; }

        public override object? Read(ref Utf8JsonReader reader)
        {
            return TypedConverter.Read(ref reader, JsonApiTypes.Object, Options);
        }
    }
}
