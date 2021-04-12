﻿using System;

namespace JsonApi.Validation
{
    [Flags]
    internal enum ResourceFlags : byte
    {
        None = 0,
        Id = 1,
        Type = 2,
        Relationships = 4
    }
}
