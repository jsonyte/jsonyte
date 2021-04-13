using System.Text.Json;

namespace Jsonyte.Serialization
{
    internal class EmptyJsonParameterInfo : IJsonParameterInfo
    {
        public EmptyJsonParameterInfo(int position)
        {
            Position = position;
        }

        public int Position { get; }

        public object? Read(ref Utf8JsonReader reader)
        {
            return null;
        }
    }
}
