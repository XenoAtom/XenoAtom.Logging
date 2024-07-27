// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace XenoAtom.Logging;

internal unsafe class LogMessageWriter
{
    private readonly LogBufferManager _bufferManager;
    private void** _beginObject;
    private void** _currentObject;
    private void** _endObject;
    private byte* _beginData;
    private byte* _currentData;
    private byte* _endData;

    internal LogMessageWriter(LogBufferManager bufferManager)
    {
        _bufferManager = bufferManager;
    }

    public void Initialize(void** currentObject, void** endObject, byte* currentData, byte* endData)
    {
        _beginObject = currentObject;
        _currentObject = currentObject;
        _endObject = endObject;
        _beginData = currentData;
        _currentData = currentData;
        _endData = endData;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateManaged(void** beginObject, void** endObject)
    {
        _currentObject = beginObject;
        _endObject = endObject;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateUnmanaged(byte* beginData, byte* endData)
    {
        _currentData = beginData;
        _endData = endData;
    }

    public void BeginMessage(LogLevel level)
    {
        var size = sizeof(LogDataHeader) + sizeof(int);
        var endData = _currentData + size;
        if (endData > _endData) AllocateUnmanaged();

        var currentData = _currentData;
        *(LogDataHeader*)currentData = new LogDataHeader(LogDataPartKind.BeginLiteralMessage, LogDataKind.Unknown, 0);
        *(LogLevel*)(currentData + sizeof(LogDataHeader)) = level;
        _currentData = currentData + size;
    }

    public void BeginMessage(Logger logger, LogLevel level)
    {
        // LogDataPartKind.BeginMessage
        // - Header
        // - LogLevel
        // - nint Logger
        // - nint Thread
        // - DateTimeOffset Timestamp
        var size = sizeof(LogDataHeader) + sizeof(LogLevel) + sizeof(nint) + sizeof(nint) + sizeof(DateTimeOffset);
        var endData = _currentData + size;
        if (endData > _endData) AllocateUnmanaged();

        var currentData = _currentData;
        *(LogDataHeader*)currentData = new LogDataHeader(LogDataPartKind.BeginMessage, LogDataKind.Unknown, 0);
        *(LogLevel*)(currentData + sizeof(LogDataHeader)) = level;
        *(nint*)(currentData + sizeof(LogDataHeader) + sizeof(LogLevel)) = WriteObject(logger);
        *(nint*)(currentData + sizeof(LogDataHeader) + sizeof(LogLevel) + sizeof(nint)) = WriteObject(Thread.CurrentThread);
        *(DateTimeOffset*)(currentData + sizeof(LogDataHeader) + sizeof(LogLevel) + sizeof(nint) + sizeof(nint)) = LogManager.TimeProvider.GetUtcNow();

        _currentData = currentData + size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EndMessage()
    {
        var currentData = _currentData;
        *(LogDataHeader*)currentData = new LogDataHeader(LogDataPartKind.EndMessage, LogDataKind.Unknown, 0);
        currentData += sizeof(LogDataHeader);
        _currentData = currentData;

        // TODO: Flush the message
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EndDataBuffer(byte* beginDataBuffer, byte* endDataBuffer)
    {
        var currentData = _currentData;
        *(LogDataHeader*)currentData = new LogDataHeader(LogDataPartKind.EndDataBuffer, LogDataKind.Unknown, 0);
        *(byte**)(currentData + sizeof(LogDataHeader)) = _beginData; // In order to recover the original buffer to return to the pool
        *(byte**)(currentData + sizeof(LogDataHeader) + sizeof(nint)) = beginDataBuffer; // In order to switch to the next buffer

        // Update the next data buffer
        UpdateUnmanaged(beginDataBuffer, endDataBuffer);
    }

    public void AppendEventId(LogEventId eventId)
    {
        // Write the unmanaged part
        var size = sizeof(LogDataHeader) + sizeof(int) + sizeof(nint);
        var endData = _currentData + size;
        if (endData > _endData) AllocateUnmanaged();

        var currentData = _currentData;
        *(LogDataHeader*)currentData = new LogDataHeader(LogDataPartKind.EventId, LogDataKind.Unknown, 0);
        *(int*)(currentData + sizeof(LogDataHeader)) = eventId.Id;
        *(nint*)(currentData + sizeof(LogDataHeader) + sizeof(int)) = WriteObject(eventId.Name);
        _currentData = currentData + size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private nint WriteObject(object? obj)
    {
        var currentObject = _currentObject;
        if (currentObject >= _endObject) AllocateManaged();
        Unsafe.AsRef<object?>(currentObject) = obj;
        _currentObject = currentObject + 1;
        return (nint)currentObject;
    }

    public void AppendException(Exception? exception)
    {
        // Write the unmanaged part
        var size = sizeof(LogDataHeader) + sizeof(nint);
        var endData = _currentData + size;
        if (endData > _endData) AllocateUnmanaged();

        var currentData = _currentData;
        *(LogDataHeader*)currentData = new LogDataHeader(LogDataPartKind.Exception, LogDataKind.Unknown, 0);
        *(nint*)(currentData + sizeof(LogDataHeader)) = WriteObject(exception);
        _currentData = currentData + size;
    }

    public void AppendLiteral(string s)
    {
        // Write the unmanaged part
        var size = sizeof(LogDataHeader) + sizeof(nint);
        var endData = _currentData + size;
        if (endData > _endData) AllocateUnmanaged();

        var currentData = _currentData;
        *(LogDataHeader*)currentData = new LogDataHeader(LogDataPartKind.MessagePart, LogDataKind.String, 0);
        *(nint*)(currentData + sizeof(LogDataHeader)) = WriteObject(s);
        _currentData = currentData + size;
    }

    public void AppendLiteral(ReadOnlySpan<char> s)
    {
        var size = sizeof(LogDataHeader) + sizeof(int) + s.Length * 2;
        size = AlignHelper.AlignUp(size, sizeof(nint));
        var endData = _currentData + size;
        if (endData > _endData) AllocateUnmanaged(size); // We are passing the size because we don't know if we have enough space to write the data

        var currentData = _currentData;
        *(LogDataHeader*)currentData = new LogDataHeader(LogDataPartKind.MessagePart, LogDataKind.SpanChar, 0);
        *(int*)(currentData + sizeof(LogDataHeader)) = s.Length;
        s.CopyTo(new Span<char>(currentData + sizeof(LogDataHeader) + sizeof(int), s.Length));
    }

    public void AppendLiteral(ReadOnlySpan<byte> s)
    {
        var size = sizeof(LogDataHeader) + sizeof(int) + s.Length;
        size = AlignHelper.AlignUp(size, sizeof(nint));
        var endData = _currentData + size;
        if (endData > _endData) AllocateUnmanaged(size); // We are passing the size because we don't know if we have enough space to write the data

        var currentData = _currentData;
        *(LogDataHeader*)currentData = new LogDataHeader(LogDataPartKind.MessagePart, LogDataKind.SpanByte, 0);
        *(int*)(currentData + sizeof(LogDataHeader)) = s.Length;
        s.CopyTo(new Span<byte>(currentData + sizeof(LogDataHeader) + sizeof(int), s.Length));
    }

    public void AppendFormatted(string s, int alignment)
    {
        // Write the unmanaged part
        var size = sizeof(LogDataHeader) + sizeof(int) + sizeof(nint);
        var endData = _currentData + size;
        if (endData > _endData) AllocateUnmanaged();

        var currentData = _currentData;
        *(LogDataHeader*)currentData = LogDataHeader.Aligned(LogDataPartKind.MessagePart, LogDataKind.String, 0);
        *(int*)(currentData + sizeof(LogDataHeader)) = alignment;
        *(nint*)(currentData + sizeof(LogDataHeader) + sizeof(int)) = WriteObject(s);
        _currentData = currentData + size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(ReadOnlySpan<char> s, int alignment)
    {
        var size = sizeof(LogDataHeader) + sizeof(int) + sizeof(int) + s.Length * 2;
        size = AlignHelper.AlignUp(size, sizeof(nint));
        var endData = _currentData + size;
        if (endData > _endData) AllocateUnmanaged(size); // We are passing the size because we don't know if we have enough space to write the data

        var currentData = _currentData;
        *(LogDataHeader*)currentData = LogDataHeader.Aligned(LogDataPartKind.MessagePart, LogDataKind.SpanChar, 0);
        *(int*)(currentData + sizeof(LogDataHeader)) = alignment;
        *(int*)(currentData + sizeof(LogDataHeader) + sizeof(int)) = s.Length;
        s.CopyTo(new Span<char>(currentData + sizeof(LogDataHeader) + sizeof(int) + sizeof(int), s.Length));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(ReadOnlySpan<byte> s, int alignment)
    {
        var size = sizeof(LogDataHeader) + sizeof(int) + sizeof(int) + s.Length;
        size = AlignHelper.AlignUp(size, sizeof(nint));
        var endData = _currentData + size;
        if (endData > _endData) AllocateUnmanaged(size); // We are passing the size because we don't know if we have enough space to write the data

        var currentData = _currentData;
        *(LogDataHeader*)currentData = LogDataHeader.Aligned(LogDataPartKind.MessagePart, LogDataKind.SpanByte, 0);
        *(int*)(currentData + sizeof(LogDataHeader)) = alignment;
        *(int*)(currentData + sizeof(LogDataHeader) + sizeof(int)) = s.Length;
        s.CopyTo(new Span<byte>(currentData + sizeof(LogDataHeader) + sizeof(int) + sizeof(int), s.Length));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(bool value)
    {
        // Write the unmanaged part
        const int length = 1;
        var size = sizeof(LogDataHeader) + sizeof(nint);

        var endData = _currentData + size;
        if (endData > _endData) AllocateUnmanaged();

        var currentData = _currentData;
        *(LogDataHeader*)currentData = new LogDataHeader(LogDataPartKind.MessagePart, LogDataKind.Bool, length);
        *(bool*)(currentData + sizeof(LogDataHeader)) = value;
        _currentData = currentData + size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(bool value, int alignment)
    {
        // Write the unmanaged part
        var size = AlignHelper.AlignUp(sizeof(bool), sizeof(nint));
        var length = (ushort)Math.Max(size / sizeof(nint), 1);

        size += sizeof(LogDataHeader) + sizeof(int);
        var endData = _currentData + size;
        if (endData > _endData) AllocateUnmanaged();

        var currentData = _currentData;
        *(LogDataHeader*)currentData = LogDataHeader.Aligned(LogDataPartKind.MessagePart, LogDataKind.Bool, length);
        *(int*)(currentData + sizeof(LogDataHeader)) = alignment;
        *(bool*)(currentData + sizeof(LogDataHeader) + sizeof(int)) = value;
        _currentData = currentData + size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value) where T : unmanaged, ISpanFormattable
    {
        // Write the unmanaged part
        var size = AlignHelper.AlignUp(sizeof(T), sizeof(nint));
        var dataKind = LogDataKindExtension.ToDataKind<T>();
        var length = (ushort)Math.Max(size / sizeof(nint), 1);

        size += sizeof(LogDataHeader);
        byte* currentData;
        if (dataKind == LogDataKind.Unknown)
        {
            size += sizeof(SpanFormattableFunctionPointer);
            var endData = _currentData + size;
            if (endData > _endData) AllocateUnmanaged();

            currentData = _currentData;
            *(LogDataHeader*)currentData = new LogDataHeader(LogDataPartKind.MessagePart, dataKind, length);
            *(SpanFormattableFunctionPointer*)(currentData + sizeof(LogDataHeader)) = SpanFormattableFunctionPointer.Instance<T>();
            *(T*)(currentData + sizeof(LogDataHeader) + sizeof(SpanFormattableFunctionPointer)) = value;
        }
        else
        {
            var endData = _currentData + size;
            if (endData > _endData) AllocateUnmanaged();

            currentData = _currentData;
            *(LogDataHeader*)currentData = new LogDataHeader(LogDataPartKind.MessagePart, dataKind, length);
            *(T*)(currentData + sizeof(LogDataHeader) + sizeof(SpanFormattableFunctionPointer)) = value;
        }
        _currentData = currentData + size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, int alignment) where T : unmanaged, ISpanFormattable
    {
        // Write the unmanaged part
        var size = AlignHelper.AlignUp(sizeof(T), sizeof(nint));
        var dataKind = LogDataKindExtension.ToDataKind<T>();
        var length = (ushort)Math.Max(size / sizeof(nint), 1);

        size += sizeof(LogDataHeader);
        byte* currentData;
        if (dataKind == LogDataKind.Unknown)
        {
            size += sizeof(SpanFormattableFunctionPointer) + sizeof(int);
            var endData = _currentData + size;
            if (endData > _endData) AllocateUnmanaged();

            currentData = _currentData;
            *(LogDataHeader*)currentData = LogDataHeader.Aligned(LogDataPartKind.MessagePart, dataKind, length);
            *(SpanFormattableFunctionPointer*)(currentData + sizeof(LogDataHeader)) = SpanFormattableFunctionPointer.Instance<T>();

            *(int*)(currentData + sizeof(LogDataHeader) + sizeof(SpanFormattableFunctionPointer)) = alignment;
            *(T*)(currentData + sizeof(LogDataHeader) + sizeof(SpanFormattableFunctionPointer) + sizeof(int)) = value;
        }
        else
        {
            size += sizeof(int);
            var endData = _currentData + size;
            if (endData > _endData) AllocateUnmanaged();

            currentData = _currentData;
            *(LogDataHeader*)currentData = LogDataHeader.Aligned(LogDataPartKind.MessagePart, dataKind, length);
            *(int*)(currentData + sizeof(LogDataHeader)) = alignment;
            *(T*)(currentData + sizeof(LogDataHeader) + sizeof(int)) = value;
        }
        _currentData = currentData + size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, int alignment, string? format) where T : unmanaged, ISpanFormattable
    {
        // Write the unmanaged part
        var size = AlignHelper.AlignUp(sizeof(T), sizeof(nint));
        var dataKind = LogDataKindExtension.ToDataKind<T>();
        var length = (ushort)Math.Max(size / sizeof(nint), 1);

        size += sizeof(LogDataHeader);
        byte* currentData;
        if (dataKind == LogDataKind.Unknown)
        {
            size += sizeof(SpanFormattableFunctionPointer) + sizeof(int) + sizeof(nint);
            var endData = _currentData + size;
            if (endData > _endData) AllocateUnmanaged();

            currentData = _currentData;
            *(LogDataHeader*)currentData = LogDataHeader.Formatted(LogDataPartKind.MessagePart, dataKind, length);
            *(SpanFormattableFunctionPointer*)(currentData + sizeof(LogDataHeader)) = SpanFormattableFunctionPointer.Instance<T>();

            *(int*)(currentData + sizeof(LogDataHeader) + sizeof(SpanFormattableFunctionPointer)) = alignment;
            void** currentObject = null;
            if (format != null)
            {
                currentObject = _currentObject;
                if (currentObject >= _endObject) AllocateManaged();
                Unsafe.AsRef<object?>(currentObject) = format;
                _currentObject = currentObject + 1;
            }
            *(void**)(currentData + sizeof(LogDataHeader) + sizeof(SpanFormattableFunctionPointer) + sizeof(int)) = currentObject;
            *(T*)(currentData + sizeof(LogDataHeader) + sizeof(SpanFormattableFunctionPointer) + sizeof(int) + sizeof(nint)) = value;
        }
        else
        {
            size += sizeof(int) + sizeof(nint);
            var endData = _currentData + size;
            if (endData > _endData) AllocateUnmanaged();

            currentData = _currentData;
            *(LogDataHeader*)currentData = LogDataHeader.Formatted(LogDataPartKind.MessagePart, dataKind, length);

            *(int*)(currentData + sizeof(LogDataHeader)) = alignment;
            void** currentObject = null;
            if (format != null)
            {
                currentObject = _currentObject;
                if (currentObject >= _endObject) AllocateManaged();
                Unsafe.AsRef<object?>(currentObject) = format;
                _currentObject = currentObject + 1;
            }
            *(void**)(currentData + sizeof(LogDataHeader) + sizeof(int)) = currentObject;
            *(T*)(currentData + sizeof(LogDataHeader) + sizeof(int) + sizeof(nint)) = value;

        }
        _currentData = currentData + size;
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

    private void AllocateManaged() => _bufferManager.AllocateNextManaged(this);

    private void AllocateUnmanaged(int size = 0) => _bufferManager.AllocateNextUnmanaged(this, size);

    public void AppendProperties(LogProperties properties)
    {
        
    }
}
