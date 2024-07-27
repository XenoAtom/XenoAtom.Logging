// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace XenoAtom.Logging;

/// <summary>
/// A LogBufferManager, per thread, that can be used to allocate a buffer for a log message.
/// </summary>
internal unsafe class LogBufferManager
{
    [ThreadStatic]
    private static LogBufferManager? _current;

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
    public void AllocateNextManaged(LogMessageWriter writer)
    {
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void AllocateNextUnmanaged(LogMessageWriter writer, int size)
    {


        // TODO: pass buffer
        writer.EndDataBuffer(null, null);
    }

    internal static LogDataHeader Test()
    {
        return new LogDataHeader(LogDataPartKind.MessagePart, LogDataKind.String, 1);
    }
}