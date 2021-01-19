using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Serialization
{
    internal class JsonClassInfo
    {
        public JsonClassInfo(Type type, JsonSerializerOptions options)
        {
            Options = options;
            Creator = options.GetMemberAccessor().CreateCreator(type);
            Properties = GetProperties(type);
            ClassType = GetClassType(type);
        }

        public JsonSerializerOptions Options { get; }

        public Func<object> Creator { get; }

        public Dictionary<string, IJsonPropertyInfo> Properties { get; }

        public JsonClassType ClassType { get; }

        private Dictionary<string, IJsonPropertyInfo> GetProperties(Type type)
        {
            var comparer = Options.PropertyNameCaseInsensitive
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal;

            return type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => !x.GetIndexParameters().Any())
                .Where(x => x.GetMethod?.IsPublic == true || x.SetMethod?.IsPublic == true)
                .Where(x => x.GetCustomAttribute<JsonIgnoreAttribute>() == null)
                .ToDictionary(GetPropertyName, CreateProperty, comparer);
        }

        private string GetPropertyName(PropertyInfo property)
        {
            var nameAttribute = property.GetCustomAttribute<JsonPropertyNameAttribute>(false);

            if (nameAttribute != null)
            {
                return nameAttribute.Name;
            }

            if (Options.PropertyNamingPolicy != null)
            {
                return Options.PropertyNamingPolicy.ConvertName(property.Name);
            }

            return property.Name;
        }

        private IJsonPropertyInfo CreateProperty(PropertyInfo property)
        {
            var propertyType = typeof(JsonPropertyInfo<>).MakeGenericType(property.PropertyType);
            var converter = GetConverter(property);

            return Activator.CreateInstance(propertyType, property, converter, Options) as IJsonPropertyInfo;
        }

        private JsonConverter GetConverter(PropertyInfo property)
        {
            var converter = GetConverterAttribute(property);

            if (converter == null)
            {
                return Options.GetConverter(property.PropertyType);
            }

            if (converter.ConverterType == null)
            {
                return converter.CreateConverter(property.PropertyType);
            }

            return Activator.CreateInstance(converter.ConverterType) as JsonConverter;
        }

        private JsonConverterAttribute GetConverterAttribute(PropertyInfo property)
        {
            var converters = property.GetCustomAttributes<JsonConverterAttribute>(false).ToArray();

            if (!converters.Any())
            {
                return null;
            }

            if (converters.Length > 1)
            {
                throw new InvalidOperationException($"The attribute 'JsonConverterAttribute' cannot exist more than once on '{property}'.");
            }

            return converters.First();
        }

        private JsonClassType GetClassType(Type type)
        {
            if (!type.IsCollection())
            {
                return JsonClassType.Object;
            }

            if (type.IsArray)
            {
                return JsonClassType.Array;
            }

            return JsonClassType.List;
        }
    }
}
