using System.Collections.Generic;
using System.Text.Json;

namespace Jsonyte.Serialization
{
    internal ref struct TrackedResources
    {
        private const int OverflowCount = 16;

        private IncludedValue value0;

        private IncludedValue value1;

        private IncludedValue value2;

        private IncludedValue value3;

        private IncludedValue value4;

        private IncludedValue value5;

        private IncludedValue value6;

        private IncludedValue value7;

        private IncludedValue value8;

        private IncludedValue value9;

        private IncludedValue value10;

        private IncludedValue value11;

        private IncludedValue value12;

        private IncludedValue value13;

        private IncludedValue value14;

        private IncludedValue value15;

        private List<(byte[] id, byte[] type, IJsonObjectConverter converter, object value)>? overflow;

        public int Count;

        public TrackedRelationships Relationships;

        public IncludedValue Get(int index)
        {
            return index switch
            {
                0 => value0,
                1 => value1,
                2 => value2,
                3 => value3,
                4 => value4,
                5 => value5,
                6 => value6,
                7 => value7,
                8 => value8,
                9 => value9,
                10 => value10,
                11 => value11,
                12 => value12,
                13 => value13,
                14 => value14,
                15 => value15,
                _ => GetOverflow(index)
            };
        }

        public void SetIncluded(ResourceIdentifier identifier, IJsonObjectConverter converter, object value, JsonEncodedText? unwrittenRelationship = null)
        {
            if (HasIdentifier(identifier))
            {
                return;
            }

            var relationshipId = unwrittenRelationship != null
                ? Relationships.SetRelationship(unwrittenRelationship.Value)
                : null;

            var included = new IncludedValue(identifier, converter, value, relationshipId);

            if (Count == 0)
            {
                value0 = included;
            }
            else if (Count == 1)
            {
                value1 = included;
            }
            else if (Count == 2)
            {
                value2 = included;
            }
            else if (Count == 3)
            {
                value3 = included;
            }
            else if (Count == 4)
            {
                value4 = included;
            }
            else if (Count == 5)
            {
                value5 = included;
            }
            else if (Count == 6)
            {
                value6 = included;
            }
            else if (Count == 7)
            {
                value7 = included;
            }
            else if (Count == 8)
            {
                value8 = included;
            }
            else if (Count == 9)
            {
                value9 = included;
            }
            else if (Count == 10)
            {
                value10 = included;
            }
            else if (Count == 11)
            {
                value11 = included;
            }
            else if (Count == 12)
            {
                value12 = included;
            }
            else if (Count == 13)
            {
                value13 = included;
            }
            else if (Count == 14)
            {
                value14 = included;
            }
            else if (Count == 15)
            {
                value15 = included;
            }
            else
            {
                overflow ??= new List<(byte[], byte[], IJsonObjectConverter, object)>(OverflowCount);

                var id = identifier.Id;
                var type = identifier.Type;

                overflow.Add((id.ToArray(), type.ToArray(), converter, value));
            }

            Count++;
        }

        public bool TryGetIncluded(ResourceIdentifier identifier, out IncludedValue value)
        {
            for (var i = 0; i < Count; i++)
            {
                var included = Get(i);

                if (included.HasIdentifier(identifier))
                {
                    value = included;
                    return true;
                }
            }

            value = default;

            return false;
        } 

        private IncludedValue GetOverflow(int index)
        {
            if (overflow == null)
            {
                return default;
            }

            var item = overflow[index - OverflowCount];

            var identifier = new ResourceIdentifier(item.id, item.type);

            return new IncludedValue(identifier, item.converter, item.value);
        }

        private bool HasIdentifier(ResourceIdentifier identifier)
        {
            for (var i = 0; i < Count; i++)
            {
                var included = Get(i);

                if (included.HasIdentifier(identifier))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
