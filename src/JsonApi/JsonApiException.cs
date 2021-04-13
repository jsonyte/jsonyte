using System.Text.Json;

namespace JsonApi
{
    public class JsonApiException : JsonException
    {
        public JsonApiException(string? message)
            : base(message)
        {
        }
    }
}
