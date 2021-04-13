using System;

namespace JsonApi.Validation
{
    [Flags]
    internal enum RelationshipFlags : byte
    {
        None = 0,
        Links = 1,
        Data = 2,
        Meta = 4
    }
}
