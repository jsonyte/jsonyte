using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Serialization
{
    internal class JsonTypeInfo
    {
        private static readonly EmptyJsonMemberInfo EmptyMember = new();

        private readonly Dictionary<string, IJsonMemberInfo> properties;

        private readonly string[] keys;

        public JsonTypeInfo(Type type, JsonSerializerOptions options)
        {
            Creator = options.GetMemberAccessor().CreateCreator(type);
            TypeCategory = type.GetTypeCategory();

            properties = GetProperties(type, options);
            keys = properties.Keys.ToArray();
        }

        public Func<object?> Creator { get; }

        public JsonTypeCategory TypeCategory { get; }

        public IJsonMemberInfo GetProperty(string? name)
        {
            if (name == null)
            {
                return EmptyMember;
            }

            return properties.TryGetValue(name, out var property)
                ? property
                : EmptyMember;
        }

        public string[] GetPropertyKeys()
        {
            return keys;
        }

        private Dictionary<string, IJsonMemberInfo> GetProperties(Type type, JsonSerializerOptions options)
        {
            var comparer = options.PropertyNameCaseInsensitive
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal;

            var jsonProperties = new Dictionary<string, IJsonMemberInfo>(comparer);

            var typeProperties = type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => !x.GetIndexParameters().Any())
                .Where(x => x.GetMethod?.IsPublic == true || x.SetMethod?.IsPublic == true);

            foreach (var property in typeProperties)
            {
                var ignoreCondition = GetIgnoreCondition(property);

                if (ignoreCondition != JsonIgnoreCondition.Always)
                {
                    var jsonProperty = CreateProperty(property, ignoreCondition, options);

                    jsonProperties[jsonProperty.PropertyName] = jsonProperty;
                }
            }

            return jsonProperties;
        }

        private JsonIgnoreCondition? GetIgnoreCondition(MemberInfo member)
        {
            return member.GetCustomAttribute<JsonIgnoreAttribute>()?.Condition;
        }

        private IJsonMemberInfo CreateProperty(PropertyInfo property, JsonIgnoreCondition? ignoreCondition, JsonSerializerOptions options)
        {
            var propertyType = typeof(JsonPropertyInfo<>).MakeGenericType(property.PropertyType);
            var converter = GetConverter(property, options);

            var propertyInfo = Activator.CreateInstance(propertyType, property, ignoreCondition, converter, options);

            if (propertyInfo is not IJsonMemberInfo jsonPropertyInfo)
            {
                throw new JsonApiException($"Cannot get property info for '{property.Name}'");
            }

            return jsonPropertyInfo;
        }

        private JsonConverter? GetConverter(PropertyInfo property, JsonSerializerOptions options)
        {
            var converter = GetConverterAttribute(property);

            if (converter == null)
            {
                return options.GetConverter(property.PropertyType);
            }

            if (converter.ConverterType == null)
            {
                return converter.CreateConverter(property.PropertyType);
            }

            return Activator.CreateInstance(converter.ConverterType) as JsonConverter;
        }

        private JsonConverterAttribute? GetConverterAttribute(MemberInfo property)
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
    }
}
