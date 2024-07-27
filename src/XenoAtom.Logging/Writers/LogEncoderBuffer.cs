// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Buffers;
using System.Text;

namespace XenoAtom.Logging.Writers;

public ref struct LogEncoderBuffer
{
    private byte[]? _byteBuffer;

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

    public void Dispose()
    {
        var byteBuffer = _byteBuffer;
        if (byteBuffer != null)
        {
            ArrayPool<byte>.Shared.Return(byteBuffer);
            byteBuffer = null;
        }
    }
}