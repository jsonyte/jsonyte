namespace Jsonyte
{
    /// <summary>
    /// Represents a JSON pointer <see href="https://tools.ietf.org/html/rfc6901">RFC6901</see> to an associated entity in a request document.
    /// </summary>
    /// <example>
    /// The pointer to an entity in the request document may look like the below:
    ///
    /// <list type="bullet">
    /// <item>
    /// <term>/data</term>
    /// <description>A pointer to primary data</description>
    /// </item>
    /// <item>
    /// <term>/data/attributes/title</term>
    /// <description>A pointer to a specific attribute</description>
    /// </item>
    /// </list>
    /// </example>
    public sealed class JsonApiPointer
    {
        private readonly string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiPointer"/> class by using value represented by the specified string.
        /// </summary>
        /// <param name="value">A string containing a JSON pointer <see href="https://tools.ietf.org/html/rfc6901">RFC6901</see> to an associated entity in a request document</param>
        public JsonApiPointer(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// Converts a string containing a JSON pointer <see href="https://tools.ietf.org/html/rfc6901">RFC6901</see> to an <see cref="JsonApiPointer"/>.
        /// </summary>
        /// <param name="value">A string that contains a JSON pointer.</param>
        public static implicit operator JsonApiPointer(string value)
        {
            return new(value);
        }

        /// <summary>
        /// Converts a <see cref="JsonApiPointer"/> containing a JSON pointer <see href="https://tools.ietf.org/html/rfc6901">RFC6901</see> to a string.
        /// </summary>
        /// <param name="value">A <see cref="JsonApiPointer"/> that contains a JSON pointer.</param>
        public static implicit operator string(JsonApiPointer value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Returns a string representation of the value of this instance of the <see cref="JsonApiPointer"/> class.
        /// </summary>
        /// <returns>The value of this <see cref="JsonApiPointer"/>.</returns>
        public override string ToString()
        {
            return value;
        }
    }
}
