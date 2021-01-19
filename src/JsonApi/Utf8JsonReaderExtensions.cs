using System.Text.Json;

namespace JsonApi
{
    internal static class Utf8JsonReaderExtensions
    {
        public static bool IsDocument(this Utf8JsonReader reader)
        {
            if (reader.CurrentDepth > 0)
            {
                return false;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                return false;
            }

            return true;
        }
    }
}
