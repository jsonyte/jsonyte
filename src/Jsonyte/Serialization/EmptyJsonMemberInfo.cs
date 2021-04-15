using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonyte.Serialization
{
    internal class EmptyJsonMemberInfo : IJsonMemberInfo
    {
        public string Name { get; } = string.Empty;

        public JsonEncodedText NameEncoded { get; } = default;

        public string MemberName { get; } = string.Empty;

        public Type MemberType { get; } = typeof(string);

        public bool Ignored { get; } = true;

        public JsonConverter Converter { get; }

        public bool IsRelationship { get; } = false;

        public object? GetValue(object resource)
        {
            return null;
        }

        public void Read(ref Utf8JsonReader reader, object resource)
        {
            reader.Skip();
        }

        public void ReadRelationship(ref Utf8JsonReader reader, ref TrackedResources tracked, object resource)
        {
            reader.Skip();
        }

        public object? Read(ref Utf8JsonReader reader)
        {
            return null;
        }

        public void Write(Utf8JsonWriter writer, ref TrackedResources tracked, object resource)
        {
        }

        public void WriteRelationship(Utf8JsonWriter writer, ref TrackedResources tracked, object resource, ref bool wroteSection)
        {
        }

        public void WriteRelationshipWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, object resource)
        {
        }

        public void SetValue(object resource, object? value)
        {
        }
    }
}
