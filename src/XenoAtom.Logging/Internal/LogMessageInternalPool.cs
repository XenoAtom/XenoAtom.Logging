// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace XenoAtom.Logging;

internal sealed class LogMessageInternalPool
{
    [StructLayout(LayoutKind.Explicit, Size = 128)]
    private struct PaddedCachedMessage
    {
        [FieldOffset(64)]
        public LogMessageInternal? Value;
    }

    private PaddedCachedMessage _cached;
    private LogMessageInternal? _head;
    private readonly int _maxRetainedTextLength;

    public LogMessageInternalPool(int capacity, int maxRetainedTextLength = 4096)
    {
        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        _maxRetainedTextLength = maxRetainedTextLength;

        for (var i = 0; i < capacity; i++)
        {
            Return(new LogMessageInternal());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LogMessageInternal? TryRent()
    {
        var cached = Interlocked.Exchange(ref _cached.Value, null);
        if (cached is not null)
        {
            Volatile.Write(ref cached.PoolState, 0);
            return cached;
        }

        return TryRentFromStack();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private LogMessageInternal? TryRentFromStack()
    {
        while (true)
        {
            var head = Volatile.Read(ref _head);
            if (head is null)
            {
                return null;
            }

            var next = head.PoolNext;
            if (Interlocked.CompareExchange(ref _head, next, head) == head)
            {
                head.PoolNext = null;
                Volatile.Write(ref head.PoolState, 0);
                return head;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Return(LogMessageInternal message)
    {
        if (Interlocked.Exchange(ref message.PoolState, 1) != 0)
        {
            return;
        }

        message.Reset();
        message.TrimRetainedTextBuffer(_maxRetainedTextLength);

        var cached = Interlocked.Exchange(ref _cached.Value, message);
        if (cached is null)
        {
            return;
        }

        ReturnToStack(cached);
    }

    private void ReturnToStack(LogMessageInternal message)
    {
        while (true)
        {
            var head = Volatile.Read(ref _head);
            message.PoolNext = head;
            if (Interlocked.CompareExchange(ref _head, message, head) == head)
            {
                return;
            }
        }
    }
}
