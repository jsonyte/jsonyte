using System;
using System.Reflection;

namespace Jsonyte.Serialization.Reflection
{
    internal abstract class MemberAccessor
    {
        public abstract Func<object?> CreateCreator(Type type);

        public abstract Func<object[], object?> CreateParameterizedCreator(ConstructorInfo constructor);

        public abstract Func<object, T> CreatePropertyGetter<T>(PropertyInfo property);

        public abstract Action<object, T> CreatePropertySetter<T>(PropertyInfo property);

        public abstract Func<object, T> CreateFieldGetter<T>(FieldInfo field);

        public abstract Action<object, T> CreateFieldSetter<T>(FieldInfo field);
    }
}
