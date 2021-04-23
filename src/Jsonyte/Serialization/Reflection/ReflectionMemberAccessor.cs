using System;
using System.Reflection;

namespace Jsonyte.Serialization.Reflection
{
#if !NETCOREAPP && !NETFRAMEWORK
    internal class ReflectionMemberAccessor : MemberAccessor
    {
        public override Func<object?> CreateCreator(Type type)
        {
            return () => Activator.CreateInstance(type, false);
        }

        public override Func<object[], object?> CreateParameterizedCreator(ConstructorInfo constructor)
        {
            return constructor.Invoke;
        }

        public override Func<object, T> CreatePropertyGetter<T>(PropertyInfo property)
        {
            return resource => (T) property.GetMethod.Invoke(resource, null);
        }

        public override Action<object, T> CreatePropertySetter<T>(PropertyInfo property)
        {
            return (resource, value) => property.SetMethod.Invoke(resource, new object[] {value!});
        }

        public override Func<object, T> CreateFieldGetter<T>(FieldInfo field)
        {
            return resource => (T) field.GetValue(resource);
        }

        public override Action<object, T> CreateFieldSetter<T>(FieldInfo field)
        {
            return (resource, value) => field.SetValue(resource, value);
        }
    }
#endif
}
