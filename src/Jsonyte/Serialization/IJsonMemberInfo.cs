using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonyte.Serialization
{
    internal interface IJsonMemberInfo
    {
        string Name { get; }

        JsonEncodedText NameEncoded { get; }

        string MemberName { get; }

        Type MemberType { get; }

        bool Ignored { get; }

        JsonConverter Converter { get; }

        bool IsRelationship { get; }

        object? GetValue(object resource);

        void Read(ref Utf8JsonReader reader, object resource);

        void ReadRelationship(ref Utf8JsonReader reader, ref TrackedResources tracked, object resource);

        bool Write(Utf8JsonWriter writer, ref TrackedResources tracked, object resource, JsonEncodedText section = default);

        void WriteRelationship(Utf8JsonWriter writer, ref TrackedResources tracked, object resource, ref bool wroteSection);

        void WriteRelationshipWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, object resource, ref bool wroteSection);

        void SetValue(object resource, object? value);
    }
}
