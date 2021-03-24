using System;
using System.Text.Json;

namespace JsonApi.Serialization
{
    internal class EmptyJsonMemberInfo : IJsonMemberInfo
    {
        public string PropertyName { get; } = string.Empty;

        public Type PropertyType { get; } = typeof(object);

        public bool Ignored { get; } = true;

        public void Read(ref Utf8JsonReader reader, object resource)
        {
            reader.Skip();
        }

        public void Write(Utf8JsonWriter writer, object resource)
        {
        }
    }
}
