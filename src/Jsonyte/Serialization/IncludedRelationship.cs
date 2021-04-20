using System.Text.Json;

namespace Jsonyte.Serialization
{
    internal ref struct IncludedRelationship
    {
        public JsonEncodedText Name;

        public int Id;

        public IncludedRelationship(JsonEncodedText name, int id)
        {
            Name = name;
            Id = id;
        }
    }
}
