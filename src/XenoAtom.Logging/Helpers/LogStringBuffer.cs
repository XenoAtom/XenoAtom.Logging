// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Buffers;
using System.Runtime.InteropServices;

namespace XenoAtom.Logging.Helpers;

/// <summary>
/// Provides a pooled UTF-16 character buffer for incremental text construction.
/// </summary>
public ref struct LogStringBuffer
{
    private const int DefaultMinimumSize = 256;
    private byte[]? _byteBuffer;
    private int _byteCount;

    /// <summary>
    /// Initializes a new buffer with a default minimum capacity.
    /// </summary>
    public LogStringBuffer()
        : this(DefaultMinimumSize)
    {
    }

    /// <summary>
    /// Initializes a new buffer with at least <paramref name="minimumSize"/> bytes.
    /// </summary>
    /// <param name="minimumSize">The minimum initial byte capacity.</param>
    public LogStringBuffer(int minimumSize)
    {
        _byteBuffer = ArrayPool<byte>.Shared.Rent(minimumSize);
        _byteCount = 0;
    }

    /// <summary>
    /// Returns a span over the currently written characters.
    /// </summary>
    /// <returns>A span over buffered characters.</returns>
    /// <exception cref="ObjectDisposedException">The buffer has been disposed.</exception>
    public ReadOnlySpan<char> UnsafeAsSpan()
    {
        var byteBuffer = _byteBuffer ?? throw new ObjectDisposedException(nameof(LogStringBuffer));
        return MemoryMarshal.Cast<byte, char>(byteBuffer.AsSpan(0, _byteCount));
    }

    /// <summary>
    /// Appends text to the buffer.
    /// </summary>
    /// <param name="text">The text to append.</param>
    /// <exception cref="ObjectDisposedException">The buffer has been disposed.</exception>
    public void Append(scoped ReadOnlySpan<char> text)
    {
        var byteBuffer = _byteBuffer ?? throw new ObjectDisposedException(nameof(LogStringBuffer));

        var textBytes = MemoryMarshal.Cast<char, byte>(text);
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

    /// <summary>
    /// Returns any rented buffers to the shared pool.
    /// </summary>
    public void Dispose()
    {
        var byteBuffer = _byteBuffer;
        if (byteBuffer is null)
        {
            return;
        }

        ArrayPool<byte>.Shared.Return(byteBuffer);
        _byteBuffer = null;
        _byteCount = 0;
    }
}
