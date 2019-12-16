using System.Linq;

namespace JsonApi.AspNetCore.Extensions
{
    public static class StringExtensions
    {
        public static string Dasherize(this string value)
        {
            string Selector(char x, int i)
            {
                return i > 0 && char.IsUpper(x)
                    ? "-" + x
                    : x.ToString();
            }

            return string.Concat(value.Select(Selector)).ToLower();
        }
    }
}