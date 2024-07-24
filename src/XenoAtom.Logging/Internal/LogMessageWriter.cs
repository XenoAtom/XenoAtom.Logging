// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace XenoAtom.Logging.Internal;

/// <summary>
/// A LogBufferManager, per thread, that can be used to allocate a buffer for a log message.
/// </summary>
internal unsafe class LogBufferManager
{
    [ThreadStatic]
    private static LogBufferManager _current;

    private UnsafeObjectPool<LogMessageWriter> _logMessageWriters;

    public LogBufferManager()
    {
        _logMessageWriters = new UnsafeObjectPool<LogMessageWriter>(4);
    }

    public static LogBufferManager Current => _current ??= new LogBufferManager();


    public LogMessageWriter Allocate()
    {
        var writer = _logMessageWriters.Rent() ?? new LogMessageWriter(this);
        // TODO: init writer
        return writer;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void AllocateManaged(LogMessageWriter writer)
    {
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void AllocateUnmanaged(LogMessageWriter writer)
    {
    }

    internal static LogDataHeaderPart Test()
    {
        return new LogDataHeaderPart(LogDataPartKind.MessagePart, LogDataKind.String, 1);
    }
}


// A log message is always aligned to 64 bytes and is composed of log message parts
//
// A log message part
// (4) int:
//  - byte: Part Kind: 0 = Message Part, 1 = End of Message, 2 = End of chunk,
//  - byte: Data Kind 
//  - ushort: Length of the part (number of 8 bytes)
// (8) nint: function pointer formatting (2 bit used for flags)
// (8*) byte[]: aligned on ulong

// If length ==  0, end of message
// If length == -1, end of chunk, the length is followed by a pointer to the beginning of the buffer (to be able to recover it and move it back to the pool), and a pointer to the next chunk
// If length == -2, it is followed by a pointer to a string

internal enum LogDataPartKind : byte
{
    MessagePart,
    BeginMessage,
    EndMessage,
    EndBuffer,
}

internal enum LogDataKind : byte
{
    Unknown,
    String,
    Int8,
    Int16,
    Int32,
    Int64,
    UInt8,
    UInt16,
    UInt32,
    UInt64,
    Float,
    Double,
    Bool,
    Char,
    Guid,
    Decimal,
    DateTime,
    DateTimeOffset,
    TimeSpan,
    DateOnly,
    TimeOnly,
    Version,
    Enum,
}

internal static class LogDataKindExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LogDataKind ToDataKind<T>() where T : unmanaged
    {
        if (typeof(T) == typeof(sbyte)) return LogDataKind.Int8;
        if (typeof(T) == typeof(short)) return LogDataKind.Int16;
        if (typeof(T) == typeof(int)) return LogDataKind.Int32;
        if (typeof(T) == typeof(long)) return LogDataKind.Int64;
        if (typeof(T) == typeof(byte)) return LogDataKind.UInt8;
        if (typeof(T) == typeof(ushort)) return LogDataKind.UInt16;
        if (typeof(T) == typeof(uint)) return LogDataKind.UInt32;
        if (typeof(T) == typeof(ulong)) return LogDataKind.UInt64;
        if (typeof(T) == typeof(float)) return LogDataKind.Float;
        if (typeof(T) == typeof(double)) return LogDataKind.Double;
        if (typeof(T) == typeof(bool)) return LogDataKind.Bool;
        if (typeof(T) == typeof(char)) return LogDataKind.Char;
        if (typeof(T) == typeof(Guid)) return LogDataKind.Guid;
        if (typeof(T) == typeof(decimal)) return LogDataKind.Decimal;
        if (typeof(T) == typeof(DateTime)) return LogDataKind.DateTime;
        if (typeof(T) == typeof(DateTimeOffset)) return LogDataKind.DateTimeOffset;
        if (typeof(T) == typeof(TimeSpan)) return LogDataKind.TimeSpan;
        if (typeof(T) == typeof(DateOnly)) return LogDataKind.DateOnly;
        if (typeof(T) == typeof(TimeOnly)) return LogDataKind.TimeOnly;
        if (typeof(T) == typeof(Version)) return LogDataKind.Version;
        if (typeof(T).IsEnum) return LogDataKind.Enum;
        return LogDataKind.Unknown;
    }
}

internal readonly struct LogDataHeaderPart
{
    private readonly uint _data;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private LogDataHeaderPart(uint data) => _data = data;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LogDataHeaderPart(LogDataPartKind kind, LogDataKind dataKind, ushort length)
        => _data = (uint)(byte)kind << 24 | (uint)dataKind << 16 | length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LogDataHeaderPart Aligned(LogDataPartKind kind, LogDataKind dataKind, ushort length)
        => new((uint)((byte)kind | 0x40) << 24 | (uint)dataKind << 16 | length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LogDataHeaderPart Formatted(LogDataPartKind kind, LogDataKind dataKind, ushort length)
        => new((uint)((byte)kind | 0x80) << 24 | (uint)dataKind << 16 | length);

    public LogDataPartKind Kind => (LogDataPartKind)(_data << 1 >> 24);

    public bool IsAligned => (int)(_data << 1) < 0;

    public bool IsFormatted => (int)_data < 0;

    public LogDataKind DataKind => (LogDataKind)(byte)(_data >> 16);

    public ushort Length => (ushort)_data;
}

internal unsafe class LogMessageWriter
{
    private readonly LogBufferManager _bufferManager;
    private void** _currentObject;
    private void** _endObject;
    private byte* _currentData;
    private byte* _endData;

    const int MinDataSize = 8;

    internal LogMessageWriter(LogBufferManager bufferManager)
    {
        _bufferManager = bufferManager;
    }

    public void Initialize(void** currentObject, void** endObject, byte* currentData, byte* endData)
    {
        _currentObject = currentObject;
        _endObject = endObject;
        _currentData = currentData;
        _endData = endData;
    }

    public void UpdateManaged(void** currentObject, void** endObject)
    {
        _currentObject = currentObject;
        _endObject = endObject;
    }

    public void UpdateUnmanaged(byte* currentData, byte* endData)
    {
        // TODO: handle  message end of chunk
        _currentData = currentData;
        _endData = endData;
    }

    public void BeginMessage(LogLevel level)
    {


    }

    public void BeginMessage(LogLevel level, int literalLength, int formattedCount)
    {

    }

    public void BeginMessage(LogLevel level, LogEventId id, int literalLength, int formattedCount)
    {

    }

    public void EndMessage()
    {
        var endData = _currentData + sizeof(LogDataHeaderPart);
        *(LogDataHeaderPart*)_currentData = new LogDataHeaderPart(LogDataPartKind.EndMessage, LogDataKind.Unknown, 0);
        _currentData = endData;
    }

    internal void AppendLiteral(string s)
    {
        // Write the managed part
        var currentObject = _currentObject;
        if (currentObject >= _endObject) AllocateManaged();
        Unsafe.AsRef<object>(currentObject) = s;
        // Move to next managed slot
        _currentObject = currentObject + 1;

        // Write the unmanaged part
        var size = sizeof(LogDataHeaderPart) + MinDataSize;
        var endData = _currentData + size;
        if (endData > _endData) AllocateUnmanaged();

        var currentData = _currentData;
        *(LogDataHeaderPart*)currentData = new LogDataHeaderPart(LogDataPartKind.MessagePart, LogDataKind.String, 1);
        *(void**)(currentData + sizeof(LogDataHeaderPart)) = currentData;
        _currentData = currentData + size;
    }

    internal void AppendLiteral(ReadOnlySpan<char> s)
    {
        // TODO: Implement
    }

    internal void AppendLiteral(ReadOnlySpan<byte> s)
    {
        // TODO: Implement
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(string s, int alignment)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(ReadOnlySpan<char> s, int alignment)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(ReadOnlySpan<byte> s, int alignment)
    {
    }

    public void AppendFormatted(int value, int alignment, string? format)
    {
        AppendFormatted(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(bool value)
    {
        // Write the unmanaged part
        var size = AlignHelper.AlignUp(sizeof(bool), MinDataSize);
        var length = (ushort)Math.Max(size / MinDataSize, 1);
        size += sizeof(LogDataHeaderPart);

        var endData = _currentData + size;
        if (endData > _endData) AllocateUnmanaged();

        var currentData = _currentData;
        *(LogDataHeaderPart*)currentData = new LogDataHeaderPart(LogDataPartKind.MessagePart, LogDataKind.Bool, length);
        *(bool*)(currentData + sizeof(LogDataHeaderPart)) = value;
        _currentData = currentData + size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(bool value, int alignment)
    {
        // Write the unmanaged part
        var size = AlignHelper.AlignUp(sizeof(bool), MinDataSize);
        var length = (ushort)Math.Max(size / MinDataSize, 1);

        size += sizeof(LogDataHeaderPart) + sizeof(int);
        var endData = _currentData + size;
        if (endData > _endData) AllocateUnmanaged();

        var currentData = _currentData;
        *(LogDataHeaderPart*)currentData = LogDataHeaderPart.Aligned(LogDataPartKind.MessagePart, LogDataKind.Bool, length);
        *(int*)(currentData + sizeof(LogDataHeaderPart)) = alignment;
        *(bool*)(currentData + sizeof(LogDataHeaderPart) + sizeof(int)) = value;
        _currentData = currentData + size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value) where T : unmanaged, ISpanFormattable
    {
        // Write the unmanaged part
        var size = AlignHelper.AlignUp(sizeof(T), MinDataSize);
        var dataKind = LogDataKindExtension.ToDataKind<T>();
        var length = (ushort)Math.Max(size / MinDataSize, 1);

        size += sizeof(LogDataHeaderPart);
        byte* currentData;
        if (dataKind == LogDataKind.Unknown)
        {
            size += sizeof(SpanFormattableFunctionPointer);
            var endData = _currentData + size;
            if (endData > _endData) AllocateUnmanaged();

            currentData = _currentData;
            *(LogDataHeaderPart*)currentData = new LogDataHeaderPart(LogDataPartKind.MessagePart, dataKind, length);
            *(SpanFormattableFunctionPointer*)(currentData + sizeof(LogDataHeaderPart)) = SpanFormattableFunctionPointer.Instance<T>();
            *(T*)(currentData + sizeof(LogDataHeaderPart) + sizeof(SpanFormattableFunctionPointer)) = value;
        }
        else
        {
            var endData = _currentData + size;
            if (endData > _endData) AllocateUnmanaged();

            currentData = _currentData;
            *(LogDataHeaderPart*)currentData = new LogDataHeaderPart(LogDataPartKind.MessagePart, dataKind, length);
            *(T*)(currentData + sizeof(LogDataHeaderPart) + sizeof(SpanFormattableFunctionPointer)) = value;
        }
        _currentData = currentData + size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, int alignment) where T : unmanaged, ISpanFormattable
    {
        // Write the unmanaged part
        var size = AlignHelper.AlignUp(sizeof(T), MinDataSize);
        var dataKind = LogDataKindExtension.ToDataKind<T>();
        var length = (ushort)Math.Max(size / MinDataSize, 1);

        size += sizeof(LogDataHeaderPart);
        byte* currentData;
        if (dataKind == LogDataKind.Unknown)
        {
            size += sizeof(SpanFormattableFunctionPointer) + sizeof(int);
            var endData = _currentData + size;
            if (endData > _endData) AllocateUnmanaged();

            currentData = _currentData;
            *(LogDataHeaderPart*)currentData = LogDataHeaderPart.Aligned(LogDataPartKind.MessagePart, dataKind, length);
            *(SpanFormattableFunctionPointer*)(currentData + sizeof(LogDataHeaderPart)) = SpanFormattableFunctionPointer.Instance<T>();

            *(int*)(currentData + sizeof(LogDataHeaderPart) + sizeof(SpanFormattableFunctionPointer)) = alignment;
            *(T*)(currentData + sizeof(LogDataHeaderPart) + sizeof(SpanFormattableFunctionPointer) + sizeof(int)) = value;
        }
        else
        {
            size += sizeof(int);
            var endData = _currentData + size;
            if (endData > _endData) AllocateUnmanaged();

            currentData = _currentData;
            *(LogDataHeaderPart*)currentData = LogDataHeaderPart.Aligned(LogDataPartKind.MessagePart, dataKind, length);
            *(int*)(currentData + sizeof(LogDataHeaderPart)) = alignment;
            *(T*)(currentData + sizeof(LogDataHeaderPart) + sizeof(int)) = value;
        }
        _currentData = currentData + size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, int alignment, string? format) where T : unmanaged, ISpanFormattable
    {
        // Write the unmanaged part
        var size = AlignHelper.AlignUp(sizeof(T), MinDataSize);
        var dataKind = LogDataKindExtension.ToDataKind<T>();
        var length = (ushort)Math.Max(size / MinDataSize, 1);

        size += sizeof(LogDataHeaderPart);
        byte* currentData;
        if (dataKind == LogDataKind.Unknown)
        {
            size += sizeof(SpanFormattableFunctionPointer) + sizeof(int) + sizeof(nint);
            var endData = _currentData + size;
            if (endData > _endData) AllocateUnmanaged();

            currentData = _currentData;
            *(LogDataHeaderPart*)currentData = LogDataHeaderPart.Formatted(LogDataPartKind.MessagePart, dataKind, length);
            *(SpanFormattableFunctionPointer*)(currentData + sizeof(LogDataHeaderPart)) = SpanFormattableFunctionPointer.Instance<T>();

            *(int*)(currentData + sizeof(LogDataHeaderPart) + sizeof(SpanFormattableFunctionPointer)) = alignment;
            void** currentObject = null;
            if (format != null)
            {
                currentObject = _currentObject;
                if (currentObject >= _endObject) AllocateManaged();
                Unsafe.AsRef<object?>(currentObject) = format;
                _currentObject = currentObject + 1;
            }
            *(void**)(currentData + sizeof(LogDataHeaderPart) + sizeof(SpanFormattableFunctionPointer) + sizeof(int)) = currentObject;
            *(T*)(currentData + sizeof(LogDataHeaderPart) + sizeof(SpanFormattableFunctionPointer) + sizeof(int) + sizeof(nint)) = value;
        }
        else
        {
            size += sizeof(int) + sizeof(nint);
            var endData = _currentData + size;
            if (endData > _endData) AllocateUnmanaged();

            currentData = _currentData;
            *(LogDataHeaderPart*)currentData = LogDataHeaderPart.Formatted(LogDataPartKind.MessagePart, dataKind, length);

            *(int*)(currentData + sizeof(LogDataHeaderPart)) = alignment;
            void** currentObject = null;
            if (format != null)
            {
                currentObject = _currentObject;
                if (currentObject >= _endObject) AllocateManaged();
                Unsafe.AsRef<object?>(currentObject) = format;
                _currentObject = currentObject + 1;
            }
            *(void**)(currentData + sizeof(LogDataHeaderPart) + sizeof(int)) = currentObject;
            *(T*)(currentData + sizeof(LogDataHeaderPart) + sizeof(int) + sizeof(nint)) = value;

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

    private void AllocateManaged() => _bufferManager.AllocateManaged(this);

    private void AllocateUnmanaged() => _bufferManager.AllocateUnmanaged(this);

    public void AppendEventId(LogEventId eventId)
    {
        throw new NotImplementedException();
    }

    public void AppendException(Exception? exception)
    {
        throw new NotImplementedException();
    }
}
