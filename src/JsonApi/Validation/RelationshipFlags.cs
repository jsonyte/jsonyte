using System;

namespace JsonApi.Validation
{
    [Flags]
    internal enum RelationshipFlags
    {
        None = 0,
        Links = 1,
        Data = 2,
        Meta = 4,
        Unknown = 8
    }
}
