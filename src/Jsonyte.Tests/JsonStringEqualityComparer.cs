using System.Collections.Generic;
using System.Text.Json;

namespace Jsonyte.Tests
{
    public class JsonStringEqualityComparer : IEqualityComparer<string>
    {
        public static JsonStringEqualityComparer Default { get; } = new();

        public bool Equals(string x, string y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return JsonElementEqualityComparer.Default.Equals(
                JsonDocument.Parse(x).RootElement,
                JsonDocument.Parse(y).RootElement);
        }

        public int GetHashCode(string obj)
        {
            return JsonDocument.Parse(obj).GetHashCode();
        }
    }
}
