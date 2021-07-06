using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Jsonyte
{
    internal class QueryParametersCollection : NameValueCollection
    {
        private static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;

        private static readonly string[] Comma = {","};

        private static readonly string[] Ampersand = {"&"};

        private readonly List<NameValue> entries = new();

        public QueryParametersCollection()
            : base(Comparer)
        {
        }

        public static QueryParametersCollection Parse(string query)
        {
            var result = new QueryParametersCollection();

            if (query.FirstOrDefault() == '?')
            {
                query = query.Substring(1);
            }

            var queryParts = Split(query, Ampersand);

            foreach (var queryPart in queryParts)
            {
                var index = queryPart.IndexOf('=');

                var name = index != -1
                    ? queryPart.Substring(0, index)
                    : null;

                var value = index != -1
                    ? queryPart.Substring(index + 1, queryPart.Length - index - 1)
                    : queryPart;

                var escapedName = name != null
                    ? Uri.UnescapeDataString(name)
                    : null;

                if (IsCommaSeparated(name))
                {
                    var values = Split(value, Comma);

                    foreach (var singleValue in values)
                    {
                        result.Add(escapedName, Uri.UnescapeDataString(singleValue));
                    }
                }
                else
                {
                    result.Add(escapedName, Uri.UnescapeDataString(value));
                }
            }

            return result;
        }

        public override void Add(string? name, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var entry = FindEntry(name) ?? CreateEntry(name);

            if (!entry.Values.Contains(value, Comparer))
            {
                base.Add(name, value);

                entry.Values.Add(value);
            }
        }

        public override string? Get(int index)
        {
            if (Count == 0)
            {
                return null;
            }

            var entry = entries[index];

            return ToString(entry);
        }

        public override string? Get(string? name)
        {
            var entry = FindEntry(name);

            if (entry == null)
            {
                return null;
            }

            return ToString(entry);
        }

        public override string[]? GetValues(int index)
        {
            if (Count == 0)
            {
                return null;
            }

            var entry = entries[index];

            return entry.Values.ToArray();
        }

        public override string[]? GetValues(string? name)
        {
            var entry = FindEntry(name);

            return entry?.Values.ToArray();
        }

        public override void Remove(string? name)
        {
            var entry = FindEntry(name);

            if (entry != null)
            {
                base.Remove(name);

                entries.Remove(entry);
            }
        }

        public override void Set(string? name, string value)
        {
            base.Set(name, value);

            var entry = FindEntry(name) ?? CreateEntry(name);

            entry.Values.Add(value);
        }

        public override void Clear()
        {
            base.Clear();

            entries.Clear();
        }

        public override string ToString()
        {
            if (Count == 0)
            {
                return string.Empty;
            }

            var result = new StringBuilder();

            foreach (var key in AllKeys)
            {
                var values = GetValues(key);

                if (values == null)
                {
                    continue;
                }

                if (IsCommaSeparated(key))
                {
                    result.Append($"{key}={string.Join(",", values)}&");
                }
                else
                {
                    var escapedValues = values.Select(Uri.EscapeDataString);

                    var joined = string.IsNullOrEmpty(key)
                        ? string.Join("&", escapedValues)
                        : string.Join("&", escapedValues.Select(x => $"{key}={x}"));

                    result.Append($"{joined}&");
                }
            }

            return result.ToString(0, result.Length - 1);
        }

        private static string[] Split(string value, string[] separator)
        {
            return value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        private static bool IsCommaSeparated(string? name)
        {
            if (name == null)
            {
                return false;
            }

            return name.Equals("include", StringComparison.OrdinalIgnoreCase) ||
                   name.Equals("sort", StringComparison.OrdinalIgnoreCase);
        }

        private string? ToString(NameValue? entry)
        {
            if (entry == null || entry.Values.Count == 0)
            {
                return null;
            }

            var result = new StringBuilder();

            foreach (var value in entry.Values)
            {
                result.Append($"{value},");
            }

            return result.ToString(0, result.Length - 1);
        }

        private NameValue? FindEntry(string? name)
        {
            return name == null
                ? entries.FirstOrDefault(x => x.Name == null)
                : entries.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        private NameValue CreateEntry(string? name)
        {
            entries.Add(new NameValue(name));

            return entries.Last();
        }

        private class NameValue
        {
            public NameValue(string? name)
            {
                Name = name;
            }

            public string? Name { get; }

            public List<string> Values { get; } = new();
        }
    }
}
