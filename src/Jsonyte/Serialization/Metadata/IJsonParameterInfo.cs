﻿using System.Text.Json;

namespace Jsonyte.Serialization.Metadata
{
    internal interface IJsonParameterInfo
    {
        int Position { get; }

        object? Read(ref Utf8JsonReader reader);
    }
}
