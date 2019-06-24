using System.Collections.Generic;
using Newtonsoft.Json;

namespace Jsonapi.Extensions
{
    public static class JsonReaderExtensions
    {
        public static IEnumerable<string> GetMembers(this JsonReader reader)
        {
            if (reader.TokenType == JsonToken.None)
            {
                reader.Read();
            }

            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new JsonApiException($"Expected start of object at {reader.Path}");
            }

            reader.Read();

            while (reader.TokenType != JsonToken.EndObject)
            {
                var member = reader.Value?.ToString();

                reader.Read();

                yield return member;

                reader.Read();
            }
        }

        public static IEnumerable<object> GetValues(this JsonReader reader)
        {
            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new JsonApiException($"Expected start of array at {reader.Path}");
            }

            reader.Read();

            while (reader.TokenType != JsonToken.EndArray)
            {
                yield return reader.Value;

                reader.Read();
            }
        }
    }
}
