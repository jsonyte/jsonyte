using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace JsonApi.Serialization
{
    internal ref struct TrackedResources
    {
        private const int IncludedLength = 8;

        //private IncludedValue[]? included;
        private Dictionary<JsonApiResourceIdentifier, IncludedValue>? included;

        private Queue<JsonApiResourceIdentifier>? stack;

        private int includedIndex;

        public int Count => included?.Count ?? 0;

        public Queue<JsonApiResourceIdentifier> Identifiers
        {
            get
            {
                if (stack == null)
                {
                    stack = new Queue<JsonApiResourceIdentifier>(Count);

                    foreach (var key in included!.Keys)
                    {
                        stack.Enqueue(key);
                    }
                }

                return stack;
            }
        }

        public void SetIncluded(JsonApiResourceIdentifier identifier, IJsonValueConverter converter, object value)
        {
            //included ??= ArrayPool<IncludedValue>.Shared.Rent(IncludedLength);

            //if (includedIndex >= included.Length)
            //{
            //    var array = ArrayPool<IncludedValue>.Shared.Rent(included.Length * 2);

            //    included.CopyTo(array, 0);

            //    ArrayPool<IncludedValue>.Shared.Return(included);

            //    included = array;
            //}

            //included[includedIndex++] = new IncludedValue(identifier, converter, value);

            included ??= new Dictionary<JsonApiResourceIdentifier, IncludedValue>();

            if (!included.ContainsKey(identifier))
            {
                included[identifier] = new IncludedValue(identifier, converter, value);

                stack?.Enqueue(identifier);
            }
        }

        public bool TryGetIncluded(JsonApiResourceIdentifier identifier, out IncludedValue value)
        {
            //value = default;

            //if (included == null)
            //{
            //    return false;
            //}

            //for (var i = 0; i < includedIndex; i++)
            //{
            //    var include = included[i];

            //    if (include.Identifier.Equals(identifier))
            //    {
            //        value = include;

            //        return true;
            //    }
            //}

            //return false;

            included ??= new Dictionary<JsonApiResourceIdentifier, IncludedValue>();

            return included.TryGetValue(identifier, out value);
        }

        public void Release()
        {
            //if (included != null)
            //{
            //    ArrayPool<IncludedValue>.Shared.Return(included, true);
            //}

            //included = null;
        }
    }
}
