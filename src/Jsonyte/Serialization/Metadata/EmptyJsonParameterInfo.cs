#if CONSTRUCTOR_CONVERTER
using System.Text.Json;

namespace Jsonyte.Serialization.Metadata
{
    internal class EmptyJsonParameterInfo : JsonParameterInfo
    {
        public EmptyJsonParameterInfo(int position)
        {
            Position = position;
        }

        public override int Position { get; }

        public override object? Read(ref Utf8JsonReader reader)
        {
            return null;
        }
    }
}
#endif
