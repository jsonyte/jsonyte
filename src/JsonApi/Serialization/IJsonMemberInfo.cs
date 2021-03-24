using System;
using System.Text.Json;

namespace JsonApi.Serialization
{
    internal interface IJsonMemberInfo
    {
        string PropertyName { get; }

        Type PropertyType { get; }

        bool Ignored { get; }

        void Read(ref Utf8JsonReader reader, object resource);

        void Write(Utf8JsonWriter writer, object resource);
    }
}
