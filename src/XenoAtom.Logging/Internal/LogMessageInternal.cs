// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace XenoAtom.Logging;

internal sealed class LogMessageInternal
{
    private const int DefaultTextCapacity = 256;
    private const int DefaultMaxRetainedTextCapacity = 4096;

    private char[]? _textBuffer;
    private int _textLength;

    internal LogMessageInternal? PoolNext;
    internal int PoolState;

    public Logger Logger { get; private set; } = null!;
    public LogLevel Level { get; private set; }
    public DateTimeOffset Timestamp { get; private set; }
    public LogEventId EventId { get; private set; } = LogEventId.Empty;
    public Thread Thread { get; private set; } = null!;
    public Exception? Exception { get; private set; }
    public LogScopeSnapshot Scope { get; private set; } = LogScopeSnapshot.Empty;
    public LogPropertiesSnapshot Properties { get; private set; } = LogPropertiesSnapshot.Empty;
    public long SequenceId { get; set; }

    public IFormatProvider FormatProvider { get; private set; } = CultureInfo.InvariantCulture;

    public ReadOnlySpan<char> Text
        => _textBuffer is null ? ReadOnlySpan<char>.Empty : _textBuffer.AsSpan(0, _textLength);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Initialize(
        Logger logger,
        LogLevel level,
        DateTimeOffset timestamp,
        Thread thread,
        LogScopeSnapshot scope,
        LogEventId eventId,
        LogPropertiesSnapshot properties,
        Exception? exception,
        IFormatProvider formatProvider,
        int initialTextCapacity)
    {
        Logger = logger;
        Level = level;
        Timestamp = timestamp;
        Thread = thread;
        Scope = scope;
        EventId = eventId;
        Properties = properties;
        Exception = exception;
        FormatProvider = formatProvider;
        _textLength = 0;
        EnsureTextCapacity(initialTextCapacity);
    }

    public void Reset()
    {
        Scope.Dispose();
        Scope = LogScopeSnapshot.Empty;

        Properties.Dispose();
        Properties = LogPropertiesSnapshot.Empty;

        Logger = null!;
        Level = default;
        Timestamp = default;
        EventId = LogEventId.Empty;
        Thread = null!;
        Exception = null;
        FormatProvider = CultureInfo.InvariantCulture;
        _textLength = 0;
    }

    public void TrimRetainedTextBuffer(int? maxRetainedLength = null)
    {
        var buffer = _textBuffer;
        if (buffer is null)
        {
            return;
        }

        var limit = maxRetainedLength ?? DefaultMaxRetainedTextCapacity;
        if (limit <= 0)
        {
            ArrayPool<char>.Shared.Return(buffer);
            _textBuffer = null;
            _textLength = 0;
            return;
        }

        if (buffer.Length <= limit)
        {
            return;
        }

        ArrayPool<char>.Shared.Return(buffer);
        _textBuffer = null;
        _textLength = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
        {
            return;
        }

        EnsureTextCapacity(_textLength + value.Length);
        value.CopyTo(_textBuffer!.AsSpan(_textLength));
        _textLength += value.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(string value)
        => AppendLiteral(value.AsSpan());

    public void AppendLiteral(ReadOnlySpan<byte> utf8Value)
    {
        if (utf8Value.IsEmpty)
        {
            return;
        }

        var charCount = Encoding.UTF8.GetCharCount(utf8Value);
        EnsureTextCapacity(_textLength + charCount);
        _textLength += Encoding.UTF8.GetChars(utf8Value, _textBuffer!.AsSpan(_textLength));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(ReadOnlySpan<char> value, int alignment)
        => AppendAligned(value, alignment);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(string value, int alignment)
        => AppendAligned(value.AsSpan(), alignment);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(ReadOnlySpan<byte> utf8Value, int alignment)
    {
        if (utf8Value.IsEmpty)
        {
            return;
        }

        var charCount = Encoding.UTF8.GetCharCount(utf8Value);
        if (charCount <= 256)
        {
            Span<char> stackBuffer = stackalloc char[256];
            var written = Encoding.UTF8.GetChars(utf8Value, stackBuffer);
            AppendAligned(stackBuffer[..written], alignment);
            return;
        }

        var rented = ArrayPool<char>.Shared.Rent(charCount);
        try
        {
            var written = Encoding.UTF8.GetChars(utf8Value, rented);
            AppendAligned(rented.AsSpan(0, written), alignment);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rented);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(bool value)
        => AppendLiteral(value ? bool.TrueString : bool.FalseString);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(bool value, int alignment)
        => AppendAligned((value ? bool.TrueString : bool.FalseString).AsSpan(), alignment);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value) where T : unmanaged, ISpanFormattable
        => AppendFormatted(value, alignment: 0, format: null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, int alignment) where T : unmanaged, ISpanFormattable
        => AppendFormatted(value, alignment, format: null);

    public void AppendFormatted<T>(T value, int alignment, string? format) where T : unmanaged, ISpanFormattable
    {
        Span<char> stackBuffer = stackalloc char[128];
        if (value.TryFormat(stackBuffer, out var charsWritten, format, FormatProvider))
        {
            AppendAligned(stackBuffer[..charsWritten], alignment);
            return;
        }

        var rented = ArrayPool<char>.Shared.Rent(512);
        try
        {
            while (true)
            {
                if (value.TryFormat(rented, out charsWritten, format, FormatProvider))
                {
                    AppendAligned(rented.AsSpan(0, charsWritten), alignment);
                    return;
                }

                ArrayPool<char>.Shared.Return(rented);
                rented = ArrayPool<char>.Shared.Rent(rented.Length * 2);
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rented);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T? value) where T : unmanaged, ISpanFormattable
    {
        if (value.HasValue)
        {
            AppendFormatted(value.Value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T? value, int alignment) where T : unmanaged, ISpanFormattable
    {
        if (value.HasValue)
        {
            AppendFormatted(value.Value, alignment);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T? value, int alignment, string? format) where T : unmanaged, ISpanFormattable
    {
        if (value.HasValue)
        {
            AppendFormatted(value.Value, alignment, format);
        }
    }

    private void EnsureTextCapacity(int requiredLength)
    {
        if (requiredLength <= 0)
        {
            requiredLength = DefaultTextCapacity;
        }

        var buffer = _textBuffer;
        if (buffer is null)
        {
            _textBuffer = ArrayPool<char>.Shared.Rent(Math.Max(DefaultTextCapacity, requiredLength));
            return;
        }

        if (buffer.Length >= requiredLength)
        {
            return;
        }

        var newBuffer = ArrayPool<char>.Shared.Rent(Math.Max(buffer.Length * 2, requiredLength));
        buffer.AsSpan(0, _textLength).CopyTo(newBuffer);
        ArrayPool<char>.Shared.Return(buffer);
        _textBuffer = newBuffer;
    }

    private void AppendAligned(ReadOnlySpan<char> value, int alignment)
    {
        if (alignment == 0)
        {
            AppendLiteral(value);
            return;
        }

        var width = Math.Abs(alignment);
        var padding = width - value.Length;
        if (padding <= 0)
        {
            AppendLiteral(value);
            return;
        }

        EnsureTextCapacity(_textLength + width);
        if (alignment > 0)
        {
            AppendSpaces(padding);
            AppendLiteral(value);
        }
        else
        {
            AppendLiteral(value);
            AppendSpaces(padding);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AppendSpaces(int count)
    {
        if (count <= 0)
        {
            return;
        }

        EnsureTextCapacity(_textLength + count);
        _textBuffer!.AsSpan(_textLength, count).Fill(' ');
        _textLength += count;
    }
}
