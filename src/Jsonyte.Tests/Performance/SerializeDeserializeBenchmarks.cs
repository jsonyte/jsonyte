using System;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using Bogus;
using JsonApiSerializer;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Jsonyte.Tests.Performance
{
    public class SerializeDeserializeBenchmarks
    {
        private const string Serialize = nameof(Serialize);

        private const string Deserialize = nameof(Deserialize);

        private JsonSerializerOptions options;

        private JsonSerializerOptions jsonApiOptions;

        private JsonSerializerSettings settings;

        private JsonApiSerializerSettings jsonApiSettings;

        private Data data;

        [Params("Simple", "Compound", "LargeCompound", "SingleError", "ErrorCollection", "Document", "Anonymous")]
        public string Case { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            Randomizer.Seed = new Random(56178921);

            options = new JsonSerializerOptions();
            jsonApiOptions = new JsonSerializerOptions().AddJsonApi();
            settings = new JsonSerializerSettings();
            jsonApiSettings = new JsonApiSerializerSettings();

            data = TestCases.Get(Case);
        }

        [Benchmark]
        [BenchmarkCategory(Serialize)]
        public string SerializeNoJsonApi()
        {
            return JsonSerializer.Serialize(data.Value, options);
        }

        [Benchmark]
        [BenchmarkCategory(Serialize)]
        public string SerializeJsonApi()
        {
            return JsonSerializer.Serialize(data.Value, jsonApiOptions);
        }

        [Benchmark]
        [BenchmarkCategory(Serialize)]
        public string SerializeNewtonsoftNoJsonApi()
        {
            return JsonConvert.SerializeObject(data.Value, settings);
        }

        [Benchmark]
        [BenchmarkCategory(Serialize)]
        public string SerializeNewtonsoftJsonApi()
        {
            return JsonConvert.SerializeObject(data.Value, jsonApiSettings);
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(Deserialize)]
        public object DeserializeNoJsonApi()
        {
            return data.SkipDeserialize
                ? null
                : JsonSerializer.Deserialize(data.JsonBytes, data.Type, options);
        }

        [Benchmark]
        [BenchmarkCategory(Deserialize)]
        public object DeserializeJsonApi()
        {
            return data.SkipDeserialize
                ? null
                : JsonSerializer.Deserialize(data.JsonApiBytes, data.Type, jsonApiOptions);
        }

        [Benchmark]
        [BenchmarkCategory(Deserialize)]
        public object DeserializeNewtonsoftNoJsonApi()
        {
            return data.SkipDeserialize
                ? null
                : JsonConvert.DeserializeObject(data.Json, data.Type, settings);
        }

        [Benchmark]
        [BenchmarkCategory(Deserialize)]
        public object DeserializeNewtonsoftJsonApi()
        {
            return data.SkipDeserialize
                ? null :
                JsonConvert.DeserializeObject(data.Json, data.Type, jsonApiSettings);
        }
    }
}
