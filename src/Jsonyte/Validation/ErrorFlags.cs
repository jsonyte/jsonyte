using System;

namespace Jsonyte.Validation
{
    [Flags]
    internal enum ErrorFlags : byte
    {
        None = 0,
        Id = 1,
        Links = 2,
        Status = 4,
        Code = 8,
        Title = 16,
        Detail = 32,
        Source = 64,
        Meta = 128
    }
}
