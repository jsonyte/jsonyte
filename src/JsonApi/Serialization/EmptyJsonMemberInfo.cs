using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Serialization
{
    internal class EmptyJsonMemberInfo : IJsonMemberInfo
    {
        public string Name { get; } = string.Empty;

        public string MemberName { get; } = string.Empty;

        public Type MemberType { get; } = typeof(string);

        public Type? ElementType { get; } = null;

        public bool Ignored { get; } = true;

        public JsonConverter Converter { get; }

        public void Read(ref Utf8JsonReader reader, object resource)
        {
            reader.Skip();
        }

        public void ReadRelationship(ref Utf8JsonReader reader, ref TrackedResources tracked, object resource)
        {
            reader.Skip();
        }

        public void ReadExisting(ref Utf8JsonReader reader, ref TrackedResources tracked, object existingValue)
        {
            reader.Skip();
        }

        public object? Read(ref Utf8JsonReader reader)
        {
            return null;
        }

        public void Write(Utf8JsonWriter writer, object resource)
        {
        }

        public void Write(object resource, object? value)
        {
        }
    }
}
