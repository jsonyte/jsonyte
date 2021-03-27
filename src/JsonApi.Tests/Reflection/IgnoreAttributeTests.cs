﻿using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Tests.Models;
using Xunit;

namespace JsonApi.Tests.Reflection
{
    public class IgnoreAttributeTests
    {
        [Fact]
        public void SerializingPropertiesWhenDefaultIgnoreIsNever()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            };

            var model = new ModelWithIgnoredProperties();

            var json = model.Serialize(options);

            Assert.Equal(@"
                {
                  'data': {
                    'type': 'type',
                    'attributes': {
                      'titleNeverIgnored': 'title',
                      'titleIgnoreDefault': 'title',
                      'titleIgnoreNull': 'title',
                      'countNeverIgnored': 5,
                      'countIgnoreDefault': 5,
                      'countIgnoreNull': 5,
                      'nullableCountNeverIgnored': 5,
                      'nullableCountIgnoreDefault': 5,
                      'nullableCountIgnoreNull': 5
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }

        [Fact]
        public void SerializingPropertiesWhenDefaultIgnoreIsDefault()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            var model = new ModelWithIgnoredProperties();

            var json = model.Serialize(options);

            Assert.Equal(@"
                {
                  'data': {
                    'type': 'type',
                    'attributes': {
                      'titleNeverIgnored': 'title',
                      'titleIgnoreDefault': 'title',
                      'titleIgnoreNull': 'title',
                      'countNeverIgnored': 5,
                      'countIgnoreDefault': 5,
                      'countIgnoreNull': 5,
                      'nullableCountNeverIgnored': 5,
                      'nullableCountIgnoreDefault': 5,
                      'nullableCountIgnoreNull': 5
                    }
                  }
                }".Format(), json, JsonStringEqualityComparer.Default);
        }
    }
}
