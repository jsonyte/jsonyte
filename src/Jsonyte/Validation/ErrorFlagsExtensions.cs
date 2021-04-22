namespace Jsonyte.Validation
{
    internal static class ErrorFlagsExtensions
    {
        public static bool IsSet(this ErrorFlags flags, ErrorFlags value)
        {
            return (flags & value) == value;
        }
    }
}
