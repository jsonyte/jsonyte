using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace JsonApi.Tests
{
    public class JsonElementEqualityComparer : IEqualityComparer<JsonElement>
    {
        public static JsonElementEqualityComparer Default { get; } = new();

        public bool Equals(JsonElement x, JsonElement y)
        {
            if (x.ValueKind != y.ValueKind)
            {
                return false;
            }
            
            if (x.ValueKind == JsonValueKind.Number)
            {
                return x.GetRawText() == y.GetRawText();
            }

            if (x.ValueKind == JsonValueKind.String)
            {
                return x.GetString() == y.GetString();
            }

            if (x.ValueKind == JsonValueKind.Array)
            {
                return x.EnumerateArray()
                    .SequenceEqual(y.EnumerateArray(), this);
            }

            if (x.ValueKind == JsonValueKind.Object)
            {
                var propertiesX = x.EnumerateObject()
                    .OrderBy(p => p.Name, StringComparer.Ordinal)
                    .ToArray();

                var propertiesY = y.EnumerateObject()
                    .OrderBy(p => p.Name, StringComparer.Ordinal)
                    .ToArray();

                if (propertiesX.Length != propertiesY.Length)
                {
                    return false;
                }

                foreach (var (px, py) in propertiesX.Zip(propertiesY, (px, py) => (px, py)))
                {
                    if (px.Name != py.Name)
                    {
                        return false;
                    }

                    if (!Equals(px.Value, py.Value))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public int GetHashCode(JsonElement obj)
        {
            var hash = new HashCode();

            AddHashCode(obj, ref hash);

            return hash.ToHashCode();
        }

        private void AddHashCode(JsonElement obj, ref HashCode hash)
        {
            if (obj.ValueKind == JsonValueKind.Number)
            {
                hash.Add(obj.GetRawText());
            }
            else if (obj.ValueKind == JsonValueKind.String)
            {
                hash.Add(obj.GetString());
            }
            else if (obj.ValueKind == JsonValueKind.Array)
            {
                var values = obj.EnumerateArray();

                foreach(var value in values)
                {
                    AddHashCode(value, ref hash);
                }
            }
            else if (obj.ValueKind == JsonValueKind.Object)
            {
                var properties = obj.EnumerateObject()
                    .OrderBy(x => x.Name, StringComparer.Ordinal);

                foreach (var property in properties)
                {
                    hash.Add(property.Name);

                    AddHashCode(property.Value, ref hash);
                }
            }
        }
    }
}
