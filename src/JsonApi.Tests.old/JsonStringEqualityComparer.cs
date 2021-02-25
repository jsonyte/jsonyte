using System.Collections.Generic;
using System.Text.Json;

namespace JsonApi.Tests
{
    public class JsonStringEqualityComparer : IEqualityComparer<string>
    {
        public static JsonStringEqualityComparer Default { get; } = new();

        public bool Equals(string x, string y)
        {
            var documentX = JsonDocument.Parse(x);
            var documentY = JsonDocument.Parse(y);

            return JsonElementEqualityComparer.Default.Equals(documentX.RootElement, documentY.RootElement);
        }

        public int GetHashCode(string obj)
        {
            return JsonDocument.Parse(obj).GetHashCode();
        }
    }
}
