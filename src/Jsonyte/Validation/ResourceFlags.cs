using System;

namespace Jsonyte.Validation
{
    [Flags]
    internal enum ResourceFlags : byte
    {
        None = 0,
        Id = 1,
        Type = 2,
        Relationships = 4,
        Meta = 8,
        Attributes = 16
    }
}
