using System.Collections.Generic;
using System.Text.Json;

namespace Jsonyte
{
    /// <summary>
    /// Represents a meta object containing non-standard meta-information about a <see href="https://jsonapi.org/">JSON:API</see> document or one of its members.
    /// </summary>
    public sealed class JsonApiMeta : Dictionary<string, JsonElement>
    {
        /// <summary>
        /// Creates a <see cref="JsonElement"/> that can be stored in the <see cref="JsonApiMeta"/> object.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonElement"/>.</param>
        /// <param name="options">An optional <see cref="JsonSerializerOptions"/> that is used to serialize the value.</param>
        /// <returns>A <see cref="JsonElement"/> value.</returns>
        public static JsonElement Value(object value, JsonSerializerOptions? options = null)
        {
            return Value(JsonSerializer.SerializeToUtf8Bytes(value, options));
        }

        /// <summary>
        /// Creates a <see cref="JsonElement"/> that can be stored in the <see cref="JsonApiMeta"/> object.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to be converted to a <see cref="JsonElement"/>.</param>
        /// <param name="options">An optional <see cref="JsonSerializerOptions"/> that is used to serialize the value.</param>
        /// <returns>A <see cref="JsonElement"/> value.</returns>
        public static JsonElement Value<T>(T value, JsonSerializerOptions? options = null)
        {
            return Value(JsonSerializer.SerializeToUtf8Bytes(value, options));
        }

        private static JsonElement Value(byte[] value)
        {
            using var document = JsonDocument.Parse(value);

            return document.RootElement.Clone();
        }
    }
}
