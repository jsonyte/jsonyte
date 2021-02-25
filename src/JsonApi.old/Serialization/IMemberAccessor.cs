using System;
using System.Reflection;

namespace JsonApi.Serialization
{
    internal interface IMemberAccessor
    {
        Func<object> CreateCreator(Type type);

        Func<object, T> CreatePropertyGetter<T>(PropertyInfo property);

        Action<object, T> CreatePropertySetter<T>(PropertyInfo property);
    }
}
