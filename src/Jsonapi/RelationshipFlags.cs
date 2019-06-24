using System;

namespace Jsonapi
{
    [Flags]
    public enum RelationshipFlags
    {
        None,
        Links,
        Data,
        Meta
    }
}
