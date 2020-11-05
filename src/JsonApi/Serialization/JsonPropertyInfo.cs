using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Serialization
{
    internal abstract class JsonPropertyInfo
    {
        public abstract JsonConverter Converter { get; }

        public abstract object GetValue(object value);

        public abstract void Read(ref Utf8JsonReader reader, object resource);
    }

    internal sealed class JsonPropertyInfo<T> : JsonPropertyInfo
    {
        public JsonPropertyInfo(PropertyInfo property, JsonConverter converter, JsonSerializerOptions options)
        {
            Converter = converter;
            Options = options;
            TypedConverter = converter as JsonConverter<T>;
            Get = options.GetMemberAccessor().CreatePropertyGetter<T>(property);
            Set = options.GetMemberAccessor().CreatePropertySetter<T>(property);
        }

        public override JsonConverter Converter { get; }

        public JsonSerializerOptions Options { get; }

        public JsonConverter<T> TypedConverter { get; }

        public Func<object, T> Get { get; }

        public Action<object, T> Set { get; }

        public override object GetValue(object value)
        {
            return Get(value);
        }

        public override void Read(ref Utf8JsonReader reader, object resource)
        {
            var value = TypedConverter.Read(ref reader, typeof(T), Options);

            Set(resource, value);
        }
    }
}
