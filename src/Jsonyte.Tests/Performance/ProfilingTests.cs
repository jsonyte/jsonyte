using System.Text.Json;
using Xunit;

namespace Jsonyte.Tests.Performance
{
    public class ProfilingTests
    {
        private Data data;

        public ProfilingTests()
        {
            data = TestCases.Get("Compound");
        }

        [Fact(Skip = "Used for performance profiling")]
        public void Deserializing()
        {
            var options = new JsonSerializerOptions();
            options.AddJsonApi();

            for (var i = 0; i < 100000; i++)
            {
                JsonSerializer.Deserialize(data.JsonApiBytes, data.Type, options);
            }
        }

        [Fact(Skip = "Used for performance profiling")]
        public void Serializing()
        {
            var options = new JsonSerializerOptions();
            options.AddJsonApi();

            for (var i = 0; i < 100000; i++)
            {
                JsonSerializer.Serialize(data.Value, data.Type, options);
            }
        }
    }
}
