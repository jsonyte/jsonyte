using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Serialization
{
    internal sealed class JsonPropertyInfo<T> : IJsonPropertyInfo
    {
        public JsonPropertyInfo(PropertyInfo property, JsonConverter converter, JsonSerializerOptions options)
        {
            Options = options;
            TypedConverter = converter as JsonConverter<T>;
            Get = options.GetMemberAccessor().CreatePropertyGetter<T>(property);
            Set = options.GetMemberAccessor().CreatePropertySetter<T>(property);
            PropertyName = GetPropertyName(property, options);
        }

        public JsonSerializerOptions Options { get; }

        public JsonConverter<T> TypedConverter { get; }

        public Func<object, T> Get { get; }

        public Action<object, T> Set { get; }

        public string PropertyName { get; }

        public void Read(ref Utf8JsonReader reader, object resource)
        {
            var value = TypedConverter.Read(ref reader, typeof(T), Options);

            Set(resource, value);
        }

        public void Write(Utf8JsonWriter writer, object resource)
        {
            var value = Get(resource);

            writer.WritePropertyName(PropertyName);
            TypedConverter.Write(writer, value, Options);
        }

        private string GetPropertyName(PropertyInfo property, JsonSerializerOptions options)
        {
            var attribute = property.GetCustomAttribute<JsonPropertyNameAttribute>(false);

            if (attribute != null)
            {
                return attribute.Name;
            }

            if (options.PropertyNamingPolicy != null)
            {
                return options.PropertyNamingPolicy.ConvertName(property.Name);
            }

            return property.Name;
        }
    }
}
