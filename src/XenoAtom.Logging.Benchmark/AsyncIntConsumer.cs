using System.Numerics;
using System.Runtime.CompilerServices;

namespace XenoAtom.Logging.Benchmark;

internal sealed class AsyncIntConsumer : IDisposable
{
    private readonly MpscBoundedQueue<int> _queue;
    private readonly ManualResetEventSlim _newItemEvent;
    private readonly Thread _thread;
    private volatile bool _stopping;
    private long _value;

    public AsyncIntConsumer(int capacity)
    {
        _queue = new MpscBoundedQueue<int>(Math.Max(capacity, 1));
        _newItemEvent = new ManualResetEventSlim(false);
        _thread = new Thread(ProcessLoop)
        {
            IsBackground = true,
            Name = "Benchmark.AsyncIntConsumer"
        };
        _thread.Start();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Enqueue(int value)
    {
        if (_queue.TryEnqueue(value))
        {
            _newItemEvent.Set();
        }
    }

    public void Dispose()
    {
        _stopping = true;
        _newItemEvent.Set();
        _thread.Join();
        _newItemEvent.Dispose();
        GC.KeepAlive(_value);
    }

    private void ProcessLoop()
    {
        while (true)
        {
            while (_queue.TryDequeue(out var value))
            {
                Interlocked.Add(ref _value, value);
            }

            if (_stopping)
            {
                return;
            }

            _newItemEvent.Reset();
            if (_queue.IsEmpty)
            {
                _newItemEvent.Wait();
            }
        }
    }

    private sealed class MpscBoundedQueue<T> where T : struct
    {
        private readonly T[] _buffer;
        private readonly long[] _sequence;
        private readonly int _capacity;
        private readonly bool _powerOfTwo;
        private readonly int _mask;
        private long _head;
        private long _tail;

        public MpscBoundedQueue(int capacity)
        {
            _capacity = capacity;
            _buffer = new T[capacity];
            _sequence = new long[capacity];
            for (var i = 0; i < capacity; i++)
            {
                _sequence[i] = i;
            }

            _powerOfTwo = BitOperations.IsPow2((uint)capacity);
            _mask = capacity - 1;
        }

        public bool IsEmpty => Volatile.Read(ref _tail) == Volatile.Read(ref _head);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryEnqueue(T item)
        {
            var spinWait = new SpinWait();
            while (true)
            {
                var tail = Volatile.Read(ref _tail);
                var index = GetIndex(tail);
                var seq = Volatile.Read(ref _sequence[index]);
                var diff = seq - tail;

                if (diff == 0)
                {
                    if (Interlocked.CompareExchange(ref _tail, tail + 1, tail) == tail)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryDequeue(out T item)
        {
            var head = Volatile.Read(ref _head);
            var index = GetIndex(head);
            var seq = Volatile.Read(ref _sequence[index]);
            var diff = seq - (head + 1);
            if (diff == 0)
            {
                item = _buffer[index];
                Volatile.Write(ref _sequence[index], head + _capacity);
                Volatile.Write(ref _head, head + 1);
                return true;
            }

            item = default;
            return false;
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
    }
}
