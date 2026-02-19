// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Globalization;
using System.Text.Json;
using XenoAtom.Logging.Formatters;
using XenoAtom.Logging.Writers;

namespace XenoAtom.Logging.Tests;

[TestClass]
public class FileLogWriterTests
{
    private string _tempDirectory = null!;

    [TestInitialize]
    public void Initialize()
    {
        LogManager.Shutdown();
        _tempDirectory = Path.Combine(Path.GetTempPath(), "XenoAtom.Logging.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDirectory);
    }

    [TestCleanup]
    public void Cleanup()
    {
        LogManager.Shutdown();
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, recursive: true);
        }
    }

    [TestMethod]
    public void FileLogWriter_WritesFormattedEntries()
    {
        var filePath = Path.Combine(_tempDirectory, "app.log");
        var writer = new FileLogWriter(new FileLogWriterOptions(filePath) { AutoFlush = true });
        var config = CreateConfig(writer);

        LogManager.Initialize(config);
        var logger = LogManager.GetLogger("Tests.File.Basic");
        logger.Info("hello file");
        LogManager.Shutdown();

        var lines = File.ReadAllLines(filePath);
        Assert.AreEqual(1, lines.Length);
        Assert.IsTrue(lines[0].Contains("hello file", StringComparison.Ordinal));
        Assert.IsTrue(lines[0].Contains("Tests.File.Basic", StringComparison.Ordinal));
    }

    [TestMethod]
    public void FileLogWriter_StripsMarkupPayloads()
    {
        var filePath = Path.Combine(_tempDirectory, "markup.log");
        var writer = new FileLogWriter(new FileLogWriterOptions(filePath) { AutoFlush = true });
        var config = CreateConfig(writer);

        LogManager.Initialize(config);
        var logger = LogManager.GetLogger("Tests.File.Markup");
        logger.InfoMarkup("[green]ready[/] [bold]ok[/]");
        LogManager.Shutdown();

        var lines = File.ReadAllLines(filePath);
        Assert.AreEqual(1, lines.Length);
        Assert.IsTrue(lines[0].Contains("ready ok", StringComparison.Ordinal));
        Assert.IsFalse(lines[0].Contains("[green]", StringComparison.Ordinal));
        Assert.IsFalse(lines[0].Contains("[bold]", StringComparison.Ordinal));
    }

    [TestMethod]
    public void FileLogWriter_RollsBySize_AndAppliesRetention()
    {
        var filePath = Path.Combine(_tempDirectory, "app.log");
        var writer = new FileLogWriter(
            new FileLogWriterOptions(filePath)
            {
                AutoFlush = true,
                FileSizeLimitBytes = 96,
                RetainedFileCountLimit = 2
            });

        LogManager.Initialize(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.File.Roll.Size");

        for (var index = 0; index < 30; index++)
        {
            logger.Info($"entry-{index}-abcdefghijklmnopqrstuvwxyz");
        }

        LogManager.Shutdown();

        var archivedFiles = Directory.GetFiles(_tempDirectory, "app.*.log", SearchOption.TopDirectoryOnly);
        var allFiles = Directory.GetFiles(_tempDirectory, "app*.log", SearchOption.TopDirectoryOnly);

        Assert.IsTrue(allFiles.Length >= 2, "Expected active + archived files.");
        Assert.IsTrue(archivedFiles.Length <= 2, "Retention limit was not applied.");
        Assert.IsTrue(File.Exists(filePath), "Active file is missing.");
    }

    [TestMethod]
    public void FileLogWriter_RollsByInterval()
    {
        var filePath = Path.Combine(_tempDirectory, "app.log");
        var timeProvider = new MutableTimeProvider(new DateTimeOffset(2026, 01, 01, 10, 0, 0, TimeSpan.Zero));
        var writer = new FileLogWriter(
            new FileLogWriterOptions(filePath)
            {
                AutoFlush = true,
                RollingInterval = FileRollingInterval.Daily
            });

        var config = CreateConfig(writer);
        config.TimeProvider = timeProvider;

        LogManager.Initialize(config);
        var logger = LogManager.GetLogger("Tests.File.Roll.Interval");

        logger.Info("day-one");
        timeProvider.SetUtcNow(new DateTimeOffset(2026, 01, 02, 10, 0, 0, TimeSpan.Zero));
        logger.Info("day-two");
        LogManager.Shutdown();

        var archivedFiles = Directory.GetFiles(_tempDirectory, "app.*.log", SearchOption.TopDirectoryOnly);
        Assert.AreEqual(1, archivedFiles.Length);

        var archivedText = File.ReadAllText(archivedFiles[0]);
        var activeText = File.ReadAllText(filePath);

        Assert.IsTrue(archivedText.Contains("day-one", StringComparison.Ordinal));
        Assert.IsFalse(archivedText.Contains("day-two", StringComparison.Ordinal));
        Assert.IsTrue(activeText.Contains("day-two", StringComparison.Ordinal));
    }

    [TestMethod]
    public void JsonFileLogWriter_WritesValidJsonLines()
    {
        var filePath = Path.Combine(_tempDirectory, "app.jsonl");
        var writer = new JsonFileLogWriter(
            new FileLogWriterOptions(filePath)
            {
                AutoFlush = true
            });
        var config = CreateConfig(writer);
        LogManager.Initialize(config);

        var logger = LogManager.GetLogger("Tests.File.Json");
        var properties = new LogProperties { ("UserId", 42), ("Name", "Ada") };
        logger.Info(new LogEventId(7, "UserEvent"), "structured message", properties);
        LogManager.Shutdown();

        var lines = File.ReadAllLines(filePath);
        Assert.AreEqual(1, lines.Length);

        using var document = JsonDocument.Parse(lines[0]);
        var root = document.RootElement;
        Assert.AreEqual("Tests.File.Json", root.GetProperty("logger").GetString());
        Assert.AreEqual("Information", root.GetProperty("level").GetString());
        Assert.AreEqual("structured message", root.GetProperty("message").GetString());
        Assert.AreEqual(7, root.GetProperty("eventId").GetProperty("id").GetInt32());
        Assert.AreEqual("UserEvent", root.GetProperty("eventId").GetProperty("name").GetString());
        Assert.AreEqual(2, root.GetProperty("properties").GetArrayLength());
    }

    [TestMethod]
    public void JsonFileLogWriter_CanUseFormatterOptions()
    {
        var filePath = Path.Combine(_tempDirectory, "app.jsonl");
        var writer = new JsonFileLogWriter(
            new FileLogWriterOptions(filePath)
            {
                AutoFlush = true
            },
            new JsonLogFormatterOptions
            {
                FieldNamingPolicy = JsonLogFieldNamingPolicy.SnakeCase,
                IncludeScopes = false
            });

        LogManager.Initialize(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.File.Json.Options");
        logger.Info(new LogEventId(12, "SnakeEvent"), "snake-message", new LogProperties { ("UserId", 42) });
        LogManager.Shutdown();

        var lines = File.ReadAllLines(filePath);
        Assert.AreEqual(1, lines.Length);

        using var document = JsonDocument.Parse(lines[0]);
        var root = document.RootElement;

        Assert.AreEqual("Tests.File.Json.Options", root.GetProperty("logger").GetString());
        Assert.IsTrue(root.TryGetProperty("event_id", out _));
        Assert.IsFalse(root.TryGetProperty("eventId", out _));
        Assert.IsFalse(root.TryGetProperty("scopes", out _));
    }

    [TestMethod]
    public void JsonFileLogWriter_DefaultsToLfNewline()
    {
        var filePath = Path.Combine(_tempDirectory, "app.jsonl");
        var writer = new JsonFileLogWriter(filePath);
        LogManager.Initialize(CreateConfig(writer));

        var logger = LogManager.GetLogger("Tests.File.Json.NewLine");
        logger.Info("line-one");
        logger.Info("line-two");
        LogManager.Shutdown();

        var content = File.ReadAllText(filePath);
        Assert.IsTrue(content.Contains('\n'));
        Assert.IsFalse(content.Contains("\r\n", StringComparison.Ordinal), "JsonFileLogWriter should default to LF newlines.");
    }

    [TestMethod]
    public void FileLogWriter_IgnoreFailureMode_DropsFailedWriteAndInvokesHandler()
    {
        var filePath = Path.Combine(_tempDirectory, "app.log");
        var failures = new List<FileLogWriterFailureContext>();
        var writer = new SyntheticFailureFileLogWriter(
            new FileLogWriterOptions(filePath)
            {
                // Avoid relying on platform-specific file locking/rename semantics.
                // We inject a deterministic IOException during rolling to validate FailureMode.Ignore behavior.
                AutoFlush = false,
                FileSizeLimitBytes = 1,
                FailureMode = FileLogWriterFailureMode.Ignore,
                FailureHandler = context => failures.Add(context)
            });

        LogManager.Initialize(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.File.Failure.Ignore");

        logger.Info("first");
        writer.FailNextFlush();
        logger.Info("second");

        logger.Info("third");
        LogManager.Shutdown();

        Assert.IsTrue(failures.Count > 0);
        Assert.IsTrue(failures.Any(static failure => !failure.WillRetry));

        var logFiles = Directory.GetFiles(_tempDirectory, "app*.log", SearchOption.TopDirectoryOnly);
        var contents = string.Concat(logFiles.Select(File.ReadAllText));
        Assert.IsTrue(contents.Contains("first", StringComparison.Ordinal));
        Assert.IsTrue(contents.Contains("third", StringComparison.Ordinal));
    }

    [TestMethod]
    public void FileLogWriter_ArchiveTimestampMode_UsesConfiguredClock()
    {
        var fixedUtcTime = new DateTimeOffset(2026, 02, 08, 11, 22, 33, TimeSpan.Zero);
        var utcStamp = fixedUtcTime.UtcDateTime.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        var localStamp = fixedUtcTime.LocalDateTime.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);

        var utcDirectory = Path.Combine(_tempDirectory, "utc");
        Directory.CreateDirectory(utcDirectory);
        var utcFilePath = Path.Combine(utcDirectory, "app.log");
        var utcWriter = new FileLogWriter(
            new FileLogWriterOptions(utcFilePath)
            {
                AutoFlush = true,
                FileSizeLimitBytes = 1,
                ArchiveTimestampMode = FileArchiveTimestampMode.Utc
            });

        var utcConfig = CreateConfig(utcWriter);
        utcConfig.TimeProvider = new MutableTimeProvider(fixedUtcTime);
        LogManager.Initialize(utcConfig);
        var utcLogger = LogManager.GetLogger("Tests.File.Archive.Utc");
        utcLogger.Info("first");
        utcLogger.Info("second");
        LogManager.Shutdown();

        var utcArchive = Path.GetFileName(Directory.GetFiles(utcDirectory, "app.*.log").Single());
        Assert.IsTrue(utcArchive.Contains(utcStamp, StringComparison.Ordinal));

        var localDirectory = Path.Combine(_tempDirectory, "local");
        Directory.CreateDirectory(localDirectory);
        var localFilePath = Path.Combine(localDirectory, "app.log");
        var localWriter = new FileLogWriter(
            new FileLogWriterOptions(localFilePath)
            {
                AutoFlush = true,
                FileSizeLimitBytes = 1,
                ArchiveTimestampMode = FileArchiveTimestampMode.Local
            });

        var localConfig = CreateConfig(localWriter);
        localConfig.TimeProvider = new MutableTimeProvider(fixedUtcTime);
        LogManager.Initialize(localConfig);
        var localLogger = LogManager.GetLogger("Tests.File.Archive.Local");
        localLogger.Info("first");
        localLogger.Info("second");
        LogManager.Shutdown();

        var localArchive = Path.GetFileName(Directory.GetFiles(localDirectory, "app.*.log").Single());
        Assert.IsTrue(localArchive.Contains(localStamp, StringComparison.Ordinal));
    }

    [TestMethod]
    public void FileLogWriter_ArchiveFileNameFormatter_CanUseReadableDateTimeFormat()
    {
        var fixedUtcTime = new DateTimeOffset(2026, 02, 09, 19, 16, 3, TimeSpan.Zero);
        var expectedArchiveFileName = "output.2026-02-09-19_16_03.txt";
        var filePath = Path.Combine(_tempDirectory, "output.txt");

        var writer = new FileLogWriter(
            new FileLogWriterOptions(filePath)
            {
                AutoFlush = true,
                FileSizeLimitBytes = 1,
                ArchiveFileNameFormatter = FileArchiveFileNameFormatters.DateTime
            });

        var config = CreateConfig(writer);
        config.TimeProvider = new MutableTimeProvider(fixedUtcTime);

        LogManager.Initialize(config);
        var logger = LogManager.GetLogger("Tests.File.Archive.Format");
        logger.Info("first");
        logger.Info("second");
        LogManager.Shutdown();

        var archives = Directory.GetFiles(_tempDirectory, "output.*.txt", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .ToArray();
        CollectionAssert.Contains(archives, expectedArchiveFileName);
    }

    [TestMethod]
    public void FileLogWriter_ArchiveFileNameFormatter_IgnoresSequence_StillProducesUniqueArchiveName()
    {
        var fixedUtcTime = new DateTimeOffset(2026, 02, 09, 19, 16, 3, TimeSpan.Zero);
        var filePath = Path.Combine(_tempDirectory, "output.txt");
        var collidingArchivePath = Path.Combine(_tempDirectory, "output.2026-02-09-19_16_03.txt");

        File.WriteAllText(collidingArchivePath, "collision");

        var writer = new FileLogWriter(
            new FileLogWriterOptions(filePath)
            {
                AutoFlush = true,
                FileSizeLimitBytes = 1,
                ArchiveFileNameFormatter = static context => $"{context.BaseFileName}.{context.Timestamp.ToString("yyyy-MM-dd-HH_mm_ss", CultureInfo.InvariantCulture)}{context.Extension}"
            });

        var config = CreateConfig(writer);
        config.TimeProvider = new MutableTimeProvider(fixedUtcTime);

        LogManager.Initialize(config);
        var logger = LogManager.GetLogger("Tests.File.Archive.Format.SequenceFallback");
        logger.Info("first");
        logger.Info("second");
        LogManager.Shutdown();

        Assert.IsTrue(File.Exists(collidingArchivePath), "The pre-existing archive should remain untouched.");

        var generatedArchives = Directory.GetFiles(_tempDirectory, "output.*.txt", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .ToArray();
        Assert.IsTrue(generatedArchives.Any(name => string.Equals(name, "output.2026-02-09-19_16_03.1.txt", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void FileLogWriter_ArchiveFileNameFormatter_InvalidPath_Throws()
    {
        var filePath = Path.Combine(_tempDirectory, "output.txt");
        var writer = new FileLogWriter(
            new FileLogWriterOptions(filePath)
            {
                AutoFlush = true,
                FileSizeLimitBytes = 1,
                ArchiveFileNameFormatter = static _ => "nested/output.txt"
            });

        LogManager.Initialize(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.File.Archive.InvalidFormatter");
        logger.Info("first");

        var exception = Assert.Throws<InvalidOperationException>(() => logger.Info("second"));
        Assert.IsNotNull(exception);
        StringAssert.Contains(exception.Message, "must return a file name, not a path");
        LogManager.Shutdown();
    }

    [TestMethod]
    public void FileLogWriter_FlushToDisk_UsesDurableFlushMode()
    {
        var filePath = Path.Combine(_tempDirectory, "app.log");
        var writer = new TrackingFlushFileLogWriter(
            new FileLogWriterOptions(filePath)
            {
                AutoFlush = true,
                FlushToDisk = true
            });

        LogManager.Initialize(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.File.FlushToDisk");
        logger.Info("flush");
        LogManager.Shutdown();

        Assert.IsTrue(writer.FlushModes.Count > 0);
        Assert.IsTrue(writer.FlushModes.All(static x => x));
    }

    [TestMethod]
    public void FileLogWriterOptions_NullPath_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new FileLogWriterOptions((string)null!));
    }

    [TestMethod]
    public void FileLogWriterOptions_WhitespacePath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new FileLogWriterOptions("   "));
    }

    [TestMethod]
    public void FileLogWriter_RollFile_HandlesArchiveNameCollision()
    {
        var fixedUtcTime = new DateTimeOffset(2026, 02, 09, 12, 30, 45, TimeSpan.Zero);
        var archiveStamp = fixedUtcTime.UtcDateTime.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        var filePath = Path.Combine(_tempDirectory, "app.log");
        var collidingArchivePath = Path.Combine(_tempDirectory, $"app.{archiveStamp}.log");

        var writer = new FileLogWriter(
            new FileLogWriterOptions(filePath)
            {
                AutoFlush = true,
                FileSizeLimitBytes = 1
            });

        var config = CreateConfig(writer);
        config.TimeProvider = new MutableTimeProvider(fixedUtcTime);

        LogManager.Initialize(config);
        var logger = LogManager.GetLogger("Tests.File.Roll.Collision");

        logger.Info("first");
        File.WriteAllText(collidingArchivePath, "collision");
        logger.Info("second");
        LogManager.Shutdown();

        var generatedArchive = Path.Combine(_tempDirectory, $"app.{archiveStamp}.1.log");
        Assert.IsTrue(File.Exists(collidingArchivePath), "The pre-created colliding archive should remain untouched.");
        Assert.IsTrue(File.Exists(generatedArchive), "Rolling should continue with next sequence suffix when archive name collides.");
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

    private sealed class MutableTimeProvider : TimeProvider
    {
        private DateTimeOffset _utcNow;

        public MutableTimeProvider(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }

        public override DateTimeOffset GetUtcNow() => _utcNow;

        public void SetUtcNow(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }
    }

    private sealed class TrackingFlushFileLogWriter : FileLogWriter
    {
        public TrackingFlushFileLogWriter(FileLogWriterOptions options) : base(options)
        {
        }

        public List<bool> FlushModes { get; } = [];

        protected override void FlushStream(FileStream stream, bool flushToDisk)
        {
            FlushModes.Add(flushToDisk);
            base.FlushStream(stream, flushToDisk);
        }
    }

    private sealed class SyntheticFailureFileLogWriter : FileLogWriter
    {
        private int _failNextFlush;

        public SyntheticFailureFileLogWriter(FileLogWriterOptions options) : base(options)
        {
        }

        public void FailNextFlush() => Volatile.Write(ref _failNextFlush, 1);

        protected override void FlushStream(FileStream stream, bool flushToDisk)
        {
            if (Interlocked.Exchange(ref _failNextFlush, 0) != 0)
            {
                throw new IOException("Synthetic test failure.");
            }

            base.FlushStream(stream, flushToDisk);
        }
    }
}
