using System.Text.Json;
using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests.Reflection
{
    public class ResourceAccessorTests
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
        public void InitOnlyPropertiesAreNotDeserialized()
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
            Assert.Equal("value", model.InitTitle);
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
            //const string json = @"
            //    {
            //      'data': {
            //        'id': '1',
            //        'type': 'model',
            //        'attributes': {
            //          'title': null,
            //          'readOnlyTitle': 'value',
            //          'writeOnlyTitle': null,
            //          'count': 0,
            //          'nullableCount': null,
            //          'readOnlyCount': 5,
            //          'writeOnlyCount': 0
            //        }
            //      }
            //    }";

            const string json = @"
                {
                    'id': '4',
                    'type': 'newType',
                    'title': null,
                    'readOnlyTitle': null,
                    'writeOnlyTitle': null,
                    'count': 0,
                    'nullableCount': null,
                    'readOnlyCount': 10,
                    'writeOnlyCount': 0
                }";

            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true
            };

            var model = json.Deserialize<ModelWithPropertyVisibilities>(options);

            var writeOnlyTitle = model.GetValue<string>(nameof(ModelWithPropertyVisibilities.WriteOnlyTitle));
            var writeOnlyCount = model.GetValue<int>(nameof(ModelWithPropertyVisibilities.WriteOnlyCount));

            Assert.Equal("4", model.Id);
            Assert.Equal("newType", model.Type);
            Assert.Equal("value", model.Title);
            Assert.Equal("value", model.ReadOnlyTitle);
            Assert.Equal("value", writeOnlyTitle);
            Assert.Equal(0, model.Count);
            Assert.Equal(5, model.NullableCount);
            Assert.Equal(5, model.ReadOnlyCount);
            Assert.Equal(0, writeOnlyCount);
        }
    }
}
