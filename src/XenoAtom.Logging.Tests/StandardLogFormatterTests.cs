// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Text.RegularExpressions;
using XenoAtom.Logging.Formatters;

namespace XenoAtom.Logging.Tests;

[TestClass]
public class StandardLogFormatterTests
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
    public void StandardFormatter_DefaultLevelFormat_IsTriAndAligned()
    {
        var writer = new FormatterCaptureLogWriter(StandardLogFormatter.Instance);
        LogManager.Initialize<LogMessageSyncProcessor>(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.Formatter.Tri");

        logger.Info("hello");

        var message = writer.Single();
        Assert.IsTrue(message.Text.Contains(" INF   Tests.Formatter.Tri", StringComparison.Ordinal), message.Text);
        Assert.IsTrue(message.Text.EndsWith("hello", StringComparison.Ordinal));
        CollectionAssert.AreEqual(
            new[]
            {
                LogMessageFormatSegmentKind.Timestamp,
                LogMessageFormatSegmentKind.Level,
                LogMessageFormatSegmentKind.LoggerName,
                LogMessageFormatSegmentKind.Text,
            },
            message.Segments.Select(static x => x.Kind).ToArray());
    }

    [TestMethod]
    public void StandardFormatter_AllowsLevelFormatOverrideToShort()
    {
        var formatter = StandardLogFormatter.Instance with { LevelFormat = LogLevelFormat.Short };
        var writer = new FormatterCaptureLogWriter(formatter);
        LogManager.Initialize<LogMessageSyncProcessor>(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.Formatter.Short");

        logger.Info("hello");

        var message = writer.Single();
        Assert.IsTrue(message.Text.Contains(" INFO  Tests.Formatter.Short", StringComparison.Ordinal), message.Text);
    }

    [TestMethod]
    public void StandardFormatter_AllowsLevelFormatOverrideToLong()
    {
        var formatter = StandardLogFormatter.Instance with { LevelFormat = LogLevelFormat.Long };
        var writer = new FormatterCaptureLogWriter(formatter);
        LogManager.Initialize<LogMessageSyncProcessor>(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.Formatter.Long");

        logger.Info("hello");

        var message = writer.Single();
        Assert.IsTrue(message.Text.Contains(" Information Tests.Formatter.Long", StringComparison.Ordinal), message.Text);
    }

    [TestMethod]
    public void StandardFormatter_AllowsLevelFormatOverrideToChar()
    {
        var formatter = StandardLogFormatter.Instance with { LevelFormat = LogLevelFormat.Char };
        var writer = new FormatterCaptureLogWriter(formatter);
        LogManager.Initialize<LogMessageSyncProcessor>(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.Formatter.Char");

        logger.Warn("hello");

        var message = writer.Single();
        Assert.IsTrue(message.Text.Contains(" W     Tests.Formatter.Char", StringComparison.Ordinal), message.Text);
    }

    [TestMethod]
    public void StandardFormatter_AllowsTimestampFormatOverride()
    {
        var formatter = StandardLogFormatter.Instance with { TimestampFormat = "HH:mm:ss" };
        var writer = new FormatterCaptureLogWriter(formatter);
        LogManager.Initialize<LogMessageSyncProcessor>(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.Formatter.Time");

        logger.Info("hello");

        var message = writer.Single();
        Assert.AreEqual(':', message.Text[2]);
        Assert.AreEqual(':', message.Text[5]);
        Assert.AreEqual(' ', message.Text[8]);
        Assert.IsFalse(message.Text.AsSpan(0, 8).Contains('-'), message.Text);
    }

    [TestMethod]
    public void StandardFormatter_FormatsEventIdAndExceptionSegments()
    {
        var writer = new FormatterCaptureLogWriter(StandardLogFormatter.Instance);
        LogManager.Initialize<LogMessageSyncProcessor>(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.Formatter.EventException");
        var exception = new InvalidOperationException("boom");

        logger.Error(new LogEventId(7, "Evt"), exception, "failed");

        var message = writer.Single();
        Assert.IsTrue(message.Text.Contains("[7:Evt]", StringComparison.Ordinal));
        Assert.IsTrue(message.Text.Contains("failed", StringComparison.Ordinal));
        Assert.IsTrue(message.Text.Contains("InvalidOperationException", StringComparison.Ordinal));
        CollectionAssert.Contains(message.Segments.Select(static x => x.Kind).ToArray(), LogMessageFormatSegmentKind.EventId);
        CollectionAssert.Contains(message.Segments.Select(static x => x.Kind).ToArray(), LogMessageFormatSegmentKind.Exception);
    }

    [TestMethod]
    public void StandardFormatter_DoesNotEmitConditionalPartsWhenEventIdAndExceptionMissing()
    {
        var writer = new FormatterCaptureLogWriter(StandardLogFormatter.Instance);
        LogManager.Initialize<LogMessageSyncProcessor>(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.Formatter.NoOptional");

        logger.Info("hello");

        var message = writer.Single();
        Assert.IsFalse(message.Text.Contains(" [", StringComparison.Ordinal), message.Text);
        Assert.IsFalse(message.Text.Contains(" | ", StringComparison.Ordinal), message.Text);
    }

    [TestMethod]
    public void StandardFormatter_ReturnsFalse_WhenDestinationIsTooSmall()
    {
        var writer = new TooSmallBufferAssertionWriter(StandardLogFormatter.Instance);
        LogManager.Initialize<LogMessageSyncProcessor>(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.Formatter.SmallBuffer");

        logger.Info("this message does not fit");

        Assert.AreEqual(1, writer.InvocationCount);
    }

    [TestMethod]
    public void StandardFormatter_FormatsVeryLongMessage()
    {
        var writer = new FormatterCaptureLogWriter(StandardLogFormatter.Instance);
        LogManager.Initialize<LogMessageSyncProcessor>(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.Formatter.Long");
        var payload = new string('x', 20_000);

        logger.Info(payload);

        var message = writer.Single();
        Assert.IsTrue(message.Text.Contains(payload, StringComparison.Ordinal));
        CollectionAssert.Contains(message.Segments.Select(static x => x.Kind).ToArray(), LogMessageFormatSegmentKind.Text);
    }

    [TestMethod]
    public void CompactFormatter_UsesTriLevelByDefault()
    {
        var writer = new FormatterCaptureLogWriter(CompactLogFormatter.Instance);
        LogManager.Initialize<LogMessageSyncProcessor>(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.Formatter.Compact");

        logger.Info("compact");

        var message = writer.Single();
        Assert.IsTrue(message.Text.Contains(" INF compact", StringComparison.Ordinal), message.Text);
        CollectionAssert.AreEqual(
            new[]
            {
                LogMessageFormatSegmentKind.Timestamp,
                LogMessageFormatSegmentKind.Level,
                LogMessageFormatSegmentKind.Text,
            },
            message.Segments.Select(static x => x.Kind).ToArray());
    }

    [TestMethod]
    public void DetailedFormatter_EmitsThreadAndSequenceSegments()
    {
        var writer = new FormatterCaptureLogWriter(DetailedLogFormatter.Instance);
        LogManager.Initialize<LogMessageSyncProcessor>(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.Formatter.Detailed");

        logger.Info("detailed");

        var message = writer.Single();
        Assert.IsTrue(message.Text.Contains($"[{Thread.CurrentThread.ManagedThreadId}]", StringComparison.Ordinal), message.Text);
        Assert.IsTrue(Regex.IsMatch(message.Text, "#\\d{6}", RegexOptions.CultureInvariant), message.Text);
        var kinds = message.Segments.Select(static x => x.Kind).ToArray();
        CollectionAssert.Contains(kinds, LogMessageFormatSegmentKind.ThreadId);
        CollectionAssert.Contains(kinds, LogMessageFormatSegmentKind.SequenceId);
    }

    [TestMethod]
    public void DetailedFormatter_FormatsEventIdAndException()
    {
        var writer = new FormatterCaptureLogWriter(DetailedLogFormatter.Instance);
        LogManager.Initialize<LogMessageSyncProcessor>(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.Formatter.DetailedOptional");

        logger.Error(new LogEventId(42, "RequestFailed"), new InvalidOperationException("boom"), "detailed");

        var message = writer.Single();
        Assert.IsTrue(message.Text.Contains("[42:RequestFailed]", StringComparison.Ordinal), message.Text);
        Assert.IsTrue(message.Text.Contains(" | System.InvalidOperationException: boom", StringComparison.Ordinal), message.Text);
    }

    [TestMethod]
    public void DetailedFormatter_AllowsLevelFormatOverrideToShort()
    {
        var formatter = DetailedLogFormatter.Instance with { LevelFormat = LogLevelFormat.Short };
        var writer = new FormatterCaptureLogWriter(formatter);
        LogManager.Initialize<LogMessageSyncProcessor>(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.Formatter.DetailedShort");

        logger.Info("detailed");

        var message = writer.Single();
        Assert.IsTrue(message.Text.Contains(" INFO  [", StringComparison.Ordinal), message.Text);
    }

    [TestMethod]
    public void DetailedFormatter_AllowsTimestampFormatOverride()
    {
        var formatter = DetailedLogFormatter.Instance with { TimestampFormat = "HH:mm:ss" };
        var writer = new FormatterCaptureLogWriter(formatter);
        LogManager.Initialize<LogMessageSyncProcessor>(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.Formatter.DetailedTime");

        logger.Info("detailed");

        var message = writer.Single();
        Assert.AreEqual(':', message.Text[2]);
        Assert.AreEqual(':', message.Text[5]);
        Assert.AreEqual(' ', message.Text[8]);
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

    private sealed class FormatterCaptureLogWriter : LogWriter
    {
        private readonly LogFormatter _formatter;

        public FormatterCaptureLogWriter(LogFormatter formatter)
        {
            _formatter = formatter;
        }

        public List<FormattedLogMessage> Messages { get; } = [];

        public FormattedLogMessage Single()
        {
            Assert.AreEqual(1, Messages.Count);
            return Messages[0];
        }

        protected override void Log(in LogMessage logMessage)
        {
            var segments = new LogMessageFormatSegments(true);
            try
            {
                var bufferLength = 256;
                while (true)
                {
                    segments.Reset();
                    var buffer = new char[bufferLength];
                    if (_formatter.TryFormat(in logMessage, buffer, out var charsWritten, ref segments))
                    {
                        Messages.Add(new FormattedLogMessage(new string(buffer, 0, charsWritten), segments.AsSpan().ToArray()));
                        return;
                    }

                    bufferLength *= 2;
                    if (bufferLength > 2 * 1024 * 1024)
                    {
                        Assert.Fail("Formatter failed to produce output within expected buffer limits.");
                    }
                }
            }
            finally
            {
                segments.Dispose();
            }
        }
    }

    private sealed class TooSmallBufferAssertionWriter : LogWriter
    {
        private readonly LogFormatter _formatter;

        public TooSmallBufferAssertionWriter(LogFormatter formatter)
        {
            _formatter = formatter;
        }

        public int InvocationCount { get; private set; }

        protected override void Log(in LogMessage logMessage)
        {
            InvocationCount++;
            var destination = new char[8];
            var segments = new LogMessageFormatSegments(true);
            try
            {
                var result = _formatter.TryFormat(in logMessage, destination, out var charsWritten, ref segments);
                Assert.IsFalse(result);
                Assert.AreEqual(0, charsWritten);
            }
            finally
            {
                segments.Dispose();
            }
        }
    }

    private readonly record struct FormattedLogMessage(string Text, LogMessageFormatSegment[] Segments);
}
