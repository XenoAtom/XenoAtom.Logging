// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Buffers;
using System.Runtime.InteropServices;

namespace XenoAtom.Logging.Helpers;

public ref struct LogStringBuffer
{
    private byte[] _byteBuffer;
    private int _byteCount;

    [Obsolete("Use LogStringBuffer(int minimumSize) instead", error: true)]
    public LogStringBuffer()
    {
    }

    public LogStringBuffer(int minimumSize)
    {
        _byteBuffer = ArrayPool<byte>.Shared.Rent(minimumSize);
        _byteCount = 0;
    }

    public ReadOnlySpan<char> UnsafeAsSpan() => MemoryMarshal.Cast<byte, char>(_byteBuffer.AsSpan(0, _byteCount));

    public void Append(ReadOnlySpan<char> text)
    {
        // Char directly to byte[] without encoding
        var textBytes = MemoryMarshal.Cast<char, byte>(text);
        var byteBuffer = _byteBuffer;
        var byteCount = _byteCount + textBytes.Length;
        if (byteBuffer.Length < byteCount)
        {
            var newByteBuffer = ArrayPool<byte>.Shared.Rent(byteCount * 2);
            byteBuffer.AsSpan(0, _byteCount).CopyTo(newByteBuffer);
            ArrayPool<byte>.Shared.Return(byteBuffer);
            byteBuffer = newByteBuffer;
            _byteBuffer = byteBuffer;
        }

        textBytes.CopyTo(byteBuffer.AsSpan(_byteCount));
        _byteCount = byteCount;
    }

    public void Dispose()
    {
        var byteBuffer = _byteBuffer;
        if (byteBuffer != null)
        {
            ArrayPool<byte>.Shared.Return(byteBuffer);
            byteBuffer = null!;
        }
    }
}