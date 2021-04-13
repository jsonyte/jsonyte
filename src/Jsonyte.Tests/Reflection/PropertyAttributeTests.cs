using System;
using Jsonyte.Tests.Models;
using Xunit;

namespace Jsonyte.Tests.Reflection
{
    public class PropertyAttributeTests
    {
        [Fact]
        public void DuplicatePropertyNamesThrowsWhenSerializing()
        {
            var model = new ModelWithDuplicateProperties
            {
                Id = "1",
                Type = "type",
                Name = "abc",
                AlsoName = "def"
            };

            var exception = Record.Exception(() => model.Serialize());

            Assert.IsType<InvalidOperationException>(exception);
            Assert.Contains("contains duplicate property names", exception.Message);
        }

        [Fact]
        public void DuplicatePropertyNamesThrowsWhenDeserializing()
        {
            const string json = @"
                {
                  'data': {
                    'id': '4',
                    'type': 'newType',
                    'attributes': {
                      'name': 'abc'
                    }
                  }
                }";

            var exception = Record.Exception(() => json.Deserialize<ModelWithDuplicateProperties>());

            Assert.IsType<InvalidOperationException>(exception);
            Assert.Contains("contains duplicate property names", exception.Message);
        }
    }
}
