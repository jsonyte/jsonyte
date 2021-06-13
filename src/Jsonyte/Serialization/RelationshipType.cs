namespace Jsonyte.Serialization
{
    internal enum RelationshipType : byte
    {
        None,
        Declared,
        Object,
        TypedCollection,
        PotentialCollection
    }
}
