using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Bogus;
using JsonApi.Tests.Performance;

namespace JsonApi.Profiling
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = new JsonSerializerOptions();
            options.AddJsonApi();

            Randomizer.Seed = new Random(56178921);

            var model = SerializeDeserializeBenchmarks.TestData.Cases["Compound"];

            var json = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(model.Value, options));

            var watch = Stopwatch.StartNew();

            for (var i = 0; i < 1000000; i++)
            {
                JsonSerializer.Deserialize(json, model.Type, options);
            }

            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }
    }
}
