using System;

namespace JsonApi.Validation
{
    [Flags]
    internal enum JsonApiDocumentFlags
    {
        None = 0,
        Data = 1,
        Errors = 2,
        Meta = 4,
        Jsonapi = 8,
        Links = 16,
        Included = 32
    }
}
