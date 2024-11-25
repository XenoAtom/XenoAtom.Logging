// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Collections.Concurrent;
using XenoAtom.Logging.Helpers;

namespace XenoAtom.Logging;

internal unsafe struct LogBufferPool
{
    private readonly int _objectBufferAllocationSize;
    private readonly int _dataBufferAllocationSize;
    private readonly ConcurrentStack<object[]> _poolObjectBuffers;
    private readonly ConcurrentStack<byte[]> _poolDataBuffers;
    private readonly ConcurrentStack<object> _allocatedBuffers; // Keep buffers alive
    private ManualResetEventSlim _eventDataBufferAvailable;
    private long _totalDataAllocatedInBytes;

    public static readonly int ObjectExtraLengthForCacheLine = (CpuHelper.CacheLineSize * 2) / sizeof(nint);
    public static readonly int DataExtraLengthForCacheLine = CpuHelper.CacheLineSize * 2;

    public LogBufferPool(int objectBufferAllocationSize, int dataBufferAllocationSize)
    {
        // We add 2 cache lines to avoid false sharing and allow to write end message / end buffer without having to allocate a new buffer
        _objectBufferAllocationSize = objectBufferAllocationSize + ObjectExtraLengthForCacheLine;
        _dataBufferAllocationSize = dataBufferAllocationSize + DataExtraLengthForCacheLine;
        _poolObjectBuffers = new ConcurrentStack<object[]>();
        _poolDataBuffers = new ConcurrentStack<byte[]>();
        _allocatedBuffers = new ConcurrentStack<object>();
        _eventDataBufferAvailable = new ManualResetEventSlim();
    }

    public long TotalDataAllocatedInBytes => _totalDataAllocatedInBytes;

    public object[] RentObjectBuffer()
    {
        _poolObjectBuffers.TryPop(out var buffer);
        if (buffer is not null)
        {
            return buffer;
        }

        var objectBufferAllocationSize = _objectBufferAllocationSize * sizeof(nint);
        Interlocked.Add(ref _totalDataAllocatedInBytes, objectBufferAllocationSize);
        buffer = GC.AllocateArray<object>(_objectBufferAllocationSize, true);
        _allocatedBuffers.Push(buffer);
        return buffer;
    }

    public void ReturnObjectBuffer(object[] buffer)
    {
        _poolObjectBuffers.Push(buffer);
    }

    public byte[]? TryRentDataBuffer()
    {
        _poolDataBuffers.TryPop(out var buffer);
        return buffer;
    }

    public void WaitDataBufferAvailable()
    {
        _eventDataBufferAvailable.Wait();
    }

    public byte[] RentDataBuffer()
    {
        var buffer = TryRentDataBuffer() ?? AllocateDataBuffer();
        return buffer;
    }

    public byte[] AllocateDataBuffer()
    {
        var dataBufferAllocationSize = _dataBufferAllocationSize;
        Interlocked.Add(ref _totalDataAllocatedInBytes, dataBufferAllocationSize);
        var buffer = GC.AllocateArray<byte>(dataBufferAllocationSize, true);
        _allocatedBuffers.Push(buffer);
        return buffer;
    }

    public void ReturnDataBuffer(byte[] buffer)
    {
        _poolDataBuffers.Push(buffer);
        _eventDataBufferAvailable.Set();
    }
}