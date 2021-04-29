using System.Text.Json;
using Jsonyte.Tests.Models;
using Xunit;

namespace Jsonyte.Tests.Reflection
{
    public class MemberAccessorTests
    {
        [Fact]
        public void ReadOnlyPropertiesAreNotSerialized()
        {
            var model = new ModelWithPropertyVisibilities();

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model',
                    'attributes': {
                      'title': 'value',
                      'readOnlyTitle': 'value',
                      'count': 5,
                      'nullableCount': 5,
                      'readOnlyCount': 5
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void WriteOnlyPropertiesAreNotDeserialized()
        {
            const string json = @"
                {
                  'data': {
                    'id': '4',
                    'type': 'newType',
                    'attributes': {
                      'title': 'newTitle',
                      'readOnlyTitle': 'newTitle',
                      'writeOnlyTitle': 'newTitle',
                      'count': 1,
                      'nullableCount': 1,
                      'readOnlyCount': 1,
                      'writeOnlyCount': 1
                    }
                  }
                }";

            var model = json.Deserialize<ModelWithPropertyVisibilities>();

            var writeOnlyTitle = model.GetValue<string>(nameof(ModelWithPropertyVisibilities.WriteOnlyTitle));
            var writeOnlyCount = model.GetValue<int>(nameof(ModelWithPropertyVisibilities.WriteOnlyCount));

            Assert.Equal("4", model.Id);
            Assert.Equal("newType", model.Type);
            Assert.Equal("newTitle", model.Title);
            Assert.Equal("value", model.ReadOnlyTitle);
            Assert.Equal("newTitle", writeOnlyTitle);
            Assert.Equal(1, model.Count);
            Assert.Equal(5, model.ReadOnlyCount);
            Assert.Equal(1, writeOnlyCount);
        }

        [Fact]
        public void CanSerializeNullValues()
        {
            var model = new ModelWithPropertyVisibilities
            {
                WriteOnlyTitle = null,
                Count = default,
                NullableCount = null,
                WriteOnlyCount = default,
                Title = null
            };

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model',
                    'attributes': {
                      'title': null,
                      'readOnlyTitle': 'value',
                      'count': 0,
                      'nullableCount': null,
                      'readOnlyCount': 5
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanDeserializeNullValues()
        {
            const string json = @"
                {
                  'data': {
                    'id': '1',
                    'type': 'model',
                    'attributes': {
                      'title': null,
                      'readOnlyTitle': 'value',
                      'writeOnlyTitle': null,
                      'count': 0,
                      'nullableCount': null,
                      'readOnlyCount': 5,
                      'writeOnlyCount': 0
                    }
                  }
                }";

            var model = json.Deserialize<ModelWithPropertyVisibilities>();

            var writeOnlyTitle = model.GetValue<string>(nameof(ModelWithPropertyVisibilities.WriteOnlyTitle));
            var writeOnlyCount = model.GetValue<int>(nameof(ModelWithPropertyVisibilities.WriteOnlyCount));

            Assert.Equal("1", model.Id);
            Assert.Equal("model", model.Type);
            Assert.Null(model.Title);
            Assert.Equal("value", model.ReadOnlyTitle);
            Assert.Null(writeOnlyTitle);
            Assert.Equal(0, model.Count);
            Assert.Null(model.NullableCount);
            Assert.Equal(5, model.ReadOnlyCount);
            Assert.Equal(0, writeOnlyCount);
        }

#if NET5_0_OR_GREATER
        [Fact]
        public void InitOnlyPropertiesAreDeserialized()
        {
            const string json = @"
                {
                  'data': {
                    'id': '4',
                    'type': 'newType',
                    'attributes': {
                      'initTitle': 'newTitle'
                    }
                  }
                }";

            var model = json.Deserialize<ModelWithInitProperty>();

            Assert.Equal("4", model.Id);
            Assert.Equal("newType", model.Type);
            Assert.Equal("newTitle", model.InitTitle);
        }

        [Fact]
        public void CanSerializeRecordTypes()
        {
            var model = new ModelRecord("1", "article", "Jsonapi");

            var json = model.Serialize();

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'article',
                    'attributes': {
                      'title': 'Jsonapi'
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact(Skip = "Disabled until constructor support is better")]
        public void CanDeserializeRecordTypes()
        {
            const string json = @"
                {
                  'data': {
                    'id': '1',
                    'type': 'article',
                    'attributes': {
                      'title': 'Jsonapi'
                    }
                  }
                }";

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var model = json.Deserialize<ModelRecord>(options);

            Assert.Equal("1", model.Id);
            Assert.Equal("article", model.Type);
            Assert.Equal("Jsonapi", model.Title);
        }
#endif

        [Fact]
        public void IgnoresReadOnlyPropertiesUsingOptions()
        {
            var options = new JsonSerializerOptions
            {
                IgnoreReadOnlyProperties = true
            };

            var model = new ModelWithPropertyVisibilities
            {
                WriteOnlyTitle = null,
                Count = default,
                NullableCount = null,
                WriteOnlyCount = default,
                Title = null
            };

            var json = model.Serialize(options);

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model',
                    'attributes': {
                      'title': null,
                      'count': 0,
                      'nullableCount': null
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void IgnoresReadOnlyRelationshipsUsingOptions()
        {
            var options = new JsonSerializerOptions
            {
                IgnoreReadOnlyProperties = true
            };

            var model = new ModelWithReadOnlyProperties
            {
                Id = "1",
                Type = "model"
            };

            var json = model.Serialize(options);

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model'
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void IgnoresReadOnlyFieldRelationshipsUsingOptions()
        {
            var options = new JsonSerializerOptions
            {
                IgnoreReadOnlyFields = true,
                IncludeFields = true
            };

            var model = new ModelWithReadOnlyFields
            {
                Id = "1",
                Type = "model"
            };

            var json = model.Serialize(options);

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model'
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void IgnoresNullValuesWhenSerializingUsingOptions()
        {
            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true
            };

            var model = new ModelWithPropertyVisibilities
            {
                WriteOnlyTitle = null,
                Count = default,
                NullableCount = null,
                WriteOnlyCount = default,
                Title = null
            };

            var json = model.Serialize(options);

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model',
                    'attributes': {
                      'readOnlyTitle': 'value',
                      'count': 0,
                      'readOnlyCount': 5
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void IgnoresNullValuesWhenDeserializingUsingOptions()
        {
            const string json = @"
                {
                  'data': {
                    'id': '1',
                    'type': 'model',
                    'attributes': {
                      'title': null,
                      'readOnlyTitle': 'value',
                      'writeOnlyTitle': null,
                      'count': 0,
                      'nullableCount': null,
                      'readOnlyCount': 5,
                      'writeOnlyCount': 0
                    }
                  }
                }";

            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true
            };

            var model = json.Deserialize<ModelWithPropertyVisibilities>(options);

            var writeOnlyTitle = model.GetValue<string>(nameof(ModelWithPropertyVisibilities.WriteOnlyTitle));
            var writeOnlyCount = model.GetValue<int>(nameof(ModelWithPropertyVisibilities.WriteOnlyCount));

            Assert.Equal("1", model.Id);
            Assert.Equal("model", model.Type);
            Assert.Equal("value", model.Title);
            Assert.Equal("value", model.ReadOnlyTitle);
            Assert.Equal("value", writeOnlyTitle);
            Assert.Equal(0, model.Count);
            Assert.Equal(5, model.NullableCount);
            Assert.Equal(5, model.ReadOnlyCount);
            Assert.Equal(0, writeOnlyCount);
        }

        [Fact]
        public void CanSerializeFields()
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true
            };

            var model = new ModelWithFields();

            var json = model.Serialize(options);

            Assert.Equal(@"
                {
                  'data': {
                    'type': 'type',
                    'attributes': {
                      'publicTitle': 'title',
                      'publicReadOnlyTitle': 'title',
                      'publicCount': 5,
                      'publicNullableCount': 5,
                      'publicReadOnlyCount': 5,
                      'publicReadOnlyNullableCount': 5
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeReadOnlyFields()
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                IgnoreReadOnlyFields = false
            };

            var model = new ModelWithReadOnlyFields();

            var json = model.Serialize(options);

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model',
                    'attributes': {
                      'intValue': 5,
                      'stringValue': 'str'
                    },
                    'relationships': {
                      'author': {
                        'data': {
                          'id': '4',
                          'type': 'author'
                        }
                      }
                    }
                  },
                  'included': [
                    {
                      'id': '4',
                      'type': 'author',
                      'attributes': {
                        'name': 'bob',
                        'twitter': 'bo'
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void CanSerializeReadOnlyProperties()
        {
            var options = new JsonSerializerOptions
            {
                IgnoreReadOnlyProperties = false
            };

            var model = new ModelWithReadOnlyProperties
            {
                Id = "1",
                Type = "model"
            };

            var json = model.Serialize(options);

            Assert.Equal(@"
                {
                  'data': {
                    'id': '1',
                    'type': 'model',
                    'attributes': {
                      'intValue': 5
                    },
                    'relationships': {
                      'author': {
                        'data': {
                          'id': '4',
                          'type': 'author'
                        }
                      }
                    }
                  },
                  'included': [
                    {
                      'id': '4',
                      'type': 'author',
                      'attributes': {
                        'name': 'Bob',
                        'twitter': null
                      }
                    }
                  ]
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void IgnoresReadOnlyFieldsUsingOptions()
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                IgnoreReadOnlyFields = true
            };

            var model = new ModelWithFields();

            var json = model.Serialize(options);

            Assert.Equal(@"
                {
                  'data': {
                    'type': 'type',
                    'attributes': {
                      'publicTitle': 'title',
                      'publicCount': 5,
                      'publicNullableCount': 5
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void IgnoresNullValuesInFieldsWhenSerializingUsingOptions()
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                IgnoreNullValues = true
            };

            var model = new ModelWithFields
            {
                PublicCount = 0,
                PublicNullableCount = null,
                PublicTitle = null,
                Type = "type"
            };

            var json = model.Serialize(options);

            Assert.Equal(@"
                {
                  'data': {
                    'type': 'type',
                    'attributes': {
                      'publicReadOnlyTitle': 'title',
                      'publicCount': 0,
                      'publicReadOnlyCount': 5,
                      'publicReadOnlyNullableCount': 5
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void IgnoresNullValuesInFieldsWhenDeserializingUsingOptions()
        {
            const string json = @"
                {
                  'data': {
                    'type': 'model',
                    'attributes': {
                      'readOnlyTitle': null,
                      'readWriteTitle': null,
                      'publicTitle': null,
                      'publicReadOnlyTitle': null,
                      'readWriteCount': 0,
                      'publicCount': 0,
                      'readWriteNullableCount': null,
                      'publicNullableCount': null,
                      'publicReadOnlyCount': 0,
                      'publicReadOnlyNullableCount': null
                    }
                  }
                }";

            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                IgnoreNullValues = true
            };

            var model = json.Deserialize<ModelWithFields>(options);

            Assert.Equal("model", model.Type);
            Assert.Equal("title", model.PublicTitle);
            Assert.Equal("title", model.PublicReadOnlyTitle);
            Assert.Equal(0, model.PublicCount);
            Assert.Equal(5, model.PublicReadOnlyCount);
            Assert.Equal(5, model.PublicNullableCount);
            Assert.Equal(5, model.PublicReadOnlyNullableCount);
        }
    }
}
