﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jsonyte.Serialization;

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

        public static bool IsRelationship(this Type type)
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

        public static bool IsCollection(this Type type)
        {
            return type.IsArray || typeof(IEnumerable).IsAssignableFrom(type);
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

            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                var genericType = GetInterfaces(type)
                    .Where(x => x.IsGenericType)
                    .FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(IEnumerable<>));

                return genericType?.GetGenericArguments().FirstOrDefault() ?? typeof(object);
            }

            return null;
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
