// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Text;
using XenoAtom.Logging.Formatters;

namespace XenoAtom.Logging.Writers;

/// <summary>
/// Options used to configure <see cref="FileLogWriter"/>.
/// </summary>
public sealed class FileLogWriterOptions
{
    /// <summary>
    /// Initializes a new instance of <see cref="FileLogWriterOptions"/>.
    /// </summary>
    /// <param name="filePath">The destination file path.</param>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is empty or whitespace.</exception>
    public FileLogWriterOptions(string filePath)
    {
        if (filePath is null)
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Value cannot be empty or whitespace.", nameof(filePath));
        }

        FilePath = filePath;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="FileLogWriterOptions"/> by copying another instance.
    /// </summary>
    /// <param name="options">The source options.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
    public FileLogWriterOptions(FileLogWriterOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        FilePath = options.FilePath;
        Encoding = options.Encoding;
        NewLine = options.NewLine;
        Formatter = options.Formatter;
        UseSegments = options.UseSegments;
        AutoFlush = options.AutoFlush;
        FileSizeLimitBytes = options.FileSizeLimitBytes;
        RollingInterval = options.RollingInterval;
        RetainedFileCountLimit = options.RetainedFileCountLimit;
        RollOnStartup = options.RollOnStartup;
        FileShare = options.FileShare;
        FileBufferSize = options.FileBufferSize;
        FileOptions = options.FileOptions;
        FlushToDisk = options.FlushToDisk;
        ArchiveTimestampMode = options.ArchiveTimestampMode;
        ArchiveFileNameFormatter = options.ArchiveFileNameFormatter;
        FailureMode = options.FailureMode;
        FailureHandler = options.FailureHandler;
        RetryCount = options.RetryCount;
        RetryDelay = options.RetryDelay;
    }

    /// <summary>
    /// Gets the destination file path.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Gets or sets the encoding used to write log text.
    /// </summary>
    public Encoding Encoding { get; set; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    /// <summary>
    /// Gets or sets the newline appended after each message.
    /// </summary>
    public string NewLine { get; set; } = Environment.NewLine;

    /// <summary>
    /// Gets or sets the formatter used to render messages.
    /// </summary>
    public LogFormatter Formatter { get; set; } = StandardLogFormatter.Instance;

    /// <summary>
    /// Gets or sets whether formatter segment metadata should be requested.
    /// </summary>
    public bool UseSegments { get; set; }

    /// <summary>
    /// Gets or sets whether the stream is flushed after each write.
    /// </summary>
    public bool AutoFlush { get; set; }

    /// <summary>
    /// Gets or sets the maximum active file size in bytes before size-based rolling.
    /// </summary>
    public long? FileSizeLimitBytes { get; set; }

    /// <summary>
    /// Gets or sets the time-based rolling interval.
    /// </summary>
    public FileRollingInterval RollingInterval { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of archived files to keep.
    /// </summary>
    public int? RetainedFileCountLimit { get; set; }

    /// <summary>
    /// Gets or sets whether to roll existing content on startup.
    /// </summary>
    public bool RollOnStartup { get; set; }

    /// <summary>
    /// Gets or sets the file-sharing mode for the underlying stream.
    /// </summary>
    public FileShare FileShare { get; set; } = FileShare.ReadWrite;

    /// <summary>
    /// Gets or sets the file-stream buffer size.
    /// </summary>
    public int FileBufferSize { get; set; } = 16 * 1024;

    /// <summary>
    /// Gets or sets additional file options for the underlying stream.
    /// </summary>
    public FileOptions FileOptions { get; set; } = FileOptions.SequentialScan;

    /// <summary>
    /// Gets or sets whether flush operations force writes to durable storage.
    /// </summary>
    public bool FlushToDisk { get; set; }

    /// <summary>
    /// Gets or sets which clock is used when generating archive file timestamps.
    /// </summary>
    public FileArchiveTimestampMode ArchiveTimestampMode { get; set; } = FileArchiveTimestampMode.Utc;

    /// <summary>
    /// Gets or sets an optional callback used to build archive file names when rolling.
    /// </summary>
    /// <remarks>
    /// When <see langword="null"/>, the default archive naming format is used:
    /// <c>&lt;base&gt;.&lt;yyyyMMddHHmmssfff&gt;[.&lt;sequence&gt;]&lt;extension&gt;</c>.
    /// </remarks>
    public Func<FileArchiveFileNameContext, string>? ArchiveFileNameFormatter { get; set; }

    /// <summary>
    /// Gets or sets how write/roll I/O failures are handled.
    /// </summary>
    public FileLogWriterFailureMode FailureMode { get; set; } = FileLogWriterFailureMode.Throw;

    /// <summary>
    /// Gets or sets an optional callback invoked when a write operation fails.
    /// </summary>
    public Action<FileLogWriterFailureContext>? FailureHandler { get; set; }

    /// <summary>
    /// Gets or sets the number of retries used when <see cref="FailureMode"/> is <see cref="FileLogWriterFailureMode.Retry"/>.
    /// </summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the delay between retries.
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(50);
}
