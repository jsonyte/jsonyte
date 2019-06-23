using System;
using System.Linq;
using System.Reflection;

namespace Jsonapi
{
    public static class TypeExtensions
    {
        public static bool IsDocument(this Type type)
        {
            return type.IsGenericType && typeof(JsonApiDocument<>).IsAssignableFrom(type.GetGenericTypeDefinition());
        }

        public static bool IsResource(this Type type)
        {
            var fields = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            return fields.Any(x => x.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase) ||
                                   x.Name.Equals("Type", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
