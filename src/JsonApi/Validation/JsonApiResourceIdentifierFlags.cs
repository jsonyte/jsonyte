using System;

namespace JsonApi.Validation
{
    [Flags]
    internal enum JsonApiResourceIdentifierFlags
    {
        None = 0,
        Id = 1,
        Type = 2,
        Unknown = 4
    }
}
