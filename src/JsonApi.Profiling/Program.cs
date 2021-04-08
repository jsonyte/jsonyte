using System;
using System.Text.Json;
using AutoBogus;
using Bogus;

namespace JsonApi.Profiling
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = new JsonSerializerOptions();
            options.AddJsonApi();

            Randomizer.Seed = new Random(56178921);

            var model = AutoFaker.Generate<ManyTypesModel>();

            for (var i = 0; i < 1000000; i++)
            {
                JsonSerializer.Serialize(model, options);
            }
        }
    }
}
