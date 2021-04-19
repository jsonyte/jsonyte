using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonyte.Serialization
{
    internal class JsonTypeInfo
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private const int CachedMembers = 64;

        private static readonly EmptyJsonMemberInfo EmptyMember = new();

        private static readonly EmptyJsonParameterInfo EmptyParameter = new(-1);

        private readonly Dictionary<string, IJsonMemberInfo> nameCache;

        private readonly Dictionary<string, IJsonParameterInfo> parameterCache;

        private readonly MemberRef[] memberCache;

        public JsonTypeInfo(Type type, JsonSerializerOptions options)
        {
            var constructor = GetConstructor(type);

            Creator = options.GetMemberAccessor().CreateCreator(type);
            CreatorWithArguments = options.GetMemberAccessor().CreateParameterizedCreator(constructor);

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
            parameterCache = GetParameters(constructor, members, options);

            AttributeMembers = members
                .Where(x => !x.Name.Equals(JsonApiMembers.Id, StringComparison.OrdinalIgnoreCase) &&
                            !x.Name.Equals(JsonApiMembers.Type, StringComparison.OrdinalIgnoreCase) &&
                            !x.Name.Equals(JsonApiMembers.Meta, StringComparison.OrdinalIgnoreCase) &&
                            !x.Name.Equals(JsonApiMembers.Links, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            memberCache = members
                .Take(CachedMembers)
                .Select(x => new MemberRef(GetKey(x.NameEncoded.EncodedUtf8Bytes), x, x.NameEncoded.EncodedUtf8Bytes.ToArray()))
                .ToArray();

            IdMember = GetMember(JsonApiMembers.IdEncoded.EncodedUtf8Bytes);
            TypeMember = GetMember(JsonApiMembers.TypeEncoded.EncodedUtf8Bytes);
            DataMember = GetMember(JsonApiMembers.DataEncoded.EncodedUtf8Bytes);
            MetaMember = GetMember(JsonApiMembers.MetaEncoded.EncodedUtf8Bytes);
            LinksMember = GetMember(JsonApiMembers.LinksEncoded.EncodedUtf8Bytes);
        }

        public Func<object?> Creator { get; }

        public Func<object[], object?> CreatorWithArguments { get; }

        public IJsonMemberInfo[] AttributeMembers { get; }

        public IJsonMemberInfo IdMember { get; }

        public IJsonMemberInfo TypeMember { get; }

        public IJsonMemberInfo DataMember { get; }

        public IJsonMemberInfo MetaMember { get; }

        public IJsonMemberInfo LinksMember { get; }

        public IJsonMemberInfo GetMember(ReadOnlySpan<byte> name)
        {
            if (name.IsEmpty)
            {
                return EmptyMember;
            }

            var key = GetKey(name);

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

        public IJsonParameterInfo? GetParameter(string? name)
        {
            if (name == null)
            {
                return null;
            }

            parameterCache.TryGetValue(name, out var parameter);

            return parameter;
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
                    yield return CreateMemberInfo(typeof(JsonPropertyInfo<>), property, property.PropertyType, ignoreCondition, options);
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
                    yield return CreateMemberInfo(typeof(JsonFieldInfo<>), field, field.FieldType, ignoreCondition, options);
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

        private Dictionary<string, IJsonParameterInfo> GetParameters(ConstructorInfo constructor, IJsonMemberInfo[] members, JsonSerializerOptions options)
        {
            var membersByName = members.ToDictionary(x => x.Name, options.GetPropertyComparer());

            var parameters = constructor.GetParameters();

            var jsonParameters = new Dictionary<string, IJsonParameterInfo>(parameters.Length, options.GetPropertyComparer());

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

                    if (parameterInfo is not IJsonParameterInfo jsonParameter)
                    {
                        throw new JsonApiException($"Cannot get constructor parameter '{parameter.Name}' for '{constructor.DeclaringType}'");
                    }

                    jsonParameters[property.Name] = jsonParameter;
                }
            }

            return jsonParameters;
        }

        /// <summary>
        /// Takes the first 7 bytes from the name plus the length.
        ///
        /// Code is borrowed from JsonClassInfo in System.Text.Json.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong GetKey(ReadOnlySpan<byte> span)
        {
            var length = span.Length;
            ref var reference = ref MemoryMarshal.GetReference(span);

            if (length > 7)
            {
                var key = Unsafe.ReadUnaligned<ulong>(ref reference) & 0x00ffffffffffffffL;
                key |= (ulong) Math.Min(length, 0xff) << 56;

                return key;
            }

            if (length == 7)
            {
                var key = (ulong) Unsafe.ReadUnaligned<uint>(ref reference);
                key |= (ulong) Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref reference, 4)) << 32;
                key |= (ulong) Unsafe.Add(ref reference, 6) << 48;
                key |= (ulong) length << 56;

                return key;
            }

            if (length == 6)
            {
                var key = (ulong) Unsafe.ReadUnaligned<uint>(ref reference);
                key |= (ulong) Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref reference, 4)) << 32;
                key |= (ulong) length << 56;

                return key;
            }

            if (length == 5)
            {
                var key = (ulong) Unsafe.ReadUnaligned<uint>(ref reference);
                key |= (ulong) Unsafe.Add(ref reference, 4) << 32;
                key |= (ulong) length << 56;

                return key;
            }

            if (length == 4)
            {
                var key = (ulong) Unsafe.ReadUnaligned<uint>(ref reference);
                key |= (ulong) length << 56;

                return key;
            }

            if (length == 3)
            {
                var key = (ulong) Unsafe.ReadUnaligned<ushort>(ref reference);
                key |= (ulong) Unsafe.Add(ref reference, 2) << 16;
                key |= (ulong) length << 56;

                return key;
            }

            if (length == 2)
            {
                var key = (ulong) Unsafe.ReadUnaligned<ushort>(ref reference);
                key |= (ulong) length << 56;

                return key;
            }

            if (length == 1)
            {
                var key = (ulong) reference;
                key |= (ulong) length << 56;

                return key;
            }

            return 0;
        }
    }
}
