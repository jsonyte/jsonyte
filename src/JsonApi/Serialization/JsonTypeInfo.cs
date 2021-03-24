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
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly EmptyJsonMemberInfo EmptyMember = new();

        private readonly Dictionary<string, IJsonMemberInfo> properties;

        private readonly string[] keys;

        public JsonTypeInfo(Type type, JsonSerializerOptions options)
        {
            Creator = options.GetMemberAccessor().CreateCreator(type);
            TypeCategory = type.GetTypeCategory();
            
            properties = GetMemberCache(type, options);
            keys = properties.Keys.ToArray();
        }

        public Func<object?> Creator { get; }

        public JsonTypeCategory TypeCategory { get; }

        public IJsonMemberInfo GetMember(string? name)
        {
            if (name == null)
            {
                return EmptyMember;
            }

            return properties.TryGetValue(name, out var property)
                ? property
                : EmptyMember;
        }

        public string[] GetMemberKeys()
        {
            return keys;
        }

        private Dictionary<string, IJsonMemberInfo> GetMemberCache(Type type, JsonSerializerOptions options)
        {
            var comparer = options.PropertyNameCaseInsensitive
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal;

            var members = new Dictionary<string, IJsonMemberInfo>(comparer);

            GetProperties(members, type, options);
            GetFields(members, type, options);

            return members;
        }

        private void GetProperties(Dictionary<string, IJsonMemberInfo> members, Type type, JsonSerializerOptions options)
        {
            var typeProperties = type
                .GetProperties(Flags)
                .Where(x => !x.GetIndexParameters().Any())
                .Where(x => x.GetMethod?.IsPublic == true || x.SetMethod?.IsPublic == true);

            foreach (var property in typeProperties)
            {
                var ignoreCondition = GetIgnoreCondition(property);

                if (ignoreCondition != JsonIgnoreCondition.Always)
                {
                    var jsonProperty = CreateProperty(property, ignoreCondition, options);

                    members[jsonProperty.MemberName] = jsonProperty;
                }
            }
        }

        private void GetFields(Dictionary<string, IJsonMemberInfo> members, Type type, JsonSerializerOptions options)
        {
            if (!options.IncludeFields)
            {
                return;
            }

            var typeFields = type
                .GetFields(Flags)
                .Where(x => x.IsPublic);

            foreach (var field in typeFields)
            {
                var ignoreCondition = GetIgnoreCondition(field);

                if (ignoreCondition != JsonIgnoreCondition.Always)
                {
                    var jsonProperty = CreateField(field, ignoreCondition, options);

                    members[jsonProperty.MemberName] = jsonProperty;
                }
            }
        }

        private JsonIgnoreCondition? GetIgnoreCondition(MemberInfo member)
        {
            return member.GetCustomAttribute<JsonIgnoreAttribute>()?.Condition;
        }

        private IJsonMemberInfo CreateProperty(PropertyInfo property, JsonIgnoreCondition? ignoreCondition, JsonSerializerOptions options)
        {
            var propertyType = typeof(JsonPropertyInfo<>).MakeGenericType(property.PropertyType);
            var converter = GetConverter(property, property.PropertyType, options);

            var propertyInfo = Activator.CreateInstance(propertyType, property, ignoreCondition, converter, options);

            if (propertyInfo is not IJsonMemberInfo jsonMemberInfo)
            {
                throw new JsonApiException($"Cannot get property info for '{property.Name}'");
            }

            return jsonMemberInfo;
        }

        private IJsonMemberInfo CreateField(FieldInfo field, JsonIgnoreCondition? ignoreCondition, JsonSerializerOptions options)
        {
            var fieldType = typeof(JsonFieldInfo<>).MakeGenericType(field.FieldType);
            var converter = GetConverter(field, field.FieldType, options);

            var fieldInfo = Activator.CreateInstance(fieldType, field, ignoreCondition, converter, options);

            if (fieldInfo is not IJsonMemberInfo jsonMemberInfo)
            {
                throw new JsonApiException($"Cannot get field info for '{field.Name}'");
            }

            return jsonMemberInfo;
        }

        private JsonConverter? GetConverter(MemberInfo member, Type memberType, JsonSerializerOptions options)
        {
            var converter = GetConverterAttribute(member);

            if (converter == null)
            {
                return options.GetConverter(memberType);
            }

            if (converter.ConverterType == null)
            {
                return converter.CreateConverter(memberType);
            }

            return Activator.CreateInstance(converter.ConverterType) as JsonConverter;
        }

        private JsonConverterAttribute? GetConverterAttribute(MemberInfo member)
        {
            var converters = member.GetCustomAttributes<JsonConverterAttribute>(false).ToArray();

            if (!converters.Any())
            {
                return null;
            }

            if (converters.Length > 1)
            {
                throw new InvalidOperationException($"The attribute 'JsonConverterAttribute' cannot exist more than once on '{member}'.");
            }

            return converters.First();
        }
    }
}
