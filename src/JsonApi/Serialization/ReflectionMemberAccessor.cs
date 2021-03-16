using System;
using System.Reflection;

namespace JsonApi.Serialization
{
    internal class ReflectionMemberAccessor : IMemberAccessor
    {
        public Func<object?> CreateCreator(Type type)
        {
            return () => Activator.CreateInstance(type, false);
        }

        public Func<object, T?> CreatePropertyGetter<T>(PropertyInfo property)
        {
            return resource =>
            {
                var result = property.GetMethod?.Invoke(resource, null);

                return result == null
                    ? default
                    : (T) result;
            };
        }

        public Action<object, T?> CreatePropertySetter<T>(PropertyInfo property)
        {
            return (resource, value) => property.SetMethod?.Invoke(resource, new object?[] {value});
        }
    }
}
