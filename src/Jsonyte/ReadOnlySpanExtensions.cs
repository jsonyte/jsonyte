using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace JsonApi
{
    public static class ReadOnlySpanExtensions
    {
        public static string GetString(this ref ReadOnlySpan<byte> value)
        {
#if NETSTANDARD || NETFRAMEWORK
            var bytes = value.ToArray();
#else
            var bytes = value;
#endif

            return Encoding.UTF8.GetString(bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEqual(this ref ReadOnlySpan<byte> source, JsonEncodedText value)
        {
            return source.SequenceEqual(value.EncodedUtf8Bytes);
        }
    }
}
