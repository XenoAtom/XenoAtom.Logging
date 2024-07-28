// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Buffers;
using System.Runtime.InteropServices;

namespace XenoAtom.Logging.Helpers;

public ref struct LogFormatterBuffer
{
    private byte[]? _charBuffer;

    internal const int DefaultFormatterBufferSize = 16384;

    public ReadOnlySpan<char> Format(in LogMessage logMessage, LogFormatter formatter, ref LogMessageFormatSegments segments)
    {
        _charBuffer ??= ArrayPool<byte>.Shared.Rent(DefaultFormatterBufferSize);
        var buffer = _charBuffer;
        Span<byte> span = buffer;
        int charsWritten;
        while (!formatter.TryFormat(logMessage, MemoryMarshal.Cast<byte, char>(span), out charsWritten, ref segments))
        {
            ArrayPool<byte>.Shared.Return(buffer);
            buffer = ArrayPool<byte>.Shared.Rent(span.Length * 2);
            span = buffer;
        }

        return MemoryMarshal.Cast<byte, char>(span).Slice(0, charsWritten);
    }

    public void Dispose()
    {
        if (_charBuffer != null)
        {
            ArrayPool<byte>.Shared.Return(_charBuffer);
            _charBuffer = null;
        }
    }
}