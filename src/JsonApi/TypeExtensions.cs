using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JsonApi
{
    internal static class TypeExtensions
    {
        public static bool IsResource(this Type type)
        {
            var typeProperty = type.GetProperty("Type", BindingFlags.Instance | BindingFlags.Public);

            return typeProperty?.PropertyType == typeof(string);
        }

        public static bool IsDocument(this Type type)
        {
            return type == typeof(JsonApiDocument) ||
                   type.IsGenericType && type.GetGenericTypeDefinition() == typeof(JsonApiDocument<>);
        }

        public static bool IsCollection(this Type type)
        {
            if (type == typeof(string))
            {
                return false;
            }

            return type.IsArray || typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static Type GetCollectionType(this Type type)
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
