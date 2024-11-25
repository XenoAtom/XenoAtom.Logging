// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace XenoAtom.Logging;

public sealed class LogMessageAsyncProcessor : LogMessageProcessor, ILogMessageProcessorFactory
{
    private LockFreeQueue _lockFreeQueue;

    private LogMessageAsyncProcessor(LogManagerConfig config) : base(config)
    {
        _lockFreeQueue = new LockFreeQueue();
    }

    static LogMessageProcessor ILogMessageProcessorFactory.Create(LogManagerConfig config) => new LogMessageAsyncProcessor(config);

    internal override void Log(LogMessageHandle message)
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        throw new NotImplementedException();
    }

    public override void Initialize()
    {
        throw new NotImplementedException();
    }

    private unsafe struct LockFreeQueue : IDisposable
    {
        private readonly ManualResetEventSlim _newMessageEvent;
        private LogMessageHandle _head;
        private LogMessageHandle _tail;

        public LockFreeQueue()
        {
            _newMessageEvent = new ManualResetEventSlim(false);
        }

        public void Dispose()
        {
            _newMessageEvent.Dispose();
        }

        public bool TryEnqueue(LogMessageHandle message)
        {
            message.Next = default;
            LogMessageHandle prevHead = new(Interlocked.Exchange(ref Unsafe.As<LogMessageHandle, nint>(ref _head), message.Pointer));

            if (prevHead.IsNull)
            {
                _tail = message;
            }
            else
            {
                prevHead.Next = message;
            }

            _newMessageEvent.Set(); // Signal that a new message is available
            return true;
        }

        public bool TryDequeue(out LogMessageHandle message)
        {
            message = _tail;

            if (message.IsNull)
            {
                _newMessageEvent.Reset(); // No message, reset the event
                return false; // Queue is empty
            }

            LogMessageHandle next = message.Next;
            if (next.IsNull)
            {
                if (Interlocked.CompareExchange(ref Unsafe.As<LogMessageHandle, nint>(ref _head), nint.Zero, message.Pointer) == message.Pointer)
                {
                    _tail = default;
                    return true;
                }

                while ((next = message.Next).IsNull)
                {
                    Thread.Yield();
                }
            }

            _tail = next;
            return true;
        }

        public LogMessageHandle Dequeue(CancellationToken token)
        {
            LogMessageHandle message;
            while (!TryDequeue(out message))
            {
                _newMessageEvent.Wait(token); // Wait until a new message is enqueued
            }
            return message;
        }
    }
}