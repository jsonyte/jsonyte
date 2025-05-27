using System.Text.Json;

namespace Jsonyte.Serialization.Contracts
{
    internal readonly struct PotentialRelationshipCollection
    {
        public readonly JsonEncodedText Relationship;

        public readonly object? Value;

        public readonly bool WriteImmediately;

        public readonly RelationshipSerializationType RelationshipSerializationType;

        public PotentialRelationshipCollection(JsonEncodedText relationship, object? value, bool writeImmediately, RelationshipSerializationType relationshipSerializationType)
        {
            Relationship = relationship;
            Value = value;
            WriteImmediately = writeImmediately;
            RelationshipSerializationType = relationshipSerializationType;
        }
    }
}
