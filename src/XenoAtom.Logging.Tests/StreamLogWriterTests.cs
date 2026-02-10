// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Text;
using XenoAtom.Logging.Writers;

namespace XenoAtom.Logging.Tests;

[TestClass]
public class StreamLogWriterTests
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
    public void StreamLogWriter_WritesFormattedText()
    {
        using var stream = new MemoryStream();
        var writer = new StreamLogWriter(stream, Encoding.UTF8);
        var config = CreateConfig(writer);

        LogManager.Initialize(config);
        var logger = LogManager.GetLogger("Tests.Stream.Basic");
        logger.Info("hello stream");
        LogManager.Shutdown();

        var text = Encoding.UTF8.GetString(stream.ToArray());
        Assert.IsTrue(text.Contains("Tests.Stream.Basic", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("hello stream", StringComparison.Ordinal));
    }

    [TestMethod]
    public void StreamLogWriter_AppendsNewLinePerEntry()
    {
        using var stream = new MemoryStream();
        var writer = new StreamLogWriter(stream, Encoding.UTF8);
        var config = CreateConfig(writer);

        LogManager.Initialize(config);
        var logger = LogManager.GetLogger("Tests.Stream.NewLine");
        logger.Info("first");
        logger.Info("second");
        LogManager.Shutdown();

        var text = Encoding.UTF8.GetString(stream.ToArray());
        var lines = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.AreEqual(2, lines.Length);
        Assert.IsTrue(lines[0].Contains("first", StringComparison.Ordinal));
        Assert.IsTrue(lines[1].Contains("second", StringComparison.Ordinal));
    }

    [TestMethod]
    public void StreamLogWriter_StripsMarkupPayloads()
    {
        using var stream = new MemoryStream();
        var writer = new StreamLogWriter(stream, Encoding.UTF8);
        var config = CreateConfig(writer);

        LogManager.Initialize(config);
        var logger = LogManager.GetLogger("Tests.Stream.Markup");
        logger.InfoMarkup("[green]ready[/] [bold]ok[/]");
        LogManager.Shutdown();

        var text = Encoding.UTF8.GetString(stream.ToArray());
        Assert.IsTrue(text.Contains("ready ok", StringComparison.Ordinal));
        Assert.IsFalse(text.Contains("[green]", StringComparison.Ordinal));
        Assert.IsFalse(text.Contains("[bold]", StringComparison.Ordinal));
    }

    [TestMethod]
    public void StreamLogWriter_OwnsStreamFalse_DoesNotDisposeUnderlyingStream()
    {
        using var stream = new MemoryStream();
        var writer = new StreamLogWriter(stream, ownsStream: false);
        var config = CreateConfig(writer);

        LogManager.Initialize(config);
        var logger = LogManager.GetLogger("Tests.Stream.OwnsFalse");
        logger.Info("hello");
        LogManager.Shutdown();

        Assert.IsTrue(stream.CanWrite);
        stream.WriteByte(1);
    }

    [TestMethod]
    public void StreamLogWriter_OwnsStreamTrue_DisposesUnderlyingStream()
    {
        var stream = new MemoryStream();
        var writer = new StreamLogWriter(stream, ownsStream: true);
        var config = CreateConfig(writer);

        LogManager.Initialize(config);
        var logger = LogManager.GetLogger("Tests.Stream.OwnsTrue");
        logger.Info("hello");
        LogManager.Shutdown();

        Assert.IsFalse(stream.CanWrite);
        Assert.Throws<ObjectDisposedException>(() => stream.WriteByte(1));
    }

    [TestMethod]
    public void StreamLogWriter_SerializesConcurrentWrites()
    {
        using var stream = new ReentrancyDetectingStream();
        var writer = new StreamLogWriter(stream, Encoding.UTF8);
        var config = CreateConfig(writer);

        LogManager.Initialize(config);
        var logger = LogManager.GetLogger("Tests.Stream.Concurrent");

        Parallel.For(0, 10_000, index =>
        {
            logger.Info($"message-{index}");
        });

        LogManager.Shutdown();
        Assert.IsFalse(stream.DetectedConcurrentWrites, "Concurrent writes were detected on StreamLogWriter.");
    }

    [TestMethod]
    public void StreamLogWriter_LongPayload_GrowsFormatterBufferAndWritesAllMessages()
    {
        using var stream = new MemoryStream();
        var writer = new StreamLogWriter(stream, Encoding.UTF8);
        var config = CreateConfig(writer);

        LogManager.Initialize(config);
        var logger = LogManager.GetLogger("Tests.Stream.BufferGrowth");
        var payload = new string('x', 50_000);

        logger.Info(payload);
        logger.Info(payload);
        LogManager.Shutdown();

        var text = Encoding.UTF8.GetString(stream.ToArray());
        var lines = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.AreEqual(2, lines.Length);
        Assert.IsTrue(lines[0].Contains(payload, StringComparison.Ordinal));
        Assert.IsTrue(lines[1].Contains(payload, StringComparison.Ordinal));
    }

    [TestMethod]
    public void StreamLogWriter_AutoFlush_FlushesAfterEachWrite()
    {
        using var stream = new FlushTrackingStream();
        var writer = new StreamLogWriter(stream, Encoding.UTF8)
        {
            AutoFlush = true
        };
        var config = CreateConfig(writer);

        LogManager.Initialize(config);
        var logger = LogManager.GetLogger("Tests.Stream.AutoFlush");
        logger.Info("first");
        logger.Info("second");
        LogManager.Shutdown();

        Assert.IsTrue(stream.FlushCount >= 2, $"Expected at least one flush per write. Actual flush count: {stream.FlushCount}.");
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

    private sealed class ReentrancyDetectingStream : MemoryStream
    {
        private int _activeWrites;

        public bool DetectedConcurrentWrites { get; private set; }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (Interlocked.Increment(ref _activeWrites) != 1)
            {
                DetectedConcurrentWrites = true;
            }

            try
            {
                Thread.SpinWait(512);
                base.Write(buffer, offset, count);
            }
            finally
            {
                Interlocked.Decrement(ref _activeWrites);
            }
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            var copy = buffer.ToArray();
            Write(copy, 0, copy.Length);
        }
    }

    private sealed class FlushTrackingStream : MemoryStream
    {
        public int FlushCount { get; private set; }

        public override void Flush()
        {
            FlushCount++;
            base.Flush();
        }
    }
}
