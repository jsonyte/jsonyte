using System.Text.Json;
using JsonApi;
using JsonApi.Converters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace JsonApiWeb
{
    public class Article : IResource
    {
        public string Id { get; set; }

        public string Type { get; } = "articles";

        public string Title { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            const string json = @"
{
  ""data"": {
    ""type"": ""articles"",
    ""id"": ""1"",
    ""attributes"": {
      ""title"": ""Rails is Omakase""
    }
  }
}";

            var options = new JsonSerializerOptions
            {
                Converters = {new JsonApiConverterFactory()}
            };

            //var article = JsonSerializer.Deserialize<Article>(json, options);

            int r = 1;

            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(x =>
                {
                    x.UseUrls("http://localhost:8080");
                    x.UseStartup<Startup>();
                })
                .Build()
                .Run();
        }
    }
}
