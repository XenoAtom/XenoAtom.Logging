// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace XenoAtom.Logging.Tests;

[TestClass]
public class LogManagerDiagnosticsTests
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
    public void GetDiagnostics_WhenUninitialized_ReturnsUninitialized()
    {
        Assert.IsFalse(LogManager.IsInitialized);
        var diagnostics = LogManager.GetDiagnostics();

        Assert.IsFalse(diagnostics.IsInitialized);
        Assert.IsNull(diagnostics.ProcessorType);
        Assert.IsFalse(diagnostics.IsAsyncProcessor);
        Assert.AreEqual(0, diagnostics.AsyncQueueLength);
        Assert.AreEqual(0, diagnostics.AsyncQueueCapacity);
        Assert.AreEqual(0L, diagnostics.DroppedMessages);
        Assert.AreEqual(0L, diagnostics.ErrorCount);
    }

    [TestMethod]
    public void GetDiagnostics_WithSyncProcessor_ReturnsProcessorType()
    {
        var config = CreateConfig(new CountingWriter());
        LogManager.Initialize(config);
        Assert.IsTrue(LogManager.IsInitialized);

        var diagnostics = LogManager.GetDiagnostics();
        Assert.IsTrue(diagnostics.IsInitialized);
        Assert.AreEqual(typeof(LogMessageSyncProcessor), diagnostics.ProcessorType);
        Assert.IsFalse(diagnostics.IsAsyncProcessor);
        Assert.AreEqual(0, diagnostics.AsyncQueueLength);
        Assert.AreEqual(0, diagnostics.AsyncQueueCapacity);
        Assert.AreEqual(0L, diagnostics.DroppedMessages);
        Assert.AreEqual(0L, diagnostics.ErrorCount);
    }

    [TestMethod]
    public void GetDiagnostics_WithAsyncDropMode_ReportsDroppedMessages()
    {
        using var gate = new ManualResetEventSlim(false);
        var writer = new BlockingWriter(gate);
        var config = CreateConfig(writer);
        config.AsyncLogMessageQueueCapacity = 1;
        config.RootLogger.OverflowMode = LoggerOverflowMode.Drop;
        LogManager.InitializeForAsync(config);
        Assert.IsTrue(LogManager.IsInitialized);
        var logger = LogManager.GetLogger("Tests.Diagnostics.Async.Drop");

        try
        {
            // Block the async consumer thread on the first accepted message to force saturation.
            logger.Info("hold");

            for (var index = 0; index < 10_000; index++)
            {
                logger.Info($"drop-{index}");
            }

            WaitUntil(() => LogManager.GetDiagnostics().DroppedMessages > 0, TimeSpan.FromSeconds(2));

            var diagnostics = LogManager.GetDiagnostics();
            Assert.IsTrue(diagnostics.IsInitialized);
            Assert.AreEqual(typeof(LogMessageAsyncProcessor), diagnostics.ProcessorType);
            Assert.IsTrue(diagnostics.IsAsyncProcessor);
            Assert.AreEqual(1, diagnostics.AsyncQueueCapacity);
            Assert.IsTrue(diagnostics.DroppedMessages > 0);
            Assert.AreEqual(0L, diagnostics.ErrorCount);
        }
        finally
        {
            gate.Set();
        }
    }

    [TestMethod]
    public void SyncProcessor_ExceptionLogging_DoesNotRetainUnboundedMemory()
    {
        var writer = new CountingWriter();
        var config = CreateConfig(writer);
        LogManager.Initialize(config);

        var logger = LogManager.GetLogger("Tests.Diagnostics.Sync.MemoryBound");
        var exception = new InvalidOperationException("boom");

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var memoryBefore = GC.GetTotalMemory(forceFullCollection: true);

        for (var index = 0; index < 200_000; index++)
        {
            logger.Error(exception, $"order-{index} failed");
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var memoryAfter = GC.GetTotalMemory(forceFullCollection: true);

        var retainedBytes = memoryAfter - memoryBefore;
        Assert.IsTrue(retainedBytes < 128L * 1024 * 1024, $"Retained memory too high: {retainedBytes} bytes.");
    }

    [TestMethod]
    public void IsInitialized_TracksLifecycle()
    {
        Assert.IsFalse(LogManager.IsInitialized);
        LogManager.Initialize(CreateConfig(new CountingWriter()));
        Assert.IsTrue(LogManager.IsInitialized);
        LogManager.Shutdown();
        Assert.IsFalse(LogManager.IsInitialized);
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

    private sealed class BlockingWriter : LogWriter
    {
        private readonly ManualResetEventSlim _gate;

        public BlockingWriter(ManualResetEventSlim gate)
        {
            _gate = gate;
        }

        public ConcurrentQueue<string> Messages { get; } = new();

        protected override void Log(in LogMessage logMessage)
        {
            Messages.Enqueue(logMessage.Text.ToString());
            _gate.Wait(TimeSpan.FromSeconds(5));
        }
    }

    private sealed class CountingWriter : LogWriter
    {
        public long TotalChars { get; private set; }

        protected override void Log(in LogMessage logMessage)
        {
            TotalChars += logMessage.Text.Length;
        }
    }

    // Buffer pool allocation assertions removed: sync pipeline no longer uses LogBufferPool.
}
