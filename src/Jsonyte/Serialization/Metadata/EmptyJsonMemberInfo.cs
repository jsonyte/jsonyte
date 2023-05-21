using System;
using System.Text.Json;
using Jsonyte.Converters;
using Jsonyte.Validation;

namespace Jsonyte.Serialization.Metadata
{
    internal class EmptyJsonMemberInfo : JsonMemberInfo
    {
        public EmptyJsonMemberInfo(JsonSerializerOptions options)
        {
            Options = options;
        }

        public override string Name { get; } = string.Empty;

        public override Type MemberType { get; } = typeof(string);

        public override JsonEncodedText NameEncoded { get; } = default;

        public override bool Ignored { get; } = true;

        public override bool IsRelationship { get; } = false;

        public JsonSerializerOptions Options { get; }

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
            reader.ReadRelationship();

            var state = new RelationshipState();

            while (reader.IsInObject())
            {
                var name = reader.ReadMember(ref state);

                if (name == RelationshipFlags.Data)
                {
                    if (reader.IsObject())
                    {
                        var identifier = reader.ReadResourceIdentifier();

                        AddEmptyTrackedObject(identifier, ref tracked);
                    }
                    else if (reader.IsArray())
                    {
                        reader.ReadArray(JsonApiArrayCode.Relationships);

                        while (reader.IsInArray())
                        {
                            var identifier = reader.ReadResourceIdentifier();

                            AddEmptyTrackedObject(identifier, ref tracked);

                            reader.Read();
                        }
                    }
                    else if (reader.TokenType != JsonTokenType.Null)
                    {
                        throw new JsonApiFormatException("Expected null, empty array, or resource linkage for relationship");
                    }
                }
                else
                {
                    reader.Skip();
                }

                reader.Read();
            }

            state.Validate();
        }

        private void AddEmptyTrackedObject(scoped ResourceIdentifier identifier, ref TrackedResources tracked)
        {
            var id = identifier.Id;
            var type = identifier.Type;

            var idString = id.GetString();
            var typeString = type.GetString();

            tracked.SetIncluded(identifier, idString, typeString, EmptyJsonObjectConverter.Default, null!);
        }

        public override void ReadRelationshipWrapped(ref Utf8JsonReader reader, ref TrackedResources tracked, object resource)
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
