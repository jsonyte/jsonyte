using System.Text.Json;

namespace Jsonyte.Serialization
{
    internal readonly struct RelationshipRef
    {
        public readonly ulong Key;

        public readonly JsonEncodedText Name;

        public readonly int Id;

        public RelationshipRef(ulong key, JsonEncodedText name, int id)
        {
            Key = key;
            Name = name;
            Id = id;
        }
    }
}
