// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Tests;

[TestClass]
public class LogMessageEncodingCoverageTests
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
    public void InterpolatedMessage_CoversScalarEncodingPaths()
    {
        var writer = new CaptureWriter();
        LogManager.Initialize(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.Encoding");

        var guid = Guid.Parse("01234567-89AB-CDEF-0123-456789ABCDEF");
        var dateTime = new DateTime(2026, 02, 08, 11, 22, 33, DateTimeKind.Utc);
        var dateTimeOffset = new DateTimeOffset(2026, 02, 08, 11, 22, 33, TimeSpan.Zero);
        var dateOnly = new DateOnly(2026, 02, 08);
        var timeOnly = new TimeOnly(11, 22, 33);
        var timeSpan = TimeSpan.FromMinutes(90);
        var enumValue = DayOfWeek.Friday;
        ReadOnlySpan<char> textSpan = "char-span";
        ReadOnlySpan<byte> byteSpan = "byte-span"u8;

        logger.Info(
            $"sbyte:{(sbyte)-1} short:{(short)-2} int:{-3} long:{-4L} byte:{(byte)5} ushort:{(ushort)6} uint:{7u} ulong:{8ul} " +
            $"float:{1.5f} double:{2.5d} decimal:{3.5m} bool:{true} char:{'Z'} guid:{guid} dt:{dateTime} dto:{dateTimeOffset} " +
            $"ts:{timeSpan} date:{dateOnly} time:{timeOnly} enum:{enumValue} chars:{textSpan} bytes:{byteSpan}");

        Assert.AreEqual(1, writer.Messages.Count);
        var message = writer.Messages[0];

        Assert.IsTrue(message.Contains("sbyte:-1", StringComparison.Ordinal));
        Assert.IsTrue(message.Contains("short:-2", StringComparison.Ordinal));
        Assert.IsTrue(message.Contains("int:-3", StringComparison.Ordinal));
        Assert.IsTrue(message.Contains("long:-4", StringComparison.Ordinal));
        Assert.IsTrue(message.Contains("byte:5", StringComparison.Ordinal));
        Assert.IsTrue(message.Contains("ushort:6", StringComparison.Ordinal));
        Assert.IsTrue(message.Contains("uint:7", StringComparison.Ordinal));
        Assert.IsTrue(message.Contains("ulong:8", StringComparison.Ordinal));
        Assert.IsTrue(message.Contains("float:1.5", StringComparison.Ordinal));
        Assert.IsTrue(message.Contains("double:2.5", StringComparison.Ordinal));
        Assert.IsTrue(message.Contains("decimal:3.5", StringComparison.Ordinal));
        Assert.IsTrue(message.Contains("bool:True", StringComparison.Ordinal));
        Assert.IsTrue(message.Contains("char:Z", StringComparison.Ordinal));
        Assert.IsTrue(message.Contains("guid:01234567-89ab-cdef-0123-456789abcdef", StringComparison.OrdinalIgnoreCase));
        Assert.IsTrue(message.Contains("date:", StringComparison.Ordinal));
        Assert.IsTrue(message.Contains("time:", StringComparison.Ordinal));
        Assert.IsTrue(message.Contains("enum:Friday", StringComparison.Ordinal));
        Assert.IsTrue(message.Contains("chars:char-span", StringComparison.Ordinal));
        Assert.IsTrue(message.Contains("bytes:byte-span", StringComparison.Ordinal));
    }

    [TestMethod]
    public void InterpolatedUtf8Span_WithPositiveAlignment_IsNotCorrupted()
    {
        var writer = new CaptureWriter();
        LogManager.Initialize(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.Encoding.Utf8Aligned");
        ReadOnlySpan<byte> utf8 = "é"u8;

        logger.Info($"aligned:[{utf8,5}]");

        Assert.AreEqual(1, writer.Messages.Count);
        Assert.AreEqual("aligned:[    é]", writer.Messages[0]);
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

    private sealed class CaptureWriter : LogWriter
    {
        public List<string> Messages { get; } = [];

        protected override void Log(LogMessage logMessage)
        {
            Messages.Add(logMessage.Text.ToString());
        }
    }
}
