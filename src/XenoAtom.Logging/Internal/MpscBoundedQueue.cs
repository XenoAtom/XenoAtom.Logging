// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace XenoAtom.Logging;

[StructLayout(LayoutKind.Explicit, Size = 128)]
internal struct PaddedLong
{
    [FieldOffset(64)]
    public long Value;
}

internal sealed class MpscBoundedQueue<T> where T : struct
{
    private readonly T[] _buffer;
    private readonly long[] _sequence;
    private readonly int _capacity;
    private readonly bool _powerOfTwo;
    private readonly int _mask;
    private PaddedLong _head;
    private PaddedLong _tail;
    private readonly bool _singleMode;
    private readonly object? _singleLock;
    private int _singleState;
    private T _singleItem;

    public MpscBoundedQueue(int capacity)
    {
        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        _capacity = capacity;
        _singleMode = capacity == 1;
        _singleLock = _singleMode ? new object() : null;
        _buffer = new T[capacity];
        _sequence = new long[capacity];
        for (var i = 0; i < capacity; i++)
        {
            _sequence[i] = i;
        }

        _powerOfTwo = BitOperations.IsPow2((uint)capacity);
        _mask = capacity - 1;
    }

    public int Capacity => _capacity;

    public int Count
    {
        get
        {
            if (_singleMode)
            {
                return Volatile.Read(ref _singleState);
            }

            var count = Volatile.Read(ref _tail.Value) - Volatile.Read(ref _head.Value);
            return (int)Math.Clamp(count, 0L, int.MaxValue);
        }
    }

    public bool IsEmpty
    {
        get
        {
            if (_singleMode)
            {
                return Volatile.Read(ref _singleState) == 0;
            }

            return Volatile.Read(ref _tail.Value) == Volatile.Read(ref _head.Value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetIndex(long sequence)
    {
        if (_powerOfTwo)
        {
            return (int)(sequence & _mask);
        }

        return (int)(sequence % _capacity);
    }

    public bool TryEnqueue(T item)
    {
        if (_singleMode)
        {
            lock (_singleLock!)
            {
                if (_singleState != 0)
                {
                    return false;
                }

                _singleItem = item;
                Volatile.Write(ref _singleState, 1);
                return true;
            }
        }

            var spinWait = new SpinWait();
            while (true)
            {
                var tail = Volatile.Read(ref _tail.Value);
                var index = GetIndex(tail);
                var seq = Volatile.Read(ref _sequence[index]);
                var diff = seq - tail;

                if (diff == 0)
                {
                    if (Interlocked.CompareExchange(ref _tail.Value, tail + 1, tail) == tail)
                    {
                        _buffer[index] = item;
                        Volatile.Write(ref _sequence[index], tail + 1);
                        return true;
                }

                continue;
            }

            if (diff < 0)
            {
                return false;
            }

            spinWait.SpinOnce();
        }
    }

    public bool TryDequeue(out T item)
    {
        if (_singleMode)
        {
            lock (_singleLock!)
            {
                if (_singleState == 0)
                {
                    item = default;
                    return false;
                }

                item = _singleItem;
                Volatile.Write(ref _singleState, 0);
                return true;
            }
        }

        var head = Volatile.Read(ref _head.Value);
        var index = GetIndex(head);
        var seq = Volatile.Read(ref _sequence[index]);
        var diff = seq - (head + 1);

        if (diff == 0)
        {
            item = _buffer[index];
            Volatile.Write(ref _sequence[index], head + _capacity);
            Volatile.Write(ref _head.Value, head + 1);
            return true;
        }

        item = default;
        return false;
    }
}
