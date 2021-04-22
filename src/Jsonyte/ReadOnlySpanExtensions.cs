using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Jsonyte
{
    internal static class ReadOnlySpanExtensions
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

        /// <summary>
        /// Takes the first 7 bytes from the name plus the length.
        ///
        /// Code is borrowed from JsonClassInfo in System.Text.Json
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetKey(this ReadOnlySpan<byte> value)
        {
            var length = value.Length;
            ref var reference = ref MemoryMarshal.GetReference(value);

            if (length > 7)
            {
                var key = Unsafe.ReadUnaligned<ulong>(ref reference) & 0x00ffffffffffffffL;
                key |= (ulong) Math.Min(length, 0xff) << 56;

                return key;
            }

            if (length == 7)
            {
                var key = (ulong) Unsafe.ReadUnaligned<uint>(ref reference);
                key |= (ulong) Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref reference, 4)) << 32;
                key |= (ulong) Unsafe.Add(ref reference, 6) << 48;
                key |= (ulong) length << 56;

                return key;
            }

            if (length == 6)
            {
                var key = (ulong) Unsafe.ReadUnaligned<uint>(ref reference);
                key |= (ulong) Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref reference, 4)) << 32;
                key |= (ulong) length << 56;

                return key;
            }

            if (length == 5)
            {
                var key = (ulong) Unsafe.ReadUnaligned<uint>(ref reference);
                key |= (ulong) Unsafe.Add(ref reference, 4) << 32;
                key |= (ulong) length << 56;

                return key;
            }

            if (length == 4)
            {
                var key = (ulong) Unsafe.ReadUnaligned<uint>(ref reference);
                key |= (ulong) length << 56;

                return key;
            }

            if (length == 3)
            {
                var key = (ulong) Unsafe.ReadUnaligned<ushort>(ref reference);
                key |= (ulong) Unsafe.Add(ref reference, 2) << 16;
                key |= (ulong) length << 56;

                return key;
            }

            if (length == 2)
            {
                var key = (ulong) Unsafe.ReadUnaligned<ushort>(ref reference);
                key |= (ulong) length << 56;

                return key;
            }

            if (length == 1)
            {
                var key = (ulong) reference;
                key |= (ulong) length << 56;

                return key;
            }

            return 0;
        }
    }
}
