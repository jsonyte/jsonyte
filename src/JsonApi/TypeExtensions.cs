using System;
using System.Linq;
using System.Reflection;

namespace JsonApi
{
    internal static class TypeExtensions
    {
        public static bool IsResource(this Type type)
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            return properties.Any(x => x.Name.Equals("Type", StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsDocument(this Type type)
        {
            return type == typeof(JsonApiDocument) ||
                   type.IsGenericType && type.GetGenericTypeDefinition() == typeof(JsonApiDocument<>);
        }
    }
}
