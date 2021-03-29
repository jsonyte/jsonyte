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

        private static readonly EmptyJsonParameterInfo EmptyParameter = new(-1);

        private static readonly Type PropertyType = typeof(JsonPropertyInfo<>);

        private static readonly Type FieldType = typeof(JsonFieldInfo<>);

        private readonly Dictionary<string, IJsonMemberInfo> nameCache;

        private readonly Dictionary<string, IJsonParameterInfo> parameterCache;

        private readonly string[] keys;

        public JsonTypeInfo(Type type, JsonSerializerOptions options)
        {
            var constructor = GetConstructor(type);

            Creator = options.GetMemberAccessor().CreateCreator(type);
            CreatorWithArguments = options.GetMemberAccessor().CreateParameterizedCreator(constructor);
            TypeCategory = type.GetTypeCategory();

            var members = GetProperties(type, options)
                .Concat(GetFields(type, options))
                .ToArray();

            var memberCache = GetMemberCache(members, options);

            nameCache = GetNameCache(members);
            parameterCache = GetParameters(constructor, memberCache, options);

            keys = memberCache.Keys.ToArray();
        }

        public Func<object?> Creator { get; }

        public Func<object[], object?> CreatorWithArguments { get; }

        public JsonTypeCategory TypeCategory { get; }

        public int ParameterCount => parameterCache.Count;

        public int MemberCount => nameCache.Count;

        public IJsonMemberInfo GetMember(string? name)
        {
            if (name == null)
            {
                return EmptyMember;
            }

            return nameCache.TryGetValue(name, out var member)
                ? member
                : EmptyMember;
        }

        public IJsonParameterInfo? GetParameter(string? name)
        {
            if (name == null)
            {
                return null;
            }

            parameterCache.TryGetValue(name, out var parameter);

            return parameter;
        }

        public string[] GetMemberKeys()
        {
            return keys;
        }

        private Dictionary<string, IJsonMemberInfo> GetMemberCache(IJsonMemberInfo[] members, JsonSerializerOptions options)
        {
            return members.ToDictionary(x => x.Name, options.GetPropertyComparer());
        }

        private Dictionary<string, IJsonMemberInfo> GetNameCache(IJsonMemberInfo[] members)
        {
            return members.ToDictionary(x => x.MemberName, StringComparer.OrdinalIgnoreCase);
        }

        private IEnumerable<IJsonMemberInfo> GetProperties(Type type, JsonSerializerOptions options)
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
                    yield return CreateMemberInfo(PropertyType, property, property.PropertyType, ignoreCondition, options);
                }
            }
        }

        private IEnumerable<IJsonMemberInfo> GetFields(Type type, JsonSerializerOptions options)
        {
            if (!options.IncludeFields)
            {
                yield break;
            }

            var typeFields = type
                .GetFields(Flags)
                .Where(x => x.IsPublic);

            foreach (var field in typeFields)
            {
                var ignoreCondition = GetIgnoreCondition(field);

                if (ignoreCondition != JsonIgnoreCondition.Always)
                {
                    yield return CreateMemberInfo(FieldType, field, field.FieldType, ignoreCondition, options);
                }
            }
        }

        private JsonIgnoreCondition? GetIgnoreCondition(MemberInfo member)
        {
            return member.GetCustomAttribute<JsonIgnoreAttribute>()?.Condition;
        }
        
        private IJsonMemberInfo CreateMemberInfo(Type memberInfoType, MemberInfo member, Type memberType, JsonIgnoreCondition? ignoreCondition, JsonSerializerOptions options)
        {
            var fieldType = memberInfoType.MakeGenericType(memberType);
            var converter = GetConverter(member, memberType, options);

            var fieldInfo = Activator.CreateInstance(fieldType, member, ignoreCondition, converter, options);

            if (fieldInfo is not IJsonMemberInfo jsonMemberInfo)
            {
                throw new JsonApiException($"Cannot get type member info for '{member.Name}'");
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

        private ConstructorInfo GetConstructor(Type type)
        {
            var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var marked = constructors
                .Where(x => x.GetCustomAttribute<JsonConstructorAttribute>() != null)
                .ToArray();

            if (marked.Length > 1)
            {
                throw new JsonException($"Cannot have multiple constructors marked with JsonConstructorAttribute for type '{type}'");
            }

            var markedConstructor = marked.FirstOrDefault(x => x.IsPublic);

            if (markedConstructor != null)
            {
                return markedConstructor;
            }

            var publicConstructors = constructors
                .Where(x => x.IsPublic)
                .ToArray();

            var defaultConstructor = publicConstructors.FirstOrDefault(x => x.GetParameters().Length == 0);

            if (defaultConstructor != null)
            {
                return defaultConstructor;
            }

            if (publicConstructors.Length > 1)
            {
                throw new JsonException($"Cannot have multiple constructors with parameters for type '{type}'");
            }

            return publicConstructors.First();
        }

        private Dictionary<string, IJsonParameterInfo> GetParameters(ConstructorInfo constructor, Dictionary<string, IJsonMemberInfo> members, JsonSerializerOptions options)
        {
            var parameters = constructor.GetParameters();

            var jsonParameters = new Dictionary<string, IJsonParameterInfo>(parameters.Length, options.GetPropertyComparer());

            foreach (var parameter in parameters)
            {
                var property = members.GetValueOrDefault(parameter.Name!, EmptyMember);

                if (property.Ignored)
                {
                    jsonParameters[property.Name] = EmptyParameter;
                }
                else
                {
                    var type = typeof(JsonParameterInfo<>).MakeGenericType(parameter.ParameterType);
                    var parameterInfo = Activator.CreateInstance(type, parameter, property, options);

                    if (parameterInfo is not IJsonParameterInfo jsonParameter)
                    {
                        throw new JsonApiException($"Cannot get constructor parameter '{parameter.Name}' for '{constructor.DeclaringType}'");
                    }

                    jsonParameters[property.Name] = jsonParameter;
                }
            }

            return jsonParameters;
        }
    }
}
