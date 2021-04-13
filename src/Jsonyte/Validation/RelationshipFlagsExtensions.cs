namespace Jsonyte.Validation
{
    internal static class RelationshipFlagsExtensions
    {
        public static bool IsSet(this RelationshipFlags flags, RelationshipFlags value)
        {
            return (flags & value) == value;
        }
    }
}
