using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Serialization;

namespace JsonApi.Converters
{
    public class ResourceJsonConverter<T> : JsonConverter<T>
    {
        private static readonly ConcurrentDictionary<Type, JsonPropertyInfo[]> PropertiesCache =
            new ConcurrentDictionary<Type, JsonPropertyInfo[]>();

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var document = JsonSerializer.Deserialize<JsonApiDocument>(ref reader, options);

            var properties = GetProperties(typeToConvert);

            var resource = Activator.CreateInstance(typeToConvert);

            foreach (var property in properties)
            {
                if (!property.HasSetter)
                {
                    continue;
                }

                if (property.PublicName == "id")
                {
                    property.SetValueAsObject(resource, document.Data.Id);
                }
                else if (property.PublicName == "type")
                {
                    property.SetValueAsObject(resource, document.Data.Type);
                }
                else if (document.Data.Attributes.TryGetValue(property.PublicName, out var value))
                {
                    SetValue(property, resource, value);
                }
            }

            return (T) resource;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        private JsonPropertyInfo[] GetProperties(Type type)
        {
            if (!PropertiesCache.TryGetValue(type, out var properties))
            {
                properties = PropertiesCache.GetOrAdd(type, x => CreatePropertyInfos(type));
            }

            return properties;
        }

        private JsonPropertyInfo[] CreatePropertyInfos(Type type)
        {
            return type
                .GetProperties()
                .Select(x => CreatePropertyInfo(type, x))
                .ToArray();
        }

        private JsonPropertyInfo CreatePropertyInfo(Type type, PropertyInfo property)
        {
            var infoType = typeof(ReflectionJsonPropertyInfo<,>).MakeGenericType(type, property.PropertyType);

            return (JsonPropertyInfo) Activator.CreateInstance(infoType, property);
        }

        private void SetValue(JsonPropertyInfo property, object resource, object value)
        {
            var element = (JsonElement) value;

            if (property.PropertyType == typeof(string))
            {
                property.SetValueAsObject(resource, element.GetString());
            }
            else if (property.PropertyType == typeof(int))
            {
                property.SetValueAsObject(resource, element.GetInt32());
            }
        }
    }
}