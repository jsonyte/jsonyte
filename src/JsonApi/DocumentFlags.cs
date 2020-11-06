using System;

namespace JsonApi
{
    [Flags]
    internal enum DocumentFlags
    {
        None = 0,
        Data = 1,
        Errors = 2,
        Meta = 4,
        Jsonapi = 8,
        Links = 16,
        Included = 32
    }
}
