namespace Jsonyte.Validation
{
    internal static class DocumentFlagsExtensions
    {
        public static bool IsSet(this DocumentFlags flags, DocumentFlags value)
        {
            return (flags & value) == value;
        }
    }
}
