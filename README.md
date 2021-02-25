# Jsonapi

[![NuGet](https://img.shields.io/nuget/v/Jsonapi?style=for-the-badge)](https://www.nuget.org/packages/Jsonapi) [![Build](https://img.shields.io/github/workflow/status/robertcoltheart/Jsonapi/build?style=for-the-badge)](https://github.com/robertcoltheart/Jsonapi/actions?query=workflow:build) [![License](https://img.shields.io/github/license/robertcoltheart/Jsonapi?style=for-the-badge)](https://github.com/robertcoltheart/Jsonapi/blob/master/LICENSE)

A library for serializing and deserializing [JSON:API](https://jsonapi.org) documents using `System.Text.Json`.

## Usage
Install the package from NuGet with `dotnet add package Jsonapi`.

```csharp
var options = new JsonSerializerOptions();
options.AddJsonApi();

var json = JsonSerializer.Serialize(new Article(), options);
var article = JsonSerializer.Deserialize(json, options);
```

## Contributing
Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on how to contribute to this project.

## License
Jsonapi is released under the [MIT License](LICENSE)
