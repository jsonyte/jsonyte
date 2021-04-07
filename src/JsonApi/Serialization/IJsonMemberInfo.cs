using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonApi.Serialization
{
    internal interface IJsonMemberInfo
    {
        string Name { get; }

        string MemberName { get; }

        Type MemberType { get; }

        bool Ignored { get; }

        JsonConverter Converter { get; }

        IJsonValueConverter? RelationshipConverter { get; }

        void Read(ref Utf8JsonReader reader, object resource);

        void ReadRelationship(ref Utf8JsonReader reader, ref TrackedResources tracked, object resource);

        object? Read(ref Utf8JsonReader reader);

        void Write(Utf8JsonWriter writer, object resource);

        void Write(object resource, object? value);
    }
}
