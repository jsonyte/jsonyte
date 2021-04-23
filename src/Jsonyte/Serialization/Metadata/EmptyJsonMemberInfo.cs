using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonyte.Serialization.Metadata
{
    internal class EmptyJsonMemberInfo : JsonMemberInfo
    {
        public EmptyJsonMemberInfo()
        {
            Name = string.Empty;
            NameEncoded = default;
            MemberName = string.Empty;
            MemberType = typeof(string);
            Ignored = true;
            Converter = null!;
            IsRelationship = false;
        }

        public override string Name { get; }

        public override string MemberName { get; }

        public override Type MemberType { get; }

        public override JsonEncodedText NameEncoded { get; }

        public override JsonConverter Converter { get; }

        public override bool Ignored { get; }

        public override bool IsRelationship { get; }

        public override object? GetValue(object resource)
        {
            return null;
        }

        public override void Read(ref Utf8JsonReader reader, object resource)
        {
            reader.Skip();
        }

        public override void ReadRelationship(ref Utf8JsonReader reader, ref TrackedResources tracked, object resource)
        {
            reader.Skip();
        }

        public override bool Write(Utf8JsonWriter writer, ref TrackedResources tracked, object resource, JsonEncodedText section = default)
        {
            return false;
        }

        public override void WriteRelationship(Utf8JsonWriter writer, ref TrackedResources tracked, object resource, ref bool wroteSection)
        {
        }

        public override void WriteRelationshipWrapped(Utf8JsonWriter writer, ref TrackedResources tracked, object resource)
        {
        }

        public override void SetValue(object resource, object? value)
        {
        }
    }
}
