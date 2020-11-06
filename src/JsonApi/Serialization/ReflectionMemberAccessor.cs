using System;
using System.Reflection;

namespace JsonApi.Serialization
{
    internal class ReflectionMemberAccessor : MemberAccessor
    {
        public override Func<object> CreateCreator(Type type)
        {
            return () => Activator.CreateInstance(type, false);
        }

        public override Func<object, T> CreatePropertyGetter<T>(PropertyInfo property)
        {
            return resource => (T) property.GetMethod.Invoke(resource, null);
        }

        public override Action<object, T> CreatePropertySetter<T>(PropertyInfo property)
        {
            return (resource, value) => property.SetMethod.Invoke(resource, new object[] {value});
        }
    }
}
