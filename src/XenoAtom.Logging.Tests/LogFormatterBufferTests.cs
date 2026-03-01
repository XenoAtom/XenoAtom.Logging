// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using XenoAtom.Logging.Helpers;

namespace XenoAtom.Logging.Tests;

[TestClass]
public class LogFormatterBufferTests
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
    public void LogFormatterBuffer_ResetsSegmentsBetweenRetries()
    {
        var formatter = new FailOnceSegmentingFormatter();
        var writer = new BufferCaptureWriter(formatter);

        var config = new LogManagerConfig
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

        LogManager.Initialize(config);
        var logger = LogManager.GetLogger("Tests.FormatterBuffer.Retry");
        logger.Info("hello");
        LogManager.Shutdown();

        Assert.AreEqual("ok", writer.Text);
        Assert.AreEqual(1, writer.Segments.Length);
        Assert.AreEqual(LogMessageFormatSegmentKind.Text, writer.Segments[0].Kind);
    }

    private sealed class BufferCaptureWriter : LogWriter
    {
        private readonly LogFormatter _formatter;

        public BufferCaptureWriter(LogFormatter formatter)
        {
            _formatter = formatter;
        }

        public string? Text { get; private set; }

        public LogMessageFormatSegment[] Segments { get; private set; } = [];

        protected override void Log(LogMessage logMessage)
        {
            using var formatterBuffer = new LogFormatterBuffer();
            var segments = new LogMessageFormatSegments(enabled: true);
            try
            {
                var text = formatterBuffer.Format(logMessage, _formatter, ref segments);
                Text = text.ToString();
                Segments = segments.AsSpan().ToArray();
            }
            finally
            {
                segments.Dispose();
            }
        }
    }

    private sealed record FailOnceSegmentingFormatter : LogFormatter
    {
        private int _tryCount;

        public override bool TryFormat(LogMessage logMessage, Span<char> destination, out int charsWritten, ref LogMessageFormatSegments segments)
        {
            segments.Add(0, 0, LogMessageFormatSegmentKind.Text);

            if (_tryCount++ == 0)
            {
                charsWritten = 0;
                return false;
            }

            if (destination.Length < 2)
            {
                charsWritten = 0;
                return false;
            }

            destination[0] = 'o';
            destination[1] = 'k';
            charsWritten = 2;
            return true;
        }
    }
}

