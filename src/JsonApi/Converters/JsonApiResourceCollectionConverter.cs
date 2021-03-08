using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonApi.Serialization;

namespace JsonApi.Converters
{
    internal class JsonApiResourceCollectionConverter<T, TElement> : JsonConverter<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.IsDocument())
            {
                var document = JsonSerializer.Deserialize<JsonApiDocument<T>>(ref reader, options);

                if (document == null)
                {
                    throw new JsonApiException("Invalid JSON:API document");
                }

                return document.Data;
            }

            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            var info = options.GetClassInfo(typeof(T));

            var resources = new List<TElement>();

            ReadResources(ref reader, options, resources);

            return (T) GetInstance(info, resources);
        }

        public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        private void ReadResources(ref Utf8JsonReader reader, JsonSerializerOptions options, List<TElement> resources)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonApiException("Invalid JSON:API resource");
            }

            reader.Read();

            while (reader.TokenType != JsonTokenType.EndArray)
            {
                var resource = JsonSerializer.Deserialize<TElement>(ref reader, options);

                if (resource != null)
                {
                    resources.Add(resource);
                }

                reader.Read();
            }
        }

        private object GetInstance(JsonClassInfo info, List<TElement> resources)
        {
            if (info.ClassType == JsonClassType.Array)
            {
                return resources.ToArray();
            }

            return resources;
        }
    }
}
