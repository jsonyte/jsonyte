using System;

namespace Jsonyte.Converters
{
    internal abstract class WrappedResourceJsonConverter<T> : WrappedJsonConverter<T>
    {
        public virtual Type ElementType { get; } = typeof(T);
    }
}
