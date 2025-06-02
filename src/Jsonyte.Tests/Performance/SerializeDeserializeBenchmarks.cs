#define RUN_FULL_BENCHMARK

using System;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Bogus;
using JsonApiSerializer;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Jsonyte.Tests.Performance
{
#if RUN_FULL_BENCHMARK
    [SimpleJob(RuntimeMoniker.Net472)]
    [SimpleJob(RuntimeMoniker.Net80)]
#else
    [ShortRunJob(RuntimeMoniker.Net472)]
    [ShortRunJob(RuntimeMoniker.Net80)]
#endif
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class SerializeDeserializeBenchmarks
    {
        private const string Serialize = nameof(Serialize);

        private const string Deserialize = nameof(Deserialize);

        private JsonSerializerOptions options;

        private JsonSerializerOptions jsonApiOptions;

        private JsonApiSerializerSettings jsonApiSettings;

        private Data data;

        [Params("Simple", "Compound", "LargeCompound", "ErrorCollection", "Anonymous")]
        public string Case { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            Randomizer.Seed = new Random(56178921);

            options = new JsonSerializerOptions();
            jsonApiOptions = new JsonSerializerOptions().AddJsonApi();
            jsonApiSettings = new JsonApiSerializerSettings();

            data = TestCases.Get(Case);
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(Serialize)]
        public string SerializeSystemTextJson()
        {
            return JsonSerializer.Serialize(data.Value, options);
        }

        [Benchmark]
        [BenchmarkCategory(Serialize)]
        public string SerializeJsonyte()
        {
            return JsonSerializer.Serialize(data.Value, jsonApiOptions);
        }

        [Benchmark]
        [BenchmarkCategory(Serialize)]
        public string SerializeJsonApiSerializer()
        {
            return JsonConvert.SerializeObject(data.Value, jsonApiSettings);
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(Deserialize)]
        public object DeserializeSystemTextJson()
        {
            return data.SkipDeserialize
                ? null
                : JsonSerializer.Deserialize(data.JsonBytes, data.Type, options);
        }

        [Benchmark]
        [BenchmarkCategory(Deserialize)]
        public object DeserializeJsonyte()
        {
            return data.SkipDeserialize
                ? null
                : JsonSerializer.Deserialize(data.JsonApiBytes, data.Type, jsonApiOptions);
        }

        [Benchmark]
        [BenchmarkCategory(Deserialize)]
        public object DeserializeJsonApiSerializer()
        {
            return data.SkipDeserialize
                ? null :
                JsonConvert.DeserializeObject(data.Json, data.Type, jsonApiSettings);
        }
    }
}
