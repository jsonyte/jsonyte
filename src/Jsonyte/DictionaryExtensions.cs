using System.Collections.Generic;

namespace Jsonyte
{
    internal static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue defaultValue)
            where TKey : notnull
        {
            return source.TryGetValue(key, out var value)
                ? value
                : defaultValue;
        }
    }
}
