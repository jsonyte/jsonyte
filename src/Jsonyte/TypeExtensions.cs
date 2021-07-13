using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jsonyte.Serialization.Contracts;
using Jsonyte.Serialization.Metadata;

namespace Jsonyte
{
    internal static class TypeExtensions
    {
        private const BindingFlags MemberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;

        public static bool IsResource(this Type type)
        {
            return HasMember(type, JsonApiMembers.Type);
        }

        public static bool IsResourceIdentifier(this Type type)
        {
            return HasMember(type, JsonApiMembers.Id) && HasMember(type, JsonApiMembers.Type);
        }

        public static bool IsRelationship(this Type type)
        {
            return type.IsResourceIdentifier() || type.IsResourceIdentifierCollection() || type.IsExplicitRelationshipByMembers();
        }

        public static bool IsResourceIdentifierCollection(this Type type)
        {
            if (type.IsCollection())
            {
                var elementType = type.GetCollectionElementType();

                if (elementType != null && elementType.IsResourceIdentifier())
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsError(this Type type)
        {
            return type == typeof(JsonApiError);
        }

        public static bool IsDocument(this Type type)
        {
            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                if (genericType == typeof(JsonApiDocument<>))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsRelationshipResource(this Type type)
        {
            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                if (genericType == typeof(RelationshipResource<>))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsInlineResource(this Type type)
        {
            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                if (genericType == typeof(InlineResource<>))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsExplicitRelationship(this Type type)
        {
            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                if (genericType == typeof(JsonApiRelationship<>))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsExplicitRelationshipByMembers(this Type type)
        {
            return HasMemberIgnoreType(type, JsonApiMembers.Data) || HasMemberIgnoreType(type, JsonApiMembers.Links);
        }

        public static bool IsCollection(this Type type)
        {
            return type.IsArray || JsonApiTypes.Enumerable.IsAssignableFrom(type);
        }

        public static MemberInfo? GetTypeMember(this Type type)
        {
            var property = type.GetProperty(JsonApiMembers.Type, MemberFlags);

            if (property != null && property.PropertyType == typeof(string))
            {
                return property;
            }

            var field = type.GetField(JsonApiMembers.Type, MemberFlags);

            if (field != null && field.FieldType == typeof(string))
            {
                return property;
            }

            return null;
        }

        public static JsonTypeCategory GetTypeCategory(this Type type)
        {
            if (!type.IsCollection())
            {
                return JsonTypeCategory.Object;
            }

            if (type.IsArray)
            {
                return JsonTypeCategory.Array;
            }

            return JsonTypeCategory.List;
        }

        public static Type? GetCollectionElementType(this Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            if (JsonApiTypes.Enumerable.IsAssignableFrom(type))
            {
                var genericType = GetInterfaces(type)
                    .Where(x => x.IsGenericType)
                    .FirstOrDefault(x => x.GetGenericTypeDefinition() == JsonApiTypes.EnumerableGeneric);

                return genericType?.GetGenericArguments().FirstOrDefault() ?? JsonApiTypes.Object;
            }

            return null;
        }

        public static bool GetIsPrimitive(this Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            return underlyingType.IsPrimitive ||
                   underlyingType == typeof(string) ||
                   underlyingType == typeof(decimal) ||
                   underlyingType == typeof(DateTime) ||
                   underlyingType == typeof(Guid) ||
                   underlyingType == typeof(TimeSpan);
        }

        private static bool HasMember(Type type, string name)
        {
            var property = type.GetProperty(name, MemberFlags);

            if (property != null && property.PropertyType == typeof(string))
            {
                return true;
            }

            var field = type.GetField(name, MemberFlags);

            return field != null && field.FieldType == typeof(string);
        }

        private static bool HasMemberIgnoreType(Type type, string name)
        {
            var property = type.GetProperty(name, MemberFlags);

            if (property != null)
            {
                return true;
            }

            var field = type.GetField(name, MemberFlags);

            return field != null;
        }

        private static IEnumerable<Type> GetInterfaces(Type type)
        {
            yield return type;

            foreach (var interfaceType in type.GetInterfaces())
            {
                yield return interfaceType;
            }
        }
    }
}
