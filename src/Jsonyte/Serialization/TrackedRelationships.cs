using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Jsonyte.Serialization
{
    internal ref struct TrackedRelationships
    {
        private const int CachedReferences = 64;

        private RelationshipRef[]? references;

        private Dictionary<JsonEncodedText, RelationshipRef>? referencesOverflow;

        private List<RelationshipRef>? referencesOverflowByIndex;

        private int nextId;

        public int Count;

        public bool LastWritten;

        public RelationshipRef Get(int index)
        {
            if (references == null)
            {
                return default;
            }

            if (index < CachedReferences)
            {
                return references[index];
            }

            if (referencesOverflowByIndex == null)
            {
                return default;
            }

            return referencesOverflowByIndex[index - CachedReferences];
        }

        public int? SetRelationship(JsonEncodedText name)
        {
            references ??= new RelationshipRef[CachedReferences];

            var id = GetRelationshipId(name, out var key);

            if (id != null)
            {
                return id;
            }

            id = GetNextRelationshipId();

            var relationship = new RelationshipRef(key, name, id.Value);

            if (Count < CachedReferences)
            {
                references[Count] = relationship;
            }
            else
            {
                referencesOverflow = new Dictionary<JsonEncodedText, RelationshipRef>(CachedReferences * 2);
                referencesOverflowByIndex = new List<RelationshipRef>(CachedReferences * 2);

                referencesOverflow[name] = relationship;
                referencesOverflowByIndex.Add(relationship);
            }

            Count++;

            return id;
        }

        public void Clear()
        {
            Count = 0;

            referencesOverflow?.Clear();
            referencesOverflowByIndex?.Clear();
        }

        private int GetNextRelationshipId()
        {
            return nextId++;
        }

        private int? GetRelationshipId(JsonEncodedText name, out ulong key)
        {
            key = name.EncodedUtf8Bytes.GetKey();

            var cachedCount = Count < CachedReferences
                ? Count
                : CachedReferences;

            for (var i = 0; i < cachedCount; i++)
            {
                var relationship = references![i];

                if (relationship.Key == key)
                {
                    if (name.EncodedUtf8Bytes.Length < 8)
                    {
                        return relationship.Id;
                    }

                    if (name.EncodedUtf8Bytes.SequenceEqual(name.EncodedUtf8Bytes))
                    {
                        return relationship.Id;
                    }
                }
            }

            if (referencesOverflow == null)
            {
                return null;
            }

            if (referencesOverflow.TryGetValue(name, out var value))
            {
                return value.Id;
            }

            return null;
        }
    }
}
