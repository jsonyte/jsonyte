using System;

namespace Jsonapi
{
    [Flags]
    public enum DocumentFlags
    {
        None,
        Data,
        Errors,
        Meta
    }
}
