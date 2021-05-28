using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Jsonyte.Tests.Performance
{
    public class MicroPerformanceBenchmarks
    {
        private const int CacheLimit = 8;

        private string[] ids;

        private byte[][] idBytes;

        private byte[] typeBytes;

        [Params(1, 5, 20, 100)]
        public int Items { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            ids = new string[Items];
            idBytes = new byte[Items][];
            typeBytes = Encoding.UTF8.GetBytes("netted-payment");

            for (var i = 0; i < Items; i++)
            {
                ids[i] = i.ToString();
                idBytes[i] = Encoding.UTF8.GetBytes(i.ToString());
            }
        }

        [Benchmark]
        public bool Current()
        {
            var value = false;

            var tracked = new TrackedResources();
            tracked.Max = 64;

            for (var i = 0; i < Items; i++)
            {
                tracked.SetIncluded(idBytes[i], typeBytes, ids[i], "netted-payment");
            }

            for (var i = Items - 1; i >= 0; i--)
            {
                value = tracked.TryGetIncluded(new ResourceIdentifier(idBytes[i], typeBytes), out var included);
            }

            return value;
        }

        //[Benchmark]
        //public bool Newer()
        //{
        //    var value = false;

        //    var tracked = new TrackedImproved();

        //    for (var i = 0; i < Items; i++)
        //    {
        //        tracked.SetIncluded(idBytes[i], typeBytes, ids[i], "netted-payment");
        //    }

        //    for (var i = Items - 1; i >= 0; i--)
        //    {
        //        value = tracked.TryGetIncluded(new ResourceIdentifier(idBytes[i], typeBytes), out var included);
        //    }

        //    return value;
        //}

        private ref struct TrackedImproved
        {
            private Dictionary<ResourceRef, IncludedRef>? references;

            public void SetIncluded(byte[] id, byte[] type, string idString, string typeString)
            {
                references ??= new Dictionary<ResourceRef, IncludedRef>(8);

                references[new ResourceRef(id, type)] = new IncludedRef(0, 0, id, type, idString, typeString);
            }

            public bool TryGetIncluded(ResourceIdentifier identifier, out IncludedRef value)
            {
                if (references == null)
                {
                    value = default;

                    return false;
                }

                return references.TryGetValue(new ResourceRef(identifier.Id.ToArray(), identifier.Type.ToArray()), out value);
            }
        }

        internal readonly struct ResourceRef : IEquatable<ResourceRef>
        {
            private readonly byte[] id;

            private readonly byte[] type;

            public ResourceRef(byte[] id, byte[] type)
            {
                this.id = id;
                this.type = type;
            }

            public ReadOnlySpan<byte> Id => id;

            public ReadOnlySpan<byte> Type => type;

            public override bool Equals(object? obj)
            {
                return obj is ResourceRef other && Equals(other);
            }

            public bool Equals(ResourceRef other)
            {
                return Id.SequenceEqual(other.Id) && Type.SequenceEqual(other.type);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hash = 17;

                    for (var i = 0; i < id.Length; i++)
                    {
                        hash = hash * 23 + id[i].GetHashCode();
                    }

                    for (var i = 0; i < type.Length; i++)
                    {
                        hash = hash * 23 + type[i].GetHashCode();
                    }

                    return hash;
                }
            }
        }

        private ref struct TrackedResources
        {
            public int Max;

            private IncludedRef[]? references;

            private Dictionary<(string type, string id), IncludedRef>? referencesOverflow;

            private int count;

            public void SetIncluded(byte[] id, byte[] type, string idString, string typeString)
            {
                references ??= new IncludedRef[Max];

                var idSpan = new ReadOnlySpan<byte>(id);
                var typeSpan = new ReadOnlySpan<byte>(type);

                var included = new IncludedRef(idSpan.GetKey(), typeSpan.GetKey(), id, type, idString, typeString);

                if (count < Max)
                {
                    references![count] = included;
                }
                else
                {
                    referencesOverflow ??= new Dictionary<(string, string), IncludedRef>(Max * 2);
                    referencesOverflow[(idString, typeString)] = included;
                }

                count++;
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

                var cachedCount = count < Max
                    ? count
                    : Max;

                for (var i = 0; i < cachedCount; i++)
                {
                    var include = references[i];

                    if (include.IdKey == idKey && include.TypeKey == typeKey)
                    {
                        var idEqual = identifier.Id.Length < 8 || identifier.Id.SequenceEqual(include.Id);
                        var typeEqual = identifier.Type.Length < 8 || identifier.Type.SequenceEqual(include.Type);

                        if (idEqual && typeEqual)
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

                if (referencesOverflow.TryGetValue((id.GetString(), type.GetString()), out var output))
                {
                    value = output;

                    return true;
                }

                value = default;

                return false;
            }
        }

        internal readonly ref struct ResourceIdentifier
        {
            public readonly ReadOnlySpan<byte> Id;

            public readonly ReadOnlySpan<byte> Type;

            public ResourceIdentifier(ReadOnlySpan<byte> id, ReadOnlySpan<byte> type)
            {
                Id = id;
                Type = type;
            }
        }

        internal readonly struct IncludedRef
        {
            public readonly ulong IdKey;

            public readonly ulong TypeKey;

            public readonly byte[] Id;

            public readonly byte[] Type;

            public readonly string IdString;

            public readonly string TypeString;

            public IncludedRef(ulong idKey, ulong typeKey, byte[] id, byte[] type, string idString, string typeString)
            {
                IdKey = idKey;
                TypeKey = typeKey;
                Id = id;
                Type = type;
                IdString = idString;
                TypeString = typeString;
            }
        }
    }
}
