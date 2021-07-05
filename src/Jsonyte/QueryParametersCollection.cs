using System;
using System.Collections.Specialized;
using System.Text;

namespace Jsonyte
{
    internal class QueryParametersCollection : NameValueCollection
    {
        public QueryParametersCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public override void Add(string name, string value)
        {
            base.Add(name, value);
        }

        public override string Get(int index)
        {
            return base.Get(index);
        }

        public override string Get(string name)
        {
            return base.Get(name);
        }

        public override string[] GetValues(int index)
        {
            return base.GetValues(index);
        }

        public override string[] GetValues(string name)
        {
            return base.GetValues(name);
        }

        public override void Remove(string name)
        {
            base.Remove(name);
        }

        public override void Set(string name, string value)
        {
            base.Set(name, value);
        }

        public override void Clear()
        {
            base.Clear();
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

                foreach (var value in values)
                {
                    if (string.IsNullOrEmpty(key))
                    {
                        result.AppendFormat("{0}&", Uri.EscapeDataString(value));
                    }
                    else
                    {
                        result.AppendFormat("{0}={1}&", key, Uri.EscapeDataString(value));
                    }
                }
            }

            return result.ToString(0, result.Length - 1);
        }
    }
}
