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

        private HashSet<ResourceRef> resources;

        private ResourceRef[] resourceArray;

        private HashSet<ResourceRef> overflow;

        [Params(1, 5, 20, 100)]
        public int Items { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            resources = new HashSet<ResourceRef>();
            resourceArray = new ResourceRef[CacheLimit];
            overflow = new HashSet<ResourceRef>();

            ids = new string[Items];

            for (var i = 0; i < Items; i++)
            {
                ids[i] = i.ToString();
            }

            for (var i = 0; i < Items; i++)
            {
                resources.Add(new ResourceRef(ids[i], "netted-payment"));
            }

            for (var i = 0; i < Items && i < CacheLimit; i++)
            {
                resourceArray[i] = new ResourceRef(ids[i], "netted-payment");
            }

            for (var i = CacheLimit; i < Items; i++)
            {
                overflow.Add(new ResourceRef(ids[i], "netted-payment"));
            }
        }

        //[Benchmark]
        //public void SettingHash()
        //{
        //    var hash = new HashSet<ResourceRef>();

        //    for (var i = 0; i < Items; i++)
        //    {
        //        hash.Add(new ResourceRef(i.ToString(), "netted-payment"));
        //    }
        //}

        //[Benchmark]
        //public void SettingArray()
        //{
        //    var arr = new ResourceRef[Items];

        //    for (var i = 0; i < Items && i < 64; i++)
        //    {
        //        arr[i] = new ResourceRef(i.ToString(), "netted-payment");
        //    }

        //    if (Items > 64)
        //    {
        //        var over = new HashSet<(string, string)>();

        //        for (var i = 64; i < Items; i++)
        //        {
        //            over.Add((i.ToString(), "netted-payment"));
        //        }
        //    }
        //}

        [Benchmark]
        public bool GettingHash()
        {
            var value = false;

            for (var i = Items - 1; i >= 0; i--)
            {
                value = resources.Contains(new ResourceRef(ids[i], "netted-payment"));
            }

            return value;
        }

        [Benchmark]
        public bool GettingArray()
        {
            var value = false;

            for (var i = Items - 1; i >= 0; i--)
            {
                if (i >= CacheLimit)
                {
                    value = overflow.Contains(new ResourceRef(ids[i], "netted-payment"));
                }
                else
                {
                    for (var j = 0; j < Items && j < CacheLimit; j++)
                    {
                        var item = resourceArray[j];

                        if (item.Id == ids[i] && item.Type == "netted-payment")
                        {
                            value = true;
                            break;
                        }
                    }
                }
            }

            return value;
        }

        internal readonly struct ResourceRef : IEquatable<ResourceRef>
        {
            public readonly string Id;

            public readonly string Type;

            public ResourceRef(string id, string type)
            {
                Id = id;
                Type = type;
            }

            public override bool Equals(object? obj)
            {
                return obj is ResourceRef other && Equals(other);
            }

            public bool Equals(ResourceRef other)
            {
                return Id == other.Id && Type == other.Type;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hash = 17;

                    hash = hash * 23 + Id.GetHashCode();
                    hash = hash * 23 + Type.GetHashCode();

                    return hash;
                }
            }
        }
    }
}
