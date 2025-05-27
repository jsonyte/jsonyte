using System;
using System.Collections.Generic;
using System.Text.Json;
using Jsonyte.Converters;

namespace Jsonyte.Serialization
{
    internal ref struct TrackedResources
    {
        private const int CachedIncludes = 64;

        private const int CachedEmittedResources = 8;

        private IncludedRef[]? references;

        private ResourceRef[]? emittedResources;

        private int emittedResourcesCount;

        private Dictionary<(string type, string id), IncludedRef>? referencesOverflow;

        private HashSet<ResourceRef>? resourcesOverflow;

        private List<IncludedRef>? referencesOverflowByIndex;

        public int Count;

        public TrackedRelationships Relationships;

        public IncludedRef Get(int index)
        {
            if (references == null)
            {
                return default;
            }

            if (index < CachedIncludes)
            {
                return references[index];
            }

            if (referencesOverflowByIndex == null)
            {
                return default;
            }

            return referencesOverflowByIndex[index - CachedIncludes];
        }

        public void SetEmitted(string id, string type)
        {
            emittedResources ??= new ResourceRef[CachedEmittedResources];

            if (emittedResourcesCount < CachedEmittedResources)
            {
                emittedResources[emittedResourcesCount] = new ResourceRef(id, type);
            }
            else
            {
                resourcesOverflow ??= new HashSet<ResourceRef>();

                resourcesOverflow.Add(new ResourceRef(id, type));
            }

            emittedResourcesCount++;
        }

        public bool IsEmitted(IncludedRef included)
        {
            if (emittedResources == null)
            {
                return false;
            }

            for (var i = 0; i < emittedResourcesCount && i < CachedEmittedResources; i++)
            {
                var resource = emittedResources![i];

                if (resource.Id == included.IdString && resource.Type == included.TypeString)
                {
                    return true;
                }
            }

            return resourcesOverflow != null && resourcesOverflow.Contains(new ResourceRef(included.IdString, included.TypeString));
        }

        public void SetIncluded(
            scoped ResourceIdentifier identifier,
            string idString,
            string typeString,
            JsonObjectConverter converter,
            object value,
            JsonEncodedText? unwrittenRelationship = null,
            RelationshipSerializationType relationshipSerializationType = RelationshipSerializationType.Included)
        {
            references ??= new IncludedRef[CachedIncludes];

            if (HasIdentifier(identifier, out var idKey, out var typeKey, idString, typeString))
            {
                return;
            }

            SetIncluded(idKey, typeKey, identifier.Id.ToArray(), identifier.Type.ToArray(), idString, typeString, converter, value, relationshipSerializationType, unwrittenRelationship);
        }

        public void SetIncluded(
            byte[] id,
            byte[] type,
            string idString,
            string typeString,
            JsonObjectConverter converter,
            object value,
            JsonEncodedText? unwrittenRelationship = null,
            RelationshipSerializationType relationshipSerializationType = RelationshipSerializationType.Included)
        {
            references ??= new IncludedRef[CachedIncludes];

            var identifier = new ResourceIdentifier(id, type);

            if (HasIdentifier(identifier, out var idKey, out var typeKey, idString, typeString))
            {
                return;
            }

            SetIncluded(idKey, typeKey, id, type, idString, typeString, converter, value, relationshipSerializationType, unwrittenRelationship);
        }

        private void SetIncluded(
            ulong idKey,
            ulong typeKey,
            byte[] id,
            byte[] type,
            string idString,
            string typeString,
            JsonObjectConverter converter,
            object value,
            RelationshipSerializationType relationshipSerializationType,
            JsonEncodedText? unwrittenRelationship = null)
        {
            var relationshipId = unwrittenRelationship != null
                ? Relationships.SetRelationship(unwrittenRelationship.Value)
                : null;

            var included = new IncludedRef(idKey, typeKey, id, type, idString, typeString, converter, value, relationshipSerializationType, relationshipId);

            if (Count < CachedIncludes)
            {
                references![Count] = included;
            }
            else
            {
                referencesOverflow ??= new Dictionary<(string, string), IncludedRef>(CachedIncludes * 2);
                referencesOverflowByIndex ??= new List<IncludedRef>(CachedIncludes * 2);

                referencesOverflow[(typeString, idString)] = included;
                referencesOverflowByIndex.Add(included);
            }

            Count++;
        }

        public bool TryGetIncluded(scoped ResourceIdentifier identifier, out IncludedRef value)
        {
            if (references == null)
            {
                value = default;

                return false;
            }

            var idKey = identifier.Id.GetKey();
            var typeKey = identifier.Type.GetKey();

            var cachedCount = Count < CachedIncludes
                ? Count
                : CachedIncludes;

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

        private bool HasIdentifier(scoped ResourceIdentifier identifier, out ulong idKey, out ulong typeKey, string idString, string typeString)
        {
            idKey = identifier.Id.GetKey();
            typeKey = identifier.Type.GetKey();

            var cachedCount = Count < CachedIncludes
                ? Count
                : CachedIncludes;

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
