namespace JsonApi
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var characters = value.ToCharArray();
            characters[0] = char.ToLower(characters[0]);

            return new string(characters);
        }
    }
}