// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Collections.Concurrent;
using System.Diagnostics;

namespace XenoAtom.Logging.Tests;

[TestClass]
public class LogMessageAsyncProcessorTests
{
    [TestInitialize]
    public void Initialize()
    {
        LogManager.Shutdown();
    }

    [TestCleanup]
    public void Cleanup()
    {
        LogManager.Shutdown();
    }

    [TestMethod]
    public void AsyncProcessor_ProcessesMessagesOnBackgroundThread()
    {
        var writer = new ConcurrentCaptureWriter();
        var config = CreateConfig(writer);
        LogManager.Initialize<LogMessageAsyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Async");

        for (var i = 0; i < 20; i++)
        {
            logger.Info($"message-{i}");
        }

        WaitUntil(() => writer.Count >= 20, TimeSpan.FromSeconds(5));
        Assert.AreEqual(20, writer.Messages.Count);
    }

    [TestMethod]
    public void AsyncProcessor_DropMode_DropsWhenQueueIsFull()
    {
        using var gate = new ManualResetEventSlim(false);
        var writer = new BlockingCaptureWriter(gate);
        var config = CreateConfig(writer);
        config.AsyncLogMessageQueueCapacity = 1;
        config.RootLogger.OverflowMode = LoggerOverflowMode.Drop;
        LogManager.Initialize<LogMessageAsyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Async.Drop");

        try
        {
            logger.Info("hold");
            for (var i = 0; i < 10_000; i++)
            {
                logger.Info($"drop-{i}");
            }

            WaitUntil(() => LogManager.GetDiagnostics().DroppedMessages > 0, TimeSpan.FromSeconds(2));
        }
        finally
        {
            gate.Set();
        }

        LogManager.Shutdown();

        Assert.IsTrue(writer.Messages.Count < 10_001);
    }

    [TestMethod]
    public void AsyncProcessor_Shutdown_FlushesQueuedMessages()
    {
        var writer = new ConcurrentCaptureWriter();
        var config = CreateConfig(writer);
        config.AsyncLogMessageQueueCapacity = 1;
        config.RootLogger.OverflowMode = LoggerOverflowMode.Allocate;
        LogManager.Initialize<LogMessageAsyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Async.Shutdown");

        for (var i = 0; i < 50; i++)
        {
            logger.Info($"flush-{i}");
        }

        LogManager.Shutdown();
        Assert.AreEqual(50, writer.Messages.Count);
    }

    [TestMethod]
    public void AsyncProcessor_DropMode_DroppedMessages_DoNotRetainUnboundedMemory()
    {
        using var gate = new ManualResetEventSlim(false);
        var writer = new BlockingCaptureWriter(gate);
        var config = CreateConfig(writer);
        config.AsyncLogMessageQueueCapacity = 1;
        config.RootLogger.OverflowMode = LoggerOverflowMode.Drop;
        LogManager.Initialize<LogMessageAsyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Async.Drop.Memory");
        var exception = new InvalidOperationException("Drop memory test");

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var memoryBefore = GC.GetTotalMemory(forceFullCollection: true);

        for (var i = 0; i < 200_000; i++)
        {
            logger.Error(exception, $"drop-memory-{i}");
        }

        try
        {
            WaitUntil(() => LogManager.GetDiagnostics().DroppedMessages > 0, TimeSpan.FromSeconds(2));
        }
        finally
        {
            gate.Set();
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var memoryAfter = GC.GetTotalMemory(forceFullCollection: true);
        var retainedBytes = memoryAfter - memoryBefore;

        LogManager.Shutdown();
        Assert.IsTrue(retainedBytes < 128L * 1024 * 1024, $"Retained memory is too high after drops: {retainedBytes} bytes.");
    }

    [TestMethod]
    public void AsyncProcessor_DropMode_RespectsMaxDataBufferCount()
    {
        using var gate = new ManualResetEventSlim(false);
        var writer = new BlockingCaptureWriter(gate);
        var config = CreateConfig(writer);
        config.AsyncLogMessageQueueCapacity = 1;
        config.RootLogger.OverflowMode = LoggerOverflowMode.Drop;
        LogManager.Initialize<LogMessageAsyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Async.Drop.BufferLimit");

        for (var i = 0; i < 50_000; i++)
        {
            logger.Info($"limit-{i}");
        }

        try
        {
            WaitUntil(() => LogManager.GetDiagnostics().DroppedMessages > 0, TimeSpan.FromSeconds(2));
        }
        finally
        {
            gate.Set();
        }
        LogManager.Shutdown();
    }

    [TestMethod]
    public void AsyncProcessor_ReinitializeAfterShutdown_ProcessesMessages()
    {
        var firstWriter = new ConcurrentCaptureWriter();
        var firstConfig = CreateConfig(firstWriter);
        LogManager.Initialize<LogMessageAsyncProcessor>(firstConfig);
        var firstLogger = LogManager.GetLogger("Tests.Async.Reinitialize.First");
        firstLogger.Info("first-run");
        WaitUntil(() => firstWriter.Count >= 1, TimeSpan.FromSeconds(2));
        LogManager.Shutdown();

        var secondWriter = new ConcurrentCaptureWriter();
        var secondConfig = CreateConfig(secondWriter);
        LogManager.Initialize<LogMessageAsyncProcessor>(secondConfig);
        var secondLogger = LogManager.GetLogger("Tests.Async.Reinitialize.Second");

        for (var i = 0; i < 10; i++)
        {
            secondLogger.Info($"second-run-{i}");
        }

        WaitUntil(() => secondWriter.Count >= 10, TimeSpan.FromSeconds(5));
        LogManager.Shutdown();
        Assert.AreEqual(10, secondWriter.Messages.Count);
    }

    [TestMethod]
    public void AsyncProcessor_NoRequirementsWriter_ProcessesAllMessagesWithoutDrops()
    {
        var writer = new NoRequirementsCountingWriter();
        var config = CreateConfig(writer);
        config.AsyncLogMessageQueueCapacity = 16_384;
        config.RootLogger.OverflowMode = LoggerOverflowMode.Block;
        LogManager.Initialize<LogMessageAsyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Async.NoRequirements");

        const int messageCount = 50_000;
        for (var index = 0; index < messageCount; index++)
        {
            logger.Error(new InvalidOperationException("boom"), $"order-{index}");
        }

        WaitUntil(() => writer.Count >= messageCount, TimeSpan.FromSeconds(10));
        var diagnostics = LogManager.GetDiagnostics();
        LogManager.Shutdown();

        Assert.AreEqual(messageCount, writer.Count);
        Assert.AreEqual(0, diagnostics.DroppedMessages);
    }

    [TestMethod]
    public void AsyncProcessor_BlockMode_DoesNotAllocateBeyondMaxDataBufferCount()
    {
        var writer = new TextLengthCountingWriter();
        var config = CreateConfig(writer);
        config.AsyncLogMessageQueueCapacity = 256;
        config.RootLogger.OverflowMode = LoggerOverflowMode.Block;
        LogManager.Initialize<LogMessageAsyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Async.Block.BufferLimit");
        var exception = new InvalidOperationException("boom");

        const int messageCount = 200_000;
        for (var index = 0; index < messageCount; index++)
        {
            logger.Error(exception, $"Order {index} failed");
        }

        WaitUntil(() => writer.Count >= messageCount, TimeSpan.FromSeconds(10));
        LogManager.Shutdown();
    }

    [TestMethod]
    public void AsyncProcessor_BlockMode_BlocksProducerUntilQueueHasCapacity()
    {
        using var gate = new ManualResetEventSlim(false);
        var writer = new BlockingCaptureWriter(gate);
        var config = CreateConfig(writer);
        config.AsyncLogMessageQueueCapacity = 1;
        config.RootLogger.OverflowMode = LoggerOverflowMode.Block;
        LogManager.Initialize<LogMessageAsyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Async.BlockingBehavior");

        logger.Info("first");
        WaitUntil(() => writer.Messages.Count >= 1, TimeSpan.FromSeconds(2));
        logger.Info("second");

        var thirdWrite = Task.Run(() => logger.Info("third"));
        Assert.IsFalse(thirdWrite.Wait(TimeSpan.FromMilliseconds(200)), "Expected producer to block while queue is full.");

        gate.Set();
        Assert.IsTrue(thirdWrite.Wait(TimeSpan.FromSeconds(5)), "Expected producer to resume after queue capacity is available.");
        LogManager.Shutdown();
    }

    [TestMethod]
    public void AsyncProcessor_GlobalOverflowMode_IsApplied()
    {
        using var gate = new ManualResetEventSlim(false);
        var writer = new BlockingCaptureWriter(gate);
        var config = CreateConfig(writer);
        config.AsyncLogMessageQueueCapacity = 1;
        config.OverflowMode = LoggerOverflowMode.Block;
        LogManager.Initialize<LogMessageAsyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Async.GlobalOverflow");

        logger.Info("first");
        WaitUntil(() => writer.Messages.Count >= 1, TimeSpan.FromSeconds(2));
        logger.Info("second");

        var thirdWrite = Task.Run(() => logger.Info("third"));
        Assert.IsFalse(thirdWrite.Wait(TimeSpan.FromMilliseconds(200)), "Expected global OverflowMode=Block to block producer.");

        gate.Set();
        Assert.IsTrue(thirdWrite.Wait(TimeSpan.FromSeconds(5)));
        LogManager.Shutdown();
    }

    [TestMethod]
    public void AsyncProcessor_AllocateMode_DoesNotDropMessages()
    {
        var writer = new NoRequirementsCountingWriter();
        var config = CreateConfig(writer);
        config.AsyncLogMessageQueueCapacity = 256;
        config.RootLogger.OverflowMode = LoggerOverflowMode.Allocate;
        LogManager.Initialize<LogMessageAsyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Async.Allocate");

        const int messageCount = 50_000;
        for (var index = 0; index < messageCount; index++)
        {
            logger.Info($"allocate-{index}");
        }

        WaitUntil(() => writer.Count >= messageCount, TimeSpan.FromSeconds(10));
        var diagnostics = LogManager.GetDiagnostics();
        LogManager.Shutdown();

        Assert.AreEqual(messageCount, writer.Count);
        Assert.AreEqual(0, diagnostics.DroppedMessages);
    }

    [TestMethod]
    public void AsyncProcessor_ProcessesMessagesFromMultipleProducers()
    {
        var writer = new NoRequirementsCountingWriter();
        var config = CreateConfig(writer);
        config.AsyncLogMessageQueueCapacity = 8_192;
        config.RootLogger.OverflowMode = LoggerOverflowMode.Block;
        LogManager.Initialize<LogMessageAsyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Async.MultiProducer");

        const int producerCount = 8;
        const int messagesPerProducer = 5_000;
        var tasks = new Task[producerCount];
        for (var producerIndex = 0; producerIndex < producerCount; producerIndex++)
        {
            var producerId = producerIndex;
            tasks[producerIndex] = Task.Run(() =>
            {
                for (var messageIndex = 0; messageIndex < messagesPerProducer; messageIndex++)
                {
                    logger.Info($"producer-{producerId}-message-{messageIndex}");
                }
            });
        }

        Task.WaitAll(tasks);

        var expectedCount = producerCount * messagesPerProducer;
        WaitUntil(() => writer.Count >= expectedCount, TimeSpan.FromSeconds(15));
        var diagnostics = LogManager.GetDiagnostics();
        LogManager.Shutdown();

        Assert.AreEqual(expectedCount, writer.Count);
        Assert.AreEqual(0, diagnostics.DroppedMessages);
    }

    private static LogManagerConfig CreateConfig(LogWriter writer)
    {
        return new LogManagerConfig
        {
            RootLogger =
            {
                MinimumLevel = LogLevel.Trace,
                Writers =
                {
                    writer
                }
            }
        };
    }

    private static void WaitUntil(Func<bool> condition, TimeSpan timeout)
    {
        var start = Stopwatch.GetTimestamp();
        var timeoutTicks = (long)(timeout.TotalSeconds * Stopwatch.Frequency);
        var spinWait = new SpinWait();
        while ((Stopwatch.GetTimestamp() - start) < timeoutTicks)
        {
            if (condition())
            {
                return;
            }

            spinWait.SpinOnce();
        }

        Assert.Fail("Timed out waiting for condition.");
    }

    private sealed class ConcurrentCaptureWriter : LogWriter
    {
        private int _count;

        public ConcurrentQueue<string> Messages { get; } = new();

        public int Count => Volatile.Read(ref _count);

        protected override void Log(in LogMessage logMessage)
        {
            Messages.Enqueue(logMessage.Text.ToString());
            Interlocked.Increment(ref _count);
        }
    }

    private sealed class BlockingCaptureWriter : LogWriter
    {
        private readonly ManualResetEventSlim _gate;
        private int _count;

        public BlockingCaptureWriter(ManualResetEventSlim gate)
        {
            _gate = gate;
        }

        public ConcurrentQueue<string> Messages { get; } = new();

        protected override void Log(in LogMessage logMessage)
        {
            Messages.Enqueue(logMessage.Text.ToString());
            Interlocked.Increment(ref _count);
            _gate.Wait(TimeSpan.FromSeconds(5));
        }
    }

    private sealed class NoRequirementsCountingWriter : LogWriter
    {
        private int _count;

        public int Count => Volatile.Read(ref _count);

        protected override void Log(in LogMessage logMessage)
        {
            Interlocked.Increment(ref _count);
        }
    }

    private sealed class TextLengthCountingWriter : LogWriter
    {
        private int _count;

        public int Count => Volatile.Read(ref _count);

        protected override void Log(in LogMessage logMessage)
        {
            _ = logMessage.Text.Length;
            Interlocked.Increment(ref _count);
        }
    }

    private static void WaitForCount(NoRequirementsCountingWriter writer, int expectedCount, TimeSpan timeout)
    {
        WaitUntil(() => writer.Count >= expectedCount, timeout);
    }

    // Buffer pool allocation assertions removed: async pipeline no longer uses LogBufferPool.
}
