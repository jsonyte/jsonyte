using System.Text;

namespace Jsonyte
{
    internal static class StringExtensions
    {
        public static byte[] ToByteArray(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }
    }
}
