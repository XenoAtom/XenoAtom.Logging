// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Buffers;
using System.Text;

namespace XenoAtom.Logging.Helpers;

/// <summary>
/// Provides a pooled byte buffer used to encode text for output writers.
/// </summary>
public ref struct LogEncoderBuffer
{
    private byte[]? _byteBuffer;

    /// <summary>
    /// Encodes UTF-16 text into bytes using the specified <paramref name="encoding"/>.
    /// </summary>
    /// <param name="text">The text to encode.</param>
    /// <param name="encoding">The target encoding.</param>
    /// <returns>A span over the encoded bytes.</returns>
    public ReadOnlySpan<byte> Encode(ReadOnlySpan<char> text, Encoding encoding)
    {
        var byteBuffer = _byteBuffer;
        var byteCount = encoding.GetByteCount(text);
        if (byteBuffer is not null && byteBuffer.Length < byteCount)
        {
            ArrayPool<byte>.Shared.Return(byteBuffer);
            byteBuffer = null;
        }
        byteBuffer ??= ArrayPool<byte>.Shared.Rent(byteCount);
        encoding.GetBytes(text, byteBuffer);
        _byteBuffer = byteBuffer;
        return new ReadOnlySpan<byte>(byteBuffer, 0, byteCount);
    }

    /// <summary>
    /// Returns any rented buffers to the shared pool.
    /// </summary>
    public void Dispose()
    {
        var byteBuffer = _byteBuffer;
        if (byteBuffer != null)
        {
            ArrayPool<byte>.Shared.Return(byteBuffer);
            _byteBuffer = null;
        }
    }
}
