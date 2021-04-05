using System.Buffers;
using JsonApi.Serialization;
using IncludedValue = System.ValueTuple<JsonApi.JsonApiResourceIdentifier, JsonApi.Serialization.IJsonValueConverter, object>;

namespace JsonApi
{
    internal struct JsonApiState
    {
        private const int IncludedLength = 8;

        private IncludedValue[]? included;

        private int includedIndex;

        public void AddIncluded(JsonApiResourceIdentifier identifier, IJsonValueConverter converter, object value)
        {
            included ??= ArrayPool<IncludedValue>.Shared.Rent(IncludedLength);

            if (includedIndex >= included.Length)
            {
                var array = ArrayPool<IncludedValue>.Shared.Rent(included.Length * 2);

                included.CopyTo(array, 0);

                ArrayPool<IncludedValue>.Shared.Return(included);

                included = array;
            }

            included[includedIndex++] = new IncludedValue(identifier, converter, value);
        }

        public bool TryGetIncluded(JsonApiResourceIdentifier identifier, out IncludedValue value)
        {
            value = default;

            if (included == null)
            {
                return false;
            }

            for (var i = 0; i < includedIndex; i++)
            {
                var include = included[i];

                if (include.Item1.Equals(identifier))
                {
                    value = include;

                    return true;
                }
            }

            return false;
        }
    }
}
