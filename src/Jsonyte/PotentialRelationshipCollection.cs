using System.Text.Json;

namespace Jsonyte
{
    internal struct PotentialRelationshipCollection
    {
        public readonly JsonEncodedText Relationship;

        public readonly object? Value;

        public readonly bool WriteImmediately;

        public bool Written;

        public PotentialRelationshipCollection(JsonEncodedText relationship, object? value, bool writeImmediately)
        {
            Relationship = relationship;
            Value = value;
            WriteImmediately = writeImmediately;
            Written = false;
        }
    }
}
