// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Buffers;
using System.Runtime.InteropServices;

namespace XenoAtom.Logging.Helpers;

/// <summary>
/// Provides a pooled character buffer used by <see cref="LogFormatter"/> implementations.
/// </summary>
public ref struct LogFormatterBuffer
{
    private byte[]? _charBuffer;

    internal const int DefaultFormatterBufferSize = 16384;

    /// <summary>
    /// Formats a message into the pooled buffer using the provided formatter.
    /// </summary>
    /// <param name="logMessage">The message to format.</param>
    /// <param name="formatter">The formatter used to render text.</param>
    /// <param name="segments">Associated formatting segments.</param>
    /// <returns>A span over the formatted characters.</returns>
    public ReadOnlySpan<char> Format(LogMessage logMessage, LogFormatter formatter, ref LogMessageFormatSegments segments)
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

        _charBuffer = buffer;
        return MemoryMarshal.Cast<byte, char>(span).Slice(0, charsWritten);
    }

    /// <summary>
    /// Returns any rented buffers to the shared pool.
    /// </summary>
    public void Dispose()
    {
        if (_charBuffer != null)
        {
            ArrayPool<byte>.Shared.Return(_charBuffer);
            _charBuffer = null;
        }
    }
}
