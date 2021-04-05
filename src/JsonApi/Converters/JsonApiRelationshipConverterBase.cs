using System;
using System.Text.Json;

namespace JsonApi.Converters
{
    internal abstract class JsonApiRelationshipConverterBase<T> : JsonApiConverter<T>
    {
        public abstract T? Read(ref Utf8JsonReader reader, ref JsonApiState state, Type typeToConvert, JsonSerializerOptions options);
    }
}
