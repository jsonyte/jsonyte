# Jsonyte

[![Docs](https://img.shields.io/badge/docs-wiki-blue.svg?style=for-the-badge)](https://github.com/jsonyte/jsonyte/wiki) [![NuGet](https://img.shields.io/nuget/v/Jsonyte?style=for-the-badge)](https://www.nuget.org/packages/Jsonyte) [![Build](https://img.shields.io/github/workflow/status/jsonyte/jsonyte/build?style=for-the-badge)](https://github.com/jsonyte/jsonyte/actions?query=workflow:build) [![Discussions](https://img.shields.io/badge/DISCUSS-ON%20GITHUB-orange?style=for-the-badge)](https://github.com/jsonyte/jsonyte/discussions) [![License](https://img.shields.io/github/license/jsonyte/jsonyte?style=for-the-badge)](https://github.com/jsonyte/jsonyte/blob/master/LICENSE)

A library for serializing and deserializing [JSON:API](https://jsonapi.org) documents using `System.Text.Json`.

## Usage
Install the package from NuGet with `dotnet add package Jsonyte`.

```csharp
var options = new JsonSerializerOptions();
options.AddJsonApi();

var json = JsonSerializer.Serialize(new Article(), options);
var article = JsonSerializer.Deserialize(json, options);
```

For an ASP.NET Core application, simply add the following to the startup:
```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddControllers()
            .AddJsonOptions(x => x.JsonSerializerOptions.AddJsonApi());
    }
}
```

## Quick start
Jsonyte will serialize and deserialize plain C# objects to and from the [JSON:API](https://jsonapi.org) format.

For example, the below model will serialize and deserialize the corresponding JSON:API document:

```csharp
public class Article
{
    public string Id { get; set; } = "1";

    // Required property
    public string Type { get; set; } = "articles";

    public string Title { get; set; } = "JSON:API";
}
```

```json
{
  "data": {
    "type": "articles",
    "id": "1",
    "attributes": {
      "title": "JSON:API"
    }
  }
}
```

## Documentation
See the [wiki](https://github.com/jsonyte/jsonyte/wiki) for examples and help using Jsonyte.

## Get in touch
Discuss with us on [Discussions](https://github.com/jsonyte/jsonyte/discussions), or raise an [issue](https://github.com/jsonyte/jsonyte/issues).

[![Discussions](https://img.shields.io/badge/DISCUSS-ON%20GITHUB-orange?style=for-the-badge)](https://github.com/jsonyte/jsonyte/discussions)

## Contributing
Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on how to contribute to this project.

## Acknowledgements
Inspired by the excellent [JsonApiSerializer](https://github.com/codecutout/JsonApiSerializer)

## License
Jsonyte is released under the [MIT License](LICENSE)
