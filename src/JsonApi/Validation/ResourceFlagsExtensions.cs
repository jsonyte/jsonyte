namespace JsonApi.Validation
{
    internal static class ResourceFlagsExtensions
    {
        public static bool IsSet(this ResourceFlags flags, ResourceFlags value)
        {
            return (flags & value) == value;
        }
    }
}
