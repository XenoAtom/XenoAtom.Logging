// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Collections.Concurrent;

namespace XenoAtom.Logging.Tests;

[TestClass]
public class LogManagerTests
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
    public void SyncProcessor_WritesLiteralMessage()
    {
        var writer = new BufferLogWriter();
        var config = CreateConfig(writer, LogLevel.Trace);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Sync");

        logger.Info("hello world");

        Assert.AreEqual(1, writer.Messages.Count);
        Assert.AreEqual(LogLevel.Info, writer.Messages[0].Level);
        Assert.AreEqual("Tests.Sync", writer.Messages[0].LoggerName);
        Assert.AreEqual("hello world", writer.Messages[0].Text);
    }

    [TestMethod]
    public void DefaultProcessor_WritesMessage()
    {
        var writer = new BufferLogWriter();
        var config = CreateConfig(writer, LogLevel.Trace);

        LogManager.Initialize(config);
        var logger = LogManager.GetLogger("Tests.Default");

        logger.Info("default processor");

        Assert.AreEqual(1, writer.Messages.Count);
        Assert.AreEqual("default processor", writer.Messages[0].Text);
    }

    [TestMethod]
    public void LoggerSpecificLevel_IsApplied()
    {
        var writer = new BufferLogWriter();
        var config = CreateConfig(writer, LogLevel.Trace);
        config.Loggers.Add("Tests.Filtered", LogLevel.Error);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Filtered");

        logger.Info("ignored");
        logger.Error("kept");

        Assert.AreEqual(1, writer.Messages.Count);
        Assert.AreEqual(LogLevel.Error, writer.Messages[0].Level);
        Assert.AreEqual("kept", writer.Messages[0].Text);
    }

    [TestMethod]
    public void InterpolatedValues_AreFormattedAndAligned()
    {
        var writer = new BufferLogWriter();
        var config = CreateConfig(writer, LogLevel.Trace);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Interpolated");

        logger.Info($"Value:{42} Flag:{true,6} Name:{"abc"}");

        Assert.AreEqual(1, writer.Messages.Count);
        Assert.AreEqual("Value:42 Flag:  True Name:abc", writer.Messages[0].Text);
    }

    [TestMethod]
    public void InterpolatedValues_WithFormatString_AreFormatted()
    {
        var writer = new BufferLogWriter();
        var config = CreateConfig(writer, LogLevel.Trace);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.InterpolatedFormat");

        logger.Info($"Price:{12.345m,0:0.00}");

        Assert.AreEqual(1, writer.Messages.Count);
        Assert.AreEqual("Price:12.35", writer.Messages[0].Text);
    }

    [TestMethod]
    public void InterpolatedValues_AreSupportedAcrossAllLevels()
    {
        var writer = new BufferLogWriter();
        var config = CreateConfig(writer, LogLevel.Trace);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Interpolated.AllLevels");

        logger.Trace($"trace:{1}");
        logger.Debug($"debug:{2}");
        logger.Info($"info:{3}");
        logger.Warn($"warn:{4}");
        logger.Error($"error:{5}");
        logger.Fatal($"fatal:{6}");

        Assert.AreEqual(6, writer.Messages.Count);
        Assert.AreEqual(LogLevel.Trace, writer.Messages[0].Level);
        Assert.AreEqual("trace:1", writer.Messages[0].Text);
        Assert.AreEqual(LogLevel.Debug, writer.Messages[1].Level);
        Assert.AreEqual("debug:2", writer.Messages[1].Text);
        Assert.AreEqual(LogLevel.Info, writer.Messages[2].Level);
        Assert.AreEqual("info:3", writer.Messages[2].Text);
        Assert.AreEqual(LogLevel.Warn, writer.Messages[3].Level);
        Assert.AreEqual("warn:4", writer.Messages[3].Text);
        Assert.AreEqual(LogLevel.Error, writer.Messages[4].Level);
        Assert.AreEqual("error:5", writer.Messages[4].Text);
        Assert.AreEqual(LogLevel.Fatal, writer.Messages[5].Level);
        Assert.AreEqual("fatal:6", writer.Messages[5].Text);
    }

    [TestMethod]
    public void InterpolatedReadOnlySpan_WithProperties_IsDecoded()
    {
        var writer = new BufferLogWriter();
        var config = CreateConfig(writer, LogLevel.Trace);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.InterpolatedSpan");
        var text = "span payload".AsSpan();
        var properties = new LogProperties { ("RequestId", 1) };

        logger.Info(properties, $"{text}");

        Assert.AreEqual(1, writer.Messages.Count);
        Assert.AreEqual("span payload", writer.Messages[0].Text);
    }

    [TestMethod]
    public void EventId_IsDecoded()
    {
        var writer = new BufferLogWriter();
        var config = CreateConfig(writer, LogLevel.Trace);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Event");

        logger.Info(new LogEventId(42, "TestEvent"), "message");

        Assert.AreEqual(1, writer.Messages.Count);
        Assert.AreEqual(42, writer.Messages[0].EventId.Id);
        Assert.AreEqual("TestEvent", writer.Messages[0].EventId.Name);
    }

    [TestMethod]
    public void Exception_IsDecoded()
    {
        var writer = new BufferLogWriter();
        var config = CreateConfig(writer, LogLevel.Trace);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Exception");
        var exception = new InvalidOperationException("boom");

        logger.Error(exception, "faulted");

        Assert.AreEqual(1, writer.Messages.Count);
        Assert.AreEqual(exception, writer.Messages[0].Exception);
    }

    [TestMethod]
    public void ErrorInterpolatedException_DisabledLogger_DoesNotAllocatePerCall()
    {
        var writer = new BufferLogWriter();
        var config = CreateConfig(writer, LogLevel.Fatal);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Exception.DisabledAlloc");
        var exception = new InvalidOperationException("boom");

        for (var i = 0; i < 1_000; i++)
        {
            logger.Error(exception, $"Order {42} failed");
        }

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 10_000; i++)
        {
            logger.Error(exception, $"Order {42} failed");
        }

        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.IsTrue(allocated < 1024, $"Expected near-zero allocations for disabled logger path, but allocated {allocated} bytes.");
    }

    [TestMethod]
    public void TimestampAndThread_AreSet()
    {
        var writer = new BufferLogWriter();
        var config = CreateConfig(writer, LogLevel.Trace);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Metadata");
        var before = DateTime.UtcNow.AddSeconds(-1);
        var threadId = Environment.CurrentManagedThreadId;

        logger.Info("metadata");

        Assert.AreEqual(1, writer.Messages.Count);
        Assert.AreEqual(threadId, writer.Messages[0].ManagedThreadId);
        Assert.IsTrue(writer.Messages[0].Timestamp >= before);
        Assert.IsTrue(writer.Messages[0].Timestamp <= DateTime.UtcNow.AddSeconds(1));
    }

    [TestMethod]
    public void WriterRejectFilter_IsApplied()
    {
        var writer = new BufferLogWriter();
        writer.RejectFilters.Add(static (in LogMessage message) => message.Text.IndexOf("skip".AsSpan(), StringComparison.Ordinal) >= 0);
        var config = CreateConfig(writer, LogLevel.Trace);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Reject");

        logger.Info("skip this");
        logger.Info("keep this");

        Assert.AreEqual(1, writer.Messages.Count);
        Assert.AreEqual("keep this", writer.Messages[0].Text);
    }

    [TestMethod]
    public void WriterWithoutFormattedMessageRequirement_StillReceivesText()
    {
        var writer = new NoTextLogWriter();
        var config = CreateConfig(writer, LogLevel.Trace);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.NoText");

        logger.Info("hello");

        Assert.AreEqual(1, writer.CallCount);
        Assert.AreEqual("hello".Length, writer.TextLength);
    }

    [TestMethod]
    public void WriterWithoutOptionalMetadataRequirements_StillReceivesMetadata()
    {
        var writer = new MinimalMetadataLogWriter();
        var config = CreateConfig(writer, LogLevel.Trace);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.MinimalMetadata");
        var exception = new InvalidOperationException("boom");
        var properties = new LogProperties { ("RequestId", 42) };
        using var scopeProperties = new LogProperties { ("ScopeId", 7) };
        using (logger.BeginScope(scopeProperties))
        {
            logger.Error(new LogEventId(12, "Evt"), exception, "payload", properties);
        }

        properties.Dispose();

        Assert.AreEqual(1, writer.CallCount);
        Assert.AreEqual("payload".Length, writer.TextLength);
        Assert.AreEqual(12, writer.EventId.Id);
        Assert.AreEqual("Evt", writer.EventId.Name);
        Assert.AreSame(exception, writer.Exception);
        Assert.AreEqual(1, writer.PropertiesCount);
        Assert.AreEqual(1, writer.ScopeCount);
    }

    [TestMethod]
    public void Shutdown_CanBeCalledMultipleTimes()
    {
        var writer = new BufferLogWriter();
        var config = CreateConfig(writer, LogLevel.Trace);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        LogManager.Shutdown();
        LogManager.Shutdown();
    }

    [TestMethod]
    public void LoggingAfterShutdown_ThrowsAndDoesNotWrite()
    {
        var writer = new BufferLogWriter();
        var config = CreateConfig(writer, LogLevel.Trace);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.AfterShutdown");
        logger.Info("before");
        LogManager.Shutdown();

        Assert.Throws<InvalidOperationException>(() => logger.Info("after"));

        Assert.AreEqual(1, writer.Messages.Count);
        Assert.AreEqual("before", writer.Messages[0].Text);
    }

    [TestMethod]
    public void GetLogger_WithWhitespaceName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => LogManager.GetLogger("   "));
    }

    [TestMethod]
    public void Initialize_WithInvalidAsyncQueueCapacity_Throws()
    {
        var writer = new BufferLogWriter();
        var config = CreateConfig(writer, LogLevel.Trace);
        config.AsyncLogMessageQueueCapacity = 0;

        Assert.Throws<ArgumentOutOfRangeException>(() => LogManager.Initialize<LogMessageAsyncProcessor>(config));
    }

    [TestMethod]
    public void LoggerConfigCollection_AddNullWriter_Throws()
    {
        var config = new LogManagerConfig();
        Assert.Throws<ArgumentNullException>(() => config.RootLogger.Writers.Add((LogWriter)null!));
    }

    [TestMethod]
    public void Initialize_IsAtomicAcrossThreads()
    {
        var errorCount = 0;
        var successCount = 0;
        var start = new Barrier(participantCount: 8);
        var tasks = new Task[8];

        for (var index = 0; index < tasks.Length; index++)
        {
            tasks[index] = Task.Run(() =>
            {
                var writer = new BufferLogWriter();
                var config = CreateConfig(writer, LogLevel.Trace);
                start.SignalAndWait();
                try
                {
                    LogManager.Initialize<LogMessageSyncProcessor>(config);
                    Interlocked.Increment(ref successCount);
                }
                catch (InvalidOperationException)
                {
                    Interlocked.Increment(ref errorCount);
                }
            });
        }

        Task.WaitAll(tasks);
        LogManager.Shutdown();

        Assert.AreEqual(1, successCount, $"Expected a single successful initialize but got {successCount}.");
        Assert.AreEqual(tasks.Length - 1, errorCount, $"Expected {tasks.Length - 1} initialization failures but got {errorCount}.");
    }

    [TestMethod]
    public void LogManagerConfig_WhitespaceLoggerName_ThrowsArgumentException()
    {
        var config = new LogManagerConfig();

        Assert.Throws<ArgumentException>(() => config.GetLoggerConfig(" "));
        Assert.Throws<ArgumentException>(() => config.SetLogLevel("\t", LogLevel.Info));
        Assert.Throws<ArgumentException>(() => config.RemoveLoggerConfig("\r\n"));
    }

    [TestMethod]
    public void Reinitialize_WithSameLoggerName_UsesNewWriters()
    {
        var firstWriter = new BufferLogWriter();
        LogManager.Initialize<LogMessageSyncProcessor>(CreateConfig(firstWriter, LogLevel.Trace));
        var firstLogger = LogManager.GetLogger("Tests.Reinitialize.SameName");
        firstLogger.Info("first");
        LogManager.Shutdown();

        var secondWriter = new BufferLogWriter();
        LogManager.Initialize<LogMessageSyncProcessor>(CreateConfig(secondWriter, LogLevel.Trace));
        var secondLogger = LogManager.GetLogger("Tests.Reinitialize.SameName");
        secondLogger.Info("second");
        LogManager.Shutdown();

        Assert.AreEqual(1, firstWriter.Messages.Count);
        Assert.AreEqual("first", firstWriter.Messages[0].Text);
        Assert.AreEqual(1, secondWriter.Messages.Count);
        Assert.AreEqual("second", secondWriter.Messages[0].Text);
    }

    [TestMethod]
    public void Reinitialize_WithExistingLoggerReference_UsesNewConfiguration()
    {
        var firstWriter = new BufferLogWriter();
        LogManager.Initialize<LogMessageSyncProcessor>(CreateConfig(firstWriter, LogLevel.Trace));
        var logger = LogManager.GetLogger("Tests.Reinitialize.ExistingReference");
        logger.Info("first");
        LogManager.Shutdown();

        var secondWriter = new BufferLogWriter();
        LogManager.Initialize<LogMessageSyncProcessor>(CreateConfig(secondWriter, LogLevel.Trace));
        logger.Info("second");
        LogManager.Shutdown();

        Assert.AreEqual(1, firstWriter.Messages.Count);
        Assert.AreEqual("first", firstWriter.Messages[0].Text);
        Assert.AreEqual(1, secondWriter.Messages.Count);
        Assert.AreEqual("second", secondWriter.Messages[0].Text);
    }

    [TestMethod]
    public void MultipleWriters_FanOutToAllConfiguredWriters()
    {
        var firstWriter = new BufferLogWriter();
        var secondWriter = new BufferLogWriter();
        var config = new LogManagerConfig
        {
            RootLogger =
            {
                MinimumLevel = LogLevel.Trace,
                Writers =
                {
                    firstWriter,
                    secondWriter
                }
            }
        };

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.FanOut");
        logger.Info("fan-out");
        LogManager.Shutdown();

        Assert.AreEqual(1, firstWriter.Messages.Count);
        Assert.AreEqual(1, secondWriter.Messages.Count);
        Assert.AreEqual("fan-out", firstWriter.Messages[0].Text);
        Assert.AreEqual("fan-out", secondWriter.Messages[0].Text);
    }

    [TestMethod]
    public void LoggerHierarchy_UsesNearestMatchingConfiguration()
    {
        var writer = new BufferLogWriter();
        var config = CreateConfig(writer, LogLevel.Trace);
        config.Loggers.Add("App", LogLevel.Error);
        config.Loggers.Add("App.Service", LogLevel.Debug);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var loggerForService = LogManager.GetLogger("App.Service.Api");
        var loggerForOther = LogManager.GetLogger("App.Other.Api");

        loggerForService.Debug("service-debug");
        loggerForOther.Info("other-info");
        loggerForOther.Error("other-error");

        LogManager.Shutdown();

        Assert.AreEqual(2, writer.Messages.Count);
        Assert.AreEqual("service-debug", writer.Messages[0].Text);
        Assert.AreEqual("other-error", writer.Messages[1].Text);
    }

    [TestMethod]
    public void SequenceId_IsUniqueUnderConcurrentSyncLogging()
    {
        var writer = new SequenceCaptureWriter();
        LogManager.Initialize<LogMessageSyncProcessor>(CreateConfig(writer, LogLevel.Trace));
        var logger = LogManager.GetLogger("Tests.Sequence.Concurrent");

        const int messageCount = 20_000;
        Parallel.For(0, messageCount, index =>
        {
            logger.Info($"message-{index}");
        });

        LogManager.Shutdown();

        var snapshot = writer.SequenceIds.ToArray();
        Assert.AreEqual(messageCount, snapshot.Length);
        Assert.AreEqual(messageCount, snapshot.Distinct().Count());
        Assert.AreEqual(1L, snapshot.Min());
        Assert.AreEqual(messageCount, snapshot.Max());
    }

    private static LogManagerConfig CreateConfig(LogWriter writer, LogLevel level)
    {
        return new LogManagerConfig
        {
            RootLogger =
            {
                MinimumLevel = level,
                Writers =
                {
                    writer
                }
            }
        };
    }

    private static void WaitForCount(BufferLogWriter writer, int expectedCount, TimeSpan timeout)
    {
        var start = DateTime.UtcNow;
        while (DateTime.UtcNow - start < timeout)
        {
            if (writer.Messages.Count >= expectedCount)
            {
                return;
            }

            Thread.Sleep(10);
        }

        Assert.Fail($"Timed out waiting for {expectedCount} messages. Current count: {writer.Messages.Count}");
    }

    private sealed class BufferLogWriter : LogWriter
    {
        public List<LoggedMessage> Messages { get; } = [];

        protected override void Log(in LogMessage logMessage)
        {
            Messages.Add(
                new LoggedMessage(
                    logMessage.Level,
                    logMessage.Logger.Name,
                    logMessage.Text.ToString(),
                    logMessage.EventId,
                    logMessage.Exception,
                    logMessage.Timestamp,
                    logMessage.Thread.ManagedThreadId));
        }
    }

    private sealed class NoTextLogWriter : LogWriter
    {
        public int CallCount { get; private set; }
        public int TextLength { get; private set; }

        protected override void Log(in LogMessage logMessage)
        {
            CallCount++;
            TextLength = logMessage.Text.Length;
        }
    }

    private sealed class MinimalMetadataLogWriter : LogWriter
    {
        public int CallCount { get; private set; }
        public int TextLength { get; private set; }
        public LogEventId EventId { get; private set; }
        public Exception? Exception { get; private set; }
        public int PropertiesCount { get; private set; }
        public int ScopeCount { get; private set; }

        protected override void Log(in LogMessage logMessage)
        {
            CallCount++;
            TextLength = logMessage.Text.Length;
            EventId = logMessage.EventId;
            Exception = logMessage.Exception;
            PropertiesCount = logMessage.Properties.Count;
            ScopeCount = logMessage.Scope.Count;
        }
    }

    private sealed class SequenceCaptureWriter : LogWriter
    {
        public ConcurrentBag<long> SequenceIds { get; } = [];

        protected override void Log(in LogMessage logMessage)
        {
            SequenceIds.Add(logMessage.SequenceId);
        }
    }

    private readonly record struct LoggedMessage(
        LogLevel Level,
        string LoggerName,
        string Text,
        LogEventId EventId,
        Exception? Exception,
        DateTime Timestamp,
        int ManagedThreadId);
}
