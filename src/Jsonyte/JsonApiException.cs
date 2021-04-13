using System.Text.Json;

namespace Jsonyte
{
    public class JsonApiException : JsonException
    {
        public JsonApiException(string? message)
            : base(message)
        {
        }
    }
}
