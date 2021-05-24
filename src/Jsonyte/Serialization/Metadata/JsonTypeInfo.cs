using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonyte.Serialization.Metadata
{
    internal class JsonTypeInfo
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private const int CachedMembers = 64;

        private static readonly EmptyJsonMemberInfo EmptyMember = new();

#if CONSTRUCTOR_CONVERTER
        private static readonly EmptyJsonParameterInfo EmptyParameter = new(-1);
#endif

        private readonly Dictionary<string, JsonMemberInfo> nameCache;

#if CONSTRUCTOR_CONVERTER
        private readonly Dictionary<string, JsonParameterInfo> parameterCache;
#endif

        private readonly MemberRef[] memberCache;

        public JsonTypeInfo(Type type, JsonSerializerOptions options)
        {
            var constructor = GetConstructor(type);

            Creator = options.GetMemberAccessor().CreateCreator(type);
            CreatorWithArguments = options.GetMemberAccessor().CreateParameterizedCreator(constructor);
            HasCircularReferences = GetHasCircularReferences(type, type, options);

            var members = GetProperties(type, options)
                .Concat(GetFields(type, options))
                .ToArray();

            var duplicates = members
                .GroupBy(x => x.Name)
                .Any(x => x.Count() > 1);

            if (duplicates)
            {
                throw new InvalidOperationException($"Type contains duplicate property names: {type.FullName}");
            }

            nameCache = GetNameCache(members);

#if CONSTRUCTOR_CONVERTER
            parameterCache = GetParameters(constructor, members, options);
#endif

            AttributeMembers = members
                .Where(x => !x.Name.Equals(JsonApiMembers.Id, StringComparison.OrdinalIgnoreCase) &&
                            !x.Name.Equals(JsonApiMembers.Type, StringComparison.OrdinalIgnoreCase) &&
                            !x.Name.Equals(JsonApiMembers.Meta, StringComparison.OrdinalIgnoreCase) &&
                            !x.Name.Equals(JsonApiMembers.Links, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            memberCache = members
                .Take(CachedMembers)
                .Select(x => new MemberRef(x.NameEncoded.EncodedUtf8Bytes.GetKey(), x, x.NameEncoded.EncodedUtf8Bytes.ToArray()))
                .ToArray();

            IdMember = GetMember(JsonApiMembers.IdEncoded.EncodedUtf8Bytes);
            TypeMember = GetMember(JsonApiMembers.TypeEncoded.EncodedUtf8Bytes);
            DataMember = GetMember(JsonApiMembers.DataEncoded.EncodedUtf8Bytes);
            MetaMember = GetMember(JsonApiMembers.MetaEncoded.EncodedUtf8Bytes);
            LinksMember = GetMember(JsonApiMembers.LinksEncoded.EncodedUtf8Bytes);
        }

        public Func<object?> Creator { get; }

        public Func<object[], object?> CreatorWithArguments { get; }

        public bool HasCircularReferences { get; }

        public JsonMemberInfo[] AttributeMembers { get; }

        public JsonMemberInfo IdMember { get; }

        public JsonMemberInfo TypeMember { get; }

        public JsonMemberInfo DataMember { get; }

        public JsonMemberInfo MetaMember { get; }

        public JsonMemberInfo LinksMember { get; }

        public JsonMemberInfo GetMember(ReadOnlySpan<byte> name)
        {
            if (name.IsEmpty)
            {
                return EmptyMember;
            }

            var key = name.GetKey();

            foreach (var item in memberCache)
            {
                if (item.Key == key)
                {
                    if (name.Length < 8 || name.SequenceEqual(item.Name))
                    {
                        return item.Member;
                    }
                }
            }

            return nameCache.TryGetValue(name.GetString(), out var member)
                ? member
                : EmptyMember;
        }

#if CONSTRUCTOR_CONVERTER
        public JsonParameterInfo? GetParameter(string? name)
        {
            if (name == null)
            {
                return null;
            }

            parameterCache.TryGetValue(name, out var parameter);

            return parameter;
        }
#endif

        private Dictionary<string, JsonMemberInfo> GetNameCache(JsonMemberInfo[] members)
        {
            return members.ToDictionary(x => x.MemberName, StringComparer.OrdinalIgnoreCase);
        }

        private IEnumerable<JsonMemberInfo> GetProperties(Type type, JsonSerializerOptions options)
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
                    yield return CreateMemberInfo(typeof(JsonPropertyInfo<>), property, property.PropertyType, ignoreCondition, options);
                }
            }
        }

        private IEnumerable<JsonMemberInfo> GetFields(Type type, JsonSerializerOptions options)
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
                    yield return CreateMemberInfo(typeof(JsonFieldInfo<>), field, field.FieldType, ignoreCondition, options);
                }
            }
        }

        private JsonIgnoreCondition? GetIgnoreCondition(MemberInfo member)
        {
            return member.GetCustomAttribute<JsonIgnoreAttribute>()?.Condition;
        }
        
        private JsonMemberInfo CreateMemberInfo(Type memberInfoType, MemberInfo member, Type memberType, JsonIgnoreCondition? ignoreCondition, JsonSerializerOptions options)
        {
            var fieldType = memberInfoType.MakeGenericType(memberType);
            var converter = GetConverter(member, memberType, options);

            var fieldInfo = Activator.CreateInstance(fieldType, member, ignoreCondition, converter, options);

            if (fieldInfo is not JsonMemberInfo jsonMemberInfo)
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

#if CONSTRUCTOR_CONVERTER
        private Dictionary<string, JsonParameterInfo> GetParameters(ConstructorInfo constructor, JsonMemberInfo[] members, JsonSerializerOptions options)
        {
            var membersByName = members.ToDictionary(x => x.Name, options.GetPropertyComparer());

            var parameters = constructor.GetParameters();

            var jsonParameters = new Dictionary<string, JsonParameterInfo>(parameters.Length, options.GetPropertyComparer());

            foreach (var parameter in parameters)
            {
                var property = membersByName.GetValueOrDefault(parameter.Name!, EmptyMember);

                if (property.Ignored)
                {
                    jsonParameters[property.Name] = EmptyParameter;
                }
                else
                {
                    var type = typeof(JsonParameterInfo<>).MakeGenericType(parameter.ParameterType);
                    var parameterInfo = Activator.CreateInstance(type, parameter, property, options);

                    if (parameterInfo is not JsonParameterInfo jsonParameter)
                    {
                        throw new JsonApiException($"Cannot get constructor parameter '{parameter.Name}' for '{constructor.DeclaringType}'");
                    }

                    jsonParameters[property.Name] = jsonParameter;
                }
            }

            return jsonParameters;
        }
#endif

        private bool GetHasCircularReferences(Type? type, Type parentType, JsonSerializerOptions options)
        {
            var types = new HashSet<Type>();

            return GetHasCircularReferences(type, parentType, types, options);
        }

        private bool GetHasCircularReferences(Type? type, Type parentType, HashSet<Type> types, JsonSerializerOptions options)
        {
            if (types.Count > 0 && type == parentType)
            {
                return true;
            }

            if (type == null || types.Contains(type))
            {
                return false;
            }

            types.Add(type);

            if (type.IsCollection())
            {
                return GetHasCircularReferences(type.GetCollectionElementType(), parentType, types, options);
            }

            var properties = type
                .GetProperties(Flags)
                .Where(x => !x.GetIndexParameters().Any())
                .Where(x => x.GetMethod?.IsPublic == true || x.SetMethod?.IsPublic == true);

            foreach (var property in properties)
            {
                if (property.PropertyType == parentType)
                {
                    return true;
                }

                if (!property.PropertyType.GetIsPrimitive())
                {
                    return GetHasCircularReferences(property.PropertyType, parentType, types, options);
                }
            }

            if (options.IncludeFields)
            {
                var fields = type
                    .GetFields(Flags)
                    .Where(x => x.IsPublic);

                foreach (var field in fields)
                {
                    if (field.FieldType == parentType)
                    {
                        return true;
                    }

                    if (!field.FieldType.GetIsPrimitive())
                    {
                        return GetHasCircularReferences(field.FieldType, parentType, types, options);
                    }
                }
            }

            return false;
        }
    }
}
