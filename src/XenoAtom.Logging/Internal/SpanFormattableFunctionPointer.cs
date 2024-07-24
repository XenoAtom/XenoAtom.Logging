// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace XenoAtom.Logging.Internal;

internal readonly unsafe struct SpanFormattableFunctionPointer(delegate*<ref byte, Span<char>, out int, ReadOnlySpan<char>, IFormatProvider?, bool> tryFormat)
{
    public readonly delegate*<ref byte, Span<char>, out int, ReadOnlySpan<char>, IFormatProvider?, bool> TryFormat = tryFormat;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SpanFormattableFunctionPointer Instance<T>() where T : unmanaged, ISpanFormattable
    {
        return new(&TryFormatImpl);

        static bool TryFormatImpl(ref byte value, Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? formatProvider = null)
            => Unsafe.As<byte, T>(ref value).TryFormat(destination, out charsWritten, format, formatProvider);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SpanFormattableFunctionPointer BoolInstance()
    {
        return new(&TryFormatImpl);

        static bool TryFormatImpl(ref byte value, Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? formatProvider = null)
        {
            var b = Unsafe.As<byte, bool>(ref value);
            var str = b ? bool.TrueString : bool.FalseString;
            if (destination.Length < str.Length)
            {
                charsWritten = 0;
                return false;
            }
            str.AsSpan().CopyTo(destination);
            charsWritten = str.Length;
            return true;
        }
    }
}