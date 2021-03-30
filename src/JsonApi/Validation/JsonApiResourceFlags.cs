using System;

namespace JsonApi.Validation
{
    [Flags]
    internal enum JsonApiResourceFlags
    {
        None = 0,
        Id = 1,
        Type = 2,
        Unknown = 4
    }
}
