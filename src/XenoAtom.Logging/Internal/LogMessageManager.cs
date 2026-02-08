// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;
using XenoAtom.Logging.Helpers;

namespace XenoAtom.Logging;

/// <summary>
/// A LogBufferManager, per thread, that can be used to allocate a buffer for a log message.
/// </summary>
internal unsafe class LogMessageManager
{
    [ThreadStatic]
    private static LogMessageManager? _current;

    private UnsafeObjectPool<LogMessageWriter> _logMessageWriters;
    private readonly LogMessageProcessor _processor;
    private readonly Stack<BufferState<byte>> _bufferDataStates;
    private BufferState<object> _objectBuffer;
    private BufferState<byte> _dataBuffer;

    public LogMessageManager()
    {
        _logMessageWriters = new UnsafeObjectPool<LogMessageWriter>(4);
        _processor = LogManager.Processor!;
        _bufferDataStates = new();
    }

    public static LogMessageManager Current => _current ??= new LogMessageManager();

    public void Log(LogMessageHandle message)
    {
        _processor.Log(message);
    }

    public LogMessageWriter Allocate()
    {
        var writer = _logMessageWriters.Rent() ?? new LogMessageWriter(this);

        if (!_objectBuffer.IsInitialized)
        {
            _objectBuffer = new BufferState<object>(_processor.BufferPool.RentObjectBuffer(), 0);
            writer.InitializeObjectPointers((void**)_objectBuffer.AlignedFirstElement, (void**)_objectBuffer.AlignedLastElement);
        }
        else
        {
            _objectBuffer.AlignCurrentElementToCacheLine();
        }
        writer.UpdateObjectPointers((void**)_objectBuffer.CurrentElement, (void**)_objectBuffer.AlignedLastElement);

        if (!_dataBuffer.IsInitialized)
        {
            _dataBuffer = new BufferState<byte>(_processor.BufferPool.RentDataBuffer(), _bufferDataStates.Count);
            writer.InitializeDataPointers((byte*)_dataBuffer.AlignedFirstElement, (byte*)_dataBuffer.AlignedLastElement);
        }
        else
        {
            _dataBuffer.AlignCurrentElementToCacheLine();
        }
        writer.UpdateDataPointers((byte*)_dataBuffer.CurrentElement, (byte*)_dataBuffer.AlignedLastElement);

        return writer;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void AllocateNextManaged(LogMessageWriter writer)
    {
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public void AllocateNextUnmanaged(LogMessageWriter writer, LoggerOverflowMode overflowMode, int size)
    {
        // TODO: pass buffer
        writer.EndDataBuffer(null, null);
    }

    internal static LogDataHeader Test()
    {
        return new LogDataHeader(LogDataPartKind.MessagePart, LogDataKind.String, 1);
    }

    public void UpdateNextDataPointer(byte* nextUnalignedMessageData)
    {
        
    }

    private struct BufferState<T>
    {
        public BufferState(T[] buffer, int stackLevel)
        {
            StackLevel = stackLevel;
            Buffer = buffer;
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
            BufferPointer = *(nint*)&buffer;
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
            var pointerToElement0 = (nint)(Unsafe.AsPointer(ref buffer[0]));
            AlignedFirstElement = AlignHelper.AlignUp(pointerToElement0, CpuHelper.CacheLineSize);
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
            AlignedLastElement = AlignHelper.AlignDown(pointerToElement0 + (buffer.Length * sizeof(T)) - CpuHelper.CacheLineSize, CpuHelper.CacheLineSize);
#pragma warning restore CS8500
            CurrentElement = AlignedFirstElement;
        }

        public readonly T[] Buffer;
        public readonly int StackLevel;
        public readonly nint BufferPointer;
        public readonly nint AlignedFirstElement;
        public readonly nint AlignedLastElement;
        public nint CurrentElement;

        public bool IsInitialized => BufferPointer != 0;


        public void AlignCurrentElementToCacheLine()
        {
            CurrentElement = AlignHelper.AlignUp(CurrentElement, CpuHelper.CacheLineSize);
        }
    }
}