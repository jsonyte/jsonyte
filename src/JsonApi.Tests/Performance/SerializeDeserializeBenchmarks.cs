using System;
using System.Text.Json;
using AutoBogus;
using BenchmarkDotNet.Attributes;
using Bogus;
using JsonApi.Tests.Models;
using JsonApiSerializer;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonApi.Tests.Performance
{
    public class SerializeDeserializeBenchmarks
    {
        private JsonSerializerOptions options;

        private JsonSerializerOptions rawOptions;

        private JsonApiSerializerSettings settings;

        private Data<ManyTypesModel> simpleModel;

        private Data<ArticleWithAuthor> baselineModel;

        private Data<ArticleWithAuthor[]> articles;

        [GlobalSetup]
        public void Setup()
        {
            Randomizer.Seed = new Random(56178921);

            rawOptions = new JsonSerializerOptions();
            options = GetOptions();
            settings = GetSettings();

            SetupTestData();
        }

        private void SetupTestData()
        {
            simpleModel = new Data<ManyTypesModel>(AutoFaker.Generate<ManyTypesModel>(), options);
            articles = new Data<ArticleWithAuthor[]>(AutoFaker.Generate<ArticleWithAuthor[]>(x => x.WithRepeatCount(20)), options);

            var author = new Author
            {
                Id = "9",
                Type = "people",
                Name = "Dan Gebhardt",
                Twitter = "dgeb"
            };

            baselineModel = new Data<ArticleWithAuthor>(new ArticleWithAuthor
            {
                Id = "1",
                Type = "articles",
                Title = "JSON:API paints my bikeshed!",
                Author = author,
                Comments = new[]
                {
                    new Comment
                    {
                        Id = "5",
                        Type = "comments",
                        Body = "First!",
                        Author = author
                    },
                    new Comment
                    {
                        Id = "12",
                        Type = "comments",
                        Body = "I like XML better",
                        Author = author
                    }
                }
            }, options);
        }

        private JsonSerializerOptions GetOptions()
        {
            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return serializerOptions.AddJsonApi();
        }

        private JsonApiSerializerSettings GetSettings()
        {
            return new();
        }

        [Benchmark]
        public string SerializeSimpleObject()
        {
            return JsonSerializer.Serialize(simpleModel.Value, options);
        }

        [Benchmark]
        public string SerializeSimpleObjectRaw()
        {
            return JsonSerializer.Serialize(simpleModel.Value, rawOptions);
        }

        [Benchmark]
        public string SerializeSimpleObjectWithNewtonsoft()
        {
            return JsonConvert.SerializeObject(simpleModel.Value, settings);
        }

        [Benchmark]
        public string SerializeBaselineObject()
        {
            return JsonSerializer.Serialize(baselineModel.Value, options);
        }

        [Benchmark]
        public string SerializeBaselineObjectRaw()
        {
            return JsonSerializer.Serialize(baselineModel.Value, rawOptions);
        }

        [Benchmark]
        public string SerializeBaselineObjectWithNewtonsoft()
        {
            return JsonConvert.SerializeObject(baselineModel.Value, settings);
        }

        [Benchmark]
        public string SerializeLargeCompoundCollection()
        {
            return JsonSerializer.Serialize(articles.Value, options);
        }

        [Benchmark]
        public string SerializeLargeCompoundCollectionRaw()
        {
            return JsonSerializer.Serialize(articles.Value, rawOptions);
        }

        [Benchmark]
        public string SerializeLargeCompoundCollectionWithNewtonsoft()
        {
            return JsonConvert.SerializeObject(articles.Value, settings);
        }

        [Benchmark]
        public object DeserializeSimpleObject()
        {
            return JsonSerializer.Deserialize<ManyTypesModel>(simpleModel.JsonApiBytes, options);
        }

        [Benchmark]
        public object DeserializeSimpleObjectRaw()
        {
            return JsonSerializer.Deserialize<ManyTypesModel>(simpleModel.JsonBytes, rawOptions);
        }

        [Benchmark]
        public object DeserializeSimpleObjectWithNewtonsoft()
        {
            return JsonConvert.DeserializeObject<ManyTypesModel>(simpleModel.Json, settings);
        }

        [Benchmark]
        public object DeserializeBaselineObject()
        {
            return JsonSerializer.Deserialize<ArticleWithAuthor>(baselineModel.JsonApiBytes, options);
        }

        [Benchmark]
        public object DeserializeBaselineObjectRaw()
        {
            return JsonSerializer.Deserialize<ArticleWithAuthor>(baselineModel.JsonApiBytes, rawOptions);
        }

        [Benchmark]
        public object DeserializeBaselineObjectWithNewtonsoft()
        {
            return JsonConvert.DeserializeObject<ArticleWithAuthor>(baselineModel.Json, settings);
        }

        [Benchmark]
        public object DeserializeLargeCompoundCollection()
        {
            return JsonSerializer.Deserialize<ArticleWithAuthor[]>(articles.JsonApiBytes, options);
        }

        [Benchmark]
        public object DeserializeLargeCompoundCollectionRaw()
        {
            return JsonSerializer.Deserialize<ArticleWithAuthor[]>(articles.JsonBytes, rawOptions);
        }

        [Benchmark]
        public object DeserializeLargeCompoundCollectionWithNewtonsoft()
        {
            return JsonConvert.DeserializeObject<ArticleWithAuthor[]>(articles.Json, settings);
        }
    }
}
