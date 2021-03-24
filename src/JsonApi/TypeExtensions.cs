using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JsonApi.Serialization;

namespace JsonApi
{
    internal static class TypeExtensions
    {
        public static bool IsResource(this Type type)
        {
            var property = type.GetProperty("Type", BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            return property?.PropertyType == typeof(string);
        }

        public static bool IsError(this Type type)
        {
            return type == typeof(JsonApiError);
        }

        public static bool IsDocument(this Type type)
        {
            if (type == typeof(JsonApiDocument))
            {
                return true;
            }

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

        public static bool IsCollection(this Type type)
        {
            if (type == typeof(string))
            {
                return false;
            }

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

        public static Type? GetCollectionType(this Type type)
        {
            if (type == typeof(string))
            {
                return null;
            }

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
