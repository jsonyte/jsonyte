using System;

namespace JsonApi.Validation
{
    [Flags]
    internal enum ResourceFlags
    {
        None = 0,
        Id = 1,
        Type = 2,
        Relationships = 4,
        Unknown = 8
    }
}
