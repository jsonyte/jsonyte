using System;
using System.Collections.Generic;
using System.Text.Json;
using Jsonyte.Converters;

namespace Jsonyte.Serialization
{
    internal ref struct TrackedResources
    {
        private const int CachedReferences = 64;

        private IncludedRef[]? references;

        private Dictionary<(string type, string id), IncludedRef>? referencesOverflow;

        private List<IncludedRef>? referencesOverflowByIndex;

        public int Count;

        public TrackedRelationships Relationships;

        public IncludedRef Get(int index)
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

        public void SetIncluded(ResourceIdentifier identifier, string idString, string typeString, JsonObjectConverter converter, object value)
        {
            references ??= new IncludedRef[CachedReferences];

            if (HasIdentifier(identifier, out var idKey, out var typeKey, idString, typeString))
            {
                return;
            }

            SetIncluded(idKey, typeKey, identifier.Id.ToArray(), identifier.Type.ToArray(), idString, typeString, converter, value);
        }

        public void SetIncluded(byte[] id, byte[] type, string idString, string typeString, JsonObjectConverter converter, object value, JsonEncodedText? unwrittenRelationship = null, bool emitIncluded = true)
        {
            references ??= new IncludedRef[CachedReferences];

            var identifier = new ResourceIdentifier(id, type);

            if (HasIdentifier(identifier, out var idKey, out var typeKey, idString, typeString))
            {
                return;
            }

            SetIncluded(idKey, typeKey, id, type, idString, typeString, converter, value, unwrittenRelationship, emitIncluded);
        }

        private void SetIncluded(ulong idKey, ulong typeKey, byte[] id, byte[] type, string idString, string typeString, JsonObjectConverter converter, object value, JsonEncodedText? unwrittenRelationship = null, bool emitIncluded = true)
        {
            var relationshipId = unwrittenRelationship != null
                ? Relationships.SetRelationship(unwrittenRelationship.Value)
                : null;

            var included = new IncludedRef(idKey, typeKey, id, type, converter, value, relationshipId, emitIncluded);

            if (Count < CachedReferences)
            {
                references![Count] = included;
            }
            else
            {
                referencesOverflow ??= new Dictionary<(string, string), IncludedRef>(CachedReferences * 2);
                referencesOverflowByIndex ??= new List<IncludedRef>(CachedReferences * 2);

                referencesOverflow[(typeString, idString)] = included;
                referencesOverflowByIndex.Add(included);
            }

            Count++;
        }

        public bool TryGetIncluded(ResourceIdentifier identifier, out IncludedRef value)
        {
            if (references == null)
            {
                value = default;

                return false;
            }

            var idKey = identifier.Id.GetKey();
            var typeKey = identifier.Type.GetKey();

            var cachedCount = Count < CachedReferences
                ? Count
                : CachedReferences;

            for (var i = 0; i < cachedCount; i++)
            {
                var include = references[i];

                if (include.IdKey == idKey && include.TypeKey == typeKey)
                {
                    if (identifier.Id.Length < 8 && identifier.Type.Length < 8)
                    {
                        value = include;

                        return true;
                    }

                    if (identifier.Id.SequenceEqual(include.Id) && identifier.Type.SequenceEqual(include.Type))
                    {
                        value = include;

                        return true;
                    }
                }
            }

            if (referencesOverflow == null)
            {
                value = default;

                return false;
            }

            var id = identifier.Id;
            var type = identifier.Type;

            var idString = id.GetString();
            var typeString = type.GetString();

            if (referencesOverflow.TryGetValue((typeString, idString), out var output))
            {
                value = output;

                return true;
            }

            value = default;

            return false;
        }

        private bool HasIdentifier(ResourceIdentifier identifier, out ulong idKey, out ulong typeKey, string idString, string typeString)
        {
            idKey = identifier.Id.GetKey();
            typeKey = identifier.Type.GetKey();

            var cachedCount = Count < CachedReferences
                ? Count
                : CachedReferences;

            for (var i = 0; i < cachedCount; i++)
            {
                var include = references![i];

                if (include.IdKey == idKey && include.TypeKey == typeKey)
                {
                    if (identifier.Id.Length < 8 && identifier.Type.Length < 8)
                    {
                        return true;
                    }

                    if (identifier.Id.SequenceEqual(include.Id) && identifier.Type.SequenceEqual(include.Type))
                    {
                        return true;
                    }
                }
            }

            if (referencesOverflow == null)
            {
                return false;
            }

            return referencesOverflow!.ContainsKey((typeString, idString));
        }
    }
}
