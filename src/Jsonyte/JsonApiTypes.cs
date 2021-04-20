using System;
using System.Collections;
using System.Collections.Generic;

namespace Jsonyte
{
    internal static class JsonApiTypes
    {
        public static readonly Type Object = typeof(object);

        public static readonly Type Enumerable = typeof(IEnumerable);

        public static readonly Type EnumerableGeneric = typeof(IEnumerable<>);
    }
}
