namespace Jsonyte.Tests.Models
{
#if NET5_0_OR_GREATER
    public record ModelRecord(string Id, string Type, string Title)
    {
    }
#endif
}
