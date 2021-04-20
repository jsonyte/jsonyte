using System.Collections.Generic;
using System.Text.Json;

namespace Jsonyte.Serialization
{
    internal ref struct TrackedRelationships
    {
        private const int OverflowCount = 8;

        private IncludedRelationship value0;

        private IncludedRelationship value1;

        private IncludedRelationship value2;

        private IncludedRelationship value3;

        private IncludedRelationship value4;

        private IncludedRelationship value5;

        private IncludedRelationship value6;

        private IncludedRelationship value7;

        private List<(JsonEncodedText, int)>? overflow;

        private int nextId;

        public int Count;

        public bool LastWritten;

        public IncludedRelationship Get(int index)
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
                _ => GetRelationshipOverflow(index)
            };
        }

        public int? SetRelationship(JsonEncodedText name)
        {
            var id = GetRelationshipId(name);

            if (id != null)
            {
                return id;
            }

            id = GetNextRelationshipId();

            var value = new IncludedRelationship(name, id.Value);

            if (Count == 0)
            {
                value0 = value;
            }
            else if (Count == 1)
            {
                value1 = value;
            }
            else if (Count == 2)
            {
                value2 = value;
            }
            else if (Count == 3)
            {
                value3 = value;
            }
            else if (Count == 4)
            {
                value4 = value;
            }
            else if (Count == 5)
            {
                value5 = value;
            }
            else if (Count == 6)
            {
                value6 = value;
            }
            else if (Count == 7)
            {
                value7 = value;
            }
            else
            {
                overflow ??= new List<(JsonEncodedText, int)>(Count);

                overflow.Add((value.Name, value.Id));
            }

            Count++;

            return id;
        }

        public void Clear()
        {
            Count = 0;

            overflow?.Clear();
        }

        private int GetNextRelationshipId()
        {
            return nextId++;
        }

        private int? GetRelationshipId(JsonEncodedText name)
        {
            for (var i = 0; i < Count; i++)
            {
                var value = Get(i);

                if (value.Name.Equals(name))
                {
                    return value.Id;
                }
            }

            return null;
        }

        private IncludedRelationship GetRelationshipOverflow(int index)
        {
            if (overflow == null)
            {
                return default;
            }

            var item = overflow[index - OverflowCount];

            return new IncludedRelationship(item.Item1, item.Item2);
        }
    }
}
