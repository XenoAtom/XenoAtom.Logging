// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

/// <summary>
/// Processes log messages asynchronously on a background thread.
/// </summary>
public sealed class LogMessageAsyncProcessor : LogMessageProcessor, ILogMessageProcessorFactory
{
    private static readonly TimeSpan ShutdownJoinTimeout = TimeSpan.FromSeconds(2);
    private readonly MpscBoundedQueue<LogMessageInternalHandle> _queue;
    private readonly LogMessageInternalPool _pool;
    private readonly ManualResetEventSlim _newItemEvent;
    private readonly LogWriter[] _flushWriters;
    private readonly int _queueCapacity;
    private Thread? _backgroundThread;
    private long _sequenceId;
    private bool _initialized;
    private volatile bool _disposed;
    private volatile bool _stopping;
    private long _droppedCount;
    private long _errorCount;

    private LogMessageAsyncProcessor(LogManagerConfig config) : base(config)
    {
        if (config.AsyncLogMessageQueueCapacity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(config.AsyncLogMessageQueueCapacity),
                config.AsyncLogMessageQueueCapacity,
                "AsyncLogMessageQueueCapacity must be greater than zero.");
        }

        _queueCapacity = config.AsyncLogMessageQueueCapacity;
        _queue = new MpscBoundedQueue<LogMessageInternalHandle>(_queueCapacity);
        _pool = new LogMessageInternalPool(_queueCapacity);
        _newItemEvent = new ManualResetEventSlim(false);
        _flushWriters = BuildFlushWriters(config);
    }

    static LogMessageProcessor ILogMessageProcessorFactory.Create(LogManagerConfig config) => new LogMessageAsyncProcessor(config);

    /// <summary>
    /// Gets the number of dropped messages due to overflow policies.
    /// </summary>
    public long DroppedCount => Interlocked.Read(ref _droppedCount);

    /// <summary>
    /// Gets the number of async processing errors observed while dispatching/flushing.
    /// </summary>
    public long ErrorCount => Interlocked.Read(ref _errorCount);

    /// <summary>
    /// Gets the current number of queued messages waiting to be processed.
    /// </summary>
    public int QueueLength
    {
        get
        {
            return _queue.Count;
        }
    }

    /// <summary>
    /// Gets the configured queue capacity.
    /// </summary>
    public int QueueCapacity => _queueCapacity;

    internal override bool Log(LogMessageInternal message, LoggerOverflowMode overflowMode)
    {
        if (_disposed)
        {
            _pool.Return(message);
            return false;
        }

        var handle = new LogMessageInternalHandle(message);
        if (_queue.TryEnqueue(handle))
        {
            SignalNewItem();
            return true;
        }

        if (overflowMode == LoggerOverflowMode.Block || overflowMode == LoggerOverflowMode.Allocate)
        {
            var spinWait = new SpinWait();
            while (!_disposed)
            {
                if (_queue.TryEnqueue(handle))
                {
                    SignalNewItem();
                    return true;
                }

                spinWait.SpinOnce(-1);
            }
        }

        _pool.Return(message);
        Interlocked.Increment(ref _droppedCount);
        return false;
    }

    internal bool TryRentMessage(LoggerOverflowMode overflowMode, out LogMessageInternal? message)
    {
        message = null;
        if (_disposed)
        {
            return false;
        }

        switch (overflowMode)
        {
            case LoggerOverflowMode.Drop:
            case LoggerOverflowMode.DropAndNotify:
                message = _pool.TryRent();
                if (message is not null)
                {
                    return true;
                }

                Interlocked.Increment(ref _droppedCount);
                return false;
            case LoggerOverflowMode.Block:
            case LoggerOverflowMode.Allocate:
                var spinWait = new SpinWait();
                while (true)
                {
                    if (_disposed)
                    {
                        return false;
                    }

                    message = _pool.TryRent();
                    if (message is not null)
                    {
                        return true;
                    }

                    spinWait.SpinOnce(-1);
                }
            default:
                message = _pool.TryRent();
                if (message is not null)
                {
                    return true;
                }

                Interlocked.Increment(ref _droppedCount);
                return false;
        }
    }

    internal void ReturnMessage(LogMessageInternal message)
        => _pool.Return(message);

    /// <inheritdoc />
    public override void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _stopping = true;
        _newItemEvent.Set();

        var thread = _backgroundThread;
        if (thread is not null && thread.IsAlive)
        {
            if (!thread.Join(ShutdownJoinTimeout))
            {
                return;
            }
        }

        _newItemEvent.Dispose();
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;
        _backgroundThread = new Thread(ProcessLoop)
        {
            IsBackground = true,
            Name = "XenoAtom.Logging.AsyncProcessor"
        };
        _backgroundThread.Start();
    }

    private readonly struct LogMessageInternalHandle
    {
        public LogMessageInternalHandle(LogMessageInternal message)
        {
            Message = message;
        }

        public LogMessageInternal Message { get; }
    }

    private void ProcessLoop()
    {
        var spinWait = new SpinWait();
        var flush = false;

        while (!_stopping || !IsQueueEmpty())
        {
            if (TryDequeue(out var message))
            {
                try
                {
                    LogMessageDispatcher.Dispatch(this, message, ref _sequenceId);
                }
                catch (Exception exception)
                {
                    Interlocked.Increment(ref _droppedCount);
                    ReportAsyncError(exception);
                }
                finally
                {
                    _pool.Return(message);
                }

                spinWait.Reset();
                flush = true;
                continue;
            }

            if (spinWait.NextSpinWillYield)
            {
                if (flush)
                {
                    FlushWriters();
                    flush = false;
                    continue;
                }

                // Wait for a producer to signal a new message
                _newItemEvent.Reset();
                if (IsQueueEmpty() && !_stopping)
                {
                    _newItemEvent.Wait();
                }

                spinWait.Reset();
                continue;
            }

            spinWait.SpinOnce(-1);
        }

        // Final flush on shutdown
        if (flush)
        {
            FlushWriters();
        }
    }

    private bool TryDequeue(out LogMessageInternal message)
    {
        if (_queue.TryDequeue(out var handle))
        {
            message = handle.Message;
            return true;
        }

        message = null!;
        return false;
    }

    private bool IsQueueEmpty() => _queue.IsEmpty;

    private static LogWriter[] BuildFlushWriters(LogManagerConfig config)
    {
        var writers = new HashSet<LogWriter>(ReferenceEqualityComparer.Instance);

        foreach (var writerConfig in config.RootLogger.Writers)
        {
            writers.Add(writerConfig.Writer);
        }

        foreach (var loggerConfig in config.Loggers)
        {
            foreach (var writerConfig in loggerConfig.Writers)
            {
                writers.Add(writerConfig.Writer);
            }
        }

        return writers.Count == 0 ? [] : writers.ToArray();
    }

    private void FlushWriters()
    {
        foreach (var writer in _flushWriters)
        {
            try
            {
                writer.Flush();
            }
            catch (Exception exception)
            {
                Interlocked.Increment(ref _droppedCount);
                ReportAsyncError(exception);
            }
        }
    }

    private void SignalNewItem()
    {
        try
        {
            _newItemEvent.Set();
        }
        catch (ObjectDisposedException)
        {
            // Ignore dispose races; shutdown is already in progress.
        }
    }

    private void ReportAsyncError(Exception exception)
    {
        Interlocked.Increment(ref _errorCount);
        try
        {
            Config.AsyncErrorHandler?.Invoke(exception);
        }
        catch
        {
            // Ignore callback failures; logging should keep running.
        }
    }
}
