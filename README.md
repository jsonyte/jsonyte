# Jsonyte

[![NuGet](https://img.shields.io/nuget/v/Jsonapi?style=for-the-badge)](https://www.nuget.org/packages/Jsonyte) [![Build](https://img.shields.io/github/workflow/status/jsonyte/jsonyte/build?style=for-the-badge)](https://github.com/jsonyte/jsonyte/actions?query=workflow:build) [![License](https://img.shields.io/github/license/jsonyte/jsonyte?style=for-the-badge)](https://github.com/jsonyte/jsonyte/blob/master/LICENSE)

A library for serializing and deserializing [JSON:API](https://jsonapi.org) documents using `System.Text.Json`.

## Usage
Install the package from NuGet with `dotnet add package Jsonyte`.

```csharp
var options = new JsonSerializerOptions();
options.AddJsonApi();

var json = JsonSerializer.Serialize(new Article(), options);
var article = JsonSerializer.Deserialize(json, options);
```

## Contributing
Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on how to contribute to this project.

## License
Jjsonyte is released under the [MIT License](LICENSE)
