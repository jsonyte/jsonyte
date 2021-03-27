using System;
using System.Reflection;

namespace JsonApi.Serialization
{
    internal interface IMemberAccessor
    {
        Func<object?> CreateCreator(Type type);

        Func<object[], object?> CreateParameterizedCreator(ConstructorInfo constructor);

        Func<object, T> CreatePropertyGetter<T>(PropertyInfo property);

        Action<object, T> CreatePropertySetter<T>(PropertyInfo property);

        Func<object, T> CreateFieldGetter<T>(FieldInfo field);

        Action<object, T> CreateFieldSetter<T>(FieldInfo field);
    }
}
