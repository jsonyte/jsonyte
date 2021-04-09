using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoBogus;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Bogus;
using Bogus.Extensions;
using JsonApi.Tests.Models;
using JsonApiSerializer;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JsonApi.Tests.Performance
{
    [CategoriesColumn]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class SerializeDeserializeBenchmarks
    {
        private const string Serialize = nameof(Serialize);

        private const string Deserialize = nameof(Deserialize);

        private const string JsonApi = nameof(JsonApi);

        private const string NoJsonApi = nameof(NoJsonApi);

        private JsonSerializerOptions options;

        private JsonSerializerOptions jsonApiOptions;

        private JsonSerializerSettings settings;

        private JsonApiSerializerSettings jsonApiSettings;

        [Params("Simple", "Compound", "LargeCompound")]
        public string Case { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            Randomizer.Seed = new Random(56178921);

            options = new JsonSerializerOptions();
            jsonApiOptions = new JsonSerializerOptions().AddJsonApi();
            settings = new JsonSerializerSettings();
            jsonApiSettings = new JsonApiSerializerSettings();
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(Serialize, NoJsonApi)]
        public string SerializeNoJsonApi()
        {
            var data = GetTestCase();

            return JsonSerializer.Serialize(data.Value, options);
        }

        [Benchmark]
        [BenchmarkCategory(Serialize, JsonApi)]
        public string SerializeJsonApi()
        {
            var data = GetTestCase();

            return JsonSerializer.Serialize(data.Value, jsonApiOptions);
        }

        [Benchmark]
        [BenchmarkCategory(Serialize, NoJsonApi)]
        public string SerializeNewtonsoftNoJsonApi()
        {
            var data = GetTestCase();

            return JsonConvert.SerializeObject(data.Value, settings);
        }

        [Benchmark]
        [BenchmarkCategory(Serialize, JsonApi)]
        public string SerializeNewtonsoftJsonApi()
        {
            var data = GetTestCase();

            return JsonConvert.SerializeObject(data.Value, jsonApiSettings);
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(Deserialize, NoJsonApi)]
        public object DeserializeNoJsonApi()
        {
            var data = GetTestCase();

            return JsonSerializer.Deserialize(data.JsonBytes, data.Type, options);
        }

        [Benchmark]
        [BenchmarkCategory(Deserialize, JsonApi)]
        public object DeserializeJsonApi()
        {
            var data = GetTestCase();

            return JsonSerializer.Deserialize(data.JsonApiBytes, data.Type, jsonApiOptions);
        }

        [Benchmark]
        [BenchmarkCategory(Deserialize, NoJsonApi)]
        public object DeserializeNewtonsoftNoJsonApi()
        {
            var data = GetTestCase();

            return JsonConvert.DeserializeObject(data.Json, data.Type, settings);
        }

        [Benchmark]
        [BenchmarkCategory(Deserialize, JsonApi)]
        public object DeserializeNewtonsoftJsonApi()
        {
            var data = GetTestCase();

            return JsonConvert.DeserializeObject(data.Json, data.Type, jsonApiSettings);
        }

        private Data GetTestCase()
        {
            return TestData.Cases[Case];
        }

        public static class TestData
        {
            public static readonly Dictionary<string, Data> Cases = new()
            {
                {"Simple", GetSimple()},
                {"Compound", GetCompound()},
                {"LargeCompound", GetLargeCompound()},
                {"SingleError", GetError()},
                {"ErrorCollection", GetErrorCollection()},
                {"Document", GetDocument()}
            };

            private static Data GetSimple()
            {
                var model = AutoFaker.Generate<ManyTypesModel>();

                return new Data(model);
            }

            private static Data GetCompound()
            {
                var author = AutoFaker.Generate<Author>();
                var comment = new AutoFaker<Comment>()
                    .RuleFor(x => x.Author, author);

                var article = new AutoFaker<ArticleWithAuthor>()
                    .RuleFor(x => x.Author, author)
                    .RuleFor(x => x.Comments, _ => comment.Generate(2).ToArray())
                    .Generate();

                return new Data(article);
            }

            private static Data GetLargeCompound()
            {
                var comment = new AutoFaker<Comment>();
                var articlesArray = new AutoFaker<ArticleWithAuthor>()
                    .RuleFor(x => x.Comments, _ => comment.GenerateBetween(2, 5).ToArray())
                    .Generate(20)
                    .ToArray();

                return new Data(articlesArray);
            }

            private static Data GetError()
            {
                var error = new JsonApiError
                {
                    Id = "1",
                    Links = new JsonApiErrorLinks
                    {
                        {"next", new JsonApiLink {Href = "http://next"}},
                        {"about", new JsonApiLink {Href = "http://about"}},
                    },
                    Status = "422",
                    Code = "Invalid",
                    Title = "Invalid Attribute",
                    Detail = "First name must contain at least three characters.",
                    Source = new JsonApiErrorSource
                    {
                        Pointer = "/data/attributes/firstName"
                    },
                    Meta = new JsonApiMeta
                    {
                        {"count", 10.ToElement()},
                        {"name", "first".ToElement()}
                    }
                };

                return new Data(error);
            }

            private static Data GetErrorCollection()
            {
                var errors = new[]
                {
                    new JsonApiError
                    {
                        Id = "1",
                        Links = new JsonApiErrorLinks
                        {
                            {"next", new JsonApiLink {Href = "http://next"}},
                            {"about", new JsonApiLink {Href = "http://about"}},
                        },
                        Status = "422",
                        Code = "Invalid",
                        Title = "Invalid Attribute",
                        Detail = "First name must contain at least three characters.",
                        Source = new JsonApiErrorSource
                        {
                            Pointer = "/data/attributes/firstName"
                        },
                        Meta = new JsonApiMeta
                        {
                            {"count", 10.ToElement()},
                            {"name", "first".ToElement()}
                        }
                    },
                    new JsonApiError
                    {
                        Id = "2",
                        Links = new JsonApiErrorLinks
                        {
                            {"next", new JsonApiLink {Href = "http://next2"}},
                            {"about", new JsonApiLink {Href = "http://about2"}},
                        },
                        Status = "522",
                        Code = "Inconceivable",
                        Title = "Inconceivable Attribute",
                        Detail = "Princess bride must be watched.",
                        Source = new JsonApiErrorSource
                        {
                            Pointer = "/data/attributes/princess"
                        },
                        Meta = new JsonApiMeta
                        {
                            {"count", 10.ToElement()}
                        }
                    }
                };

                return new Data(errors);
            }

            private static Data GetDocument()
            {
                var document = new JsonApiDocument
                {
                    Data = new[]
                    {
                        new JsonApiResource
                        {
                            Id = "1",
                            Type = "articles",
                            Attributes = new Dictionary<string, JsonElement>
                            {
                                {"title", "book".ToElement()}
                            },
                            Relationships = new Dictionary<string, JsonApiRelationship>
                            {
                                {
                                    "author", new JsonApiRelationship
                                    {
                                        Data = new[]
                                        {
                                            new JsonApiResourceIdentifier("9", "people")
                                        }
                                    }
                                }
                            }
                        }
                    },
                    Included = new[]
                    {
                        new JsonApiResource
                        {
                            Id = "9",
                            Type = "people",
                            Attributes = new Dictionary<string, JsonElement>
                            {
                                {"name", "Joe".ToElement()}
                            },
                            Relationships = new Dictionary<string, JsonApiRelationship>
                            {
                                {
                                    "author", new JsonApiRelationship
                                    {
                                        Data = new[]
                                        {
                                            new JsonApiResourceIdentifier("2", "people")
                                        }
                                    }
                                },
                                {
                                    "comments", new JsonApiRelationship
                                    {
                                        Data = new[]
                                        {
                                            new JsonApiResourceIdentifier("5", "comments")
                                        }
                                    }
                                }
                            },
                            Links = new JsonApiResourceLinks
                            {
                                Self = "http://example.com/comments/5"
                            }
                        },
                        new JsonApiResource
                        {
                            Id = "5",
                            Type = "comments",
                            Attributes = new Dictionary<string, JsonElement>
                            {
                                {"body", "first".ToElement()}
                            },
                            Relationships = new Dictionary<string, JsonApiRelationship>
                            {
                                {
                                    "tags", new JsonApiRelationship
                                    {
                                        Links = new JsonApiRelationshipLinks
                                        {
                                            Self = "/tags"
                                        },
                                        Meta = new JsonApiMeta
                                        {
                                            {"count", 5.ToElement()}
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                return new Data(document);
            }
        }
    }
}
