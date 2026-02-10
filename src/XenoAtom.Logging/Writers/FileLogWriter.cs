// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Buffers;
using System.Globalization;
using System.Text;
using XenoAtom.Logging.Formatters;
using XenoAtom.Logging.Helpers;

namespace XenoAtom.Logging.Writers;

/// <summary>
/// Writes formatted log messages to a file with optional rolling and retention policies.
/// </summary>
/// <remarks>
/// Concurrent writes are synchronized by this writer. Configure options up front and avoid mutating writer state while
/// logging is active.
/// </remarks>
public class FileLogWriter : LogWriter
{
    private readonly object _syncObject = new();
    private readonly string _filePath;
    private readonly string _directoryPath;
    private readonly string _archiveFileNamePrefix;
    private readonly string _archiveFileExtension;
    private readonly long? _fileSizeLimitBytes;
    private readonly FileRollingInterval _rollingInterval;
    private readonly int? _retainedFileCountLimit;
    private readonly bool _autoFlush;
    private readonly bool _useSegments;
    private readonly FileShare _fileShare;
    private readonly int _fileBufferSize;
    private readonly FileOptions _fileOptions;
    private readonly bool _flushToDisk;
    private readonly FileArchiveTimestampMode _archiveTimestampMode;
    private readonly FileLogWriterFailureMode _failureMode;
    private readonly Action<FileLogWriterFailureContext>? _failureHandler;
    private readonly int _retryCount;
    private readonly TimeSpan _retryDelay;
    private readonly byte[] _newLineBytes;

    private FileStream? _stream;
    private long _currentFileLength;
    private long _currentIntervalKey;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of <see cref="FileLogWriter"/> with default file options.
    /// </summary>
    /// <param name="filePath">The destination file path.</param>
    public FileLogWriter(string filePath) : this(new FileLogWriterOptions(filePath))
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="FileLogWriter"/>.
    /// </summary>
    /// <param name="options">Options used to configure file writing behavior.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">An option has an invalid value.</exception>
    public FileLogWriter(FileLogWriterOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.Encoding);
        ArgumentNullException.ThrowIfNull(options.Formatter);
        ArgumentNullException.ThrowIfNull(options.NewLine);
        if (options.FileSizeLimitBytes.HasValue && options.FileSizeLimitBytes.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "FileSizeLimitBytes must be greater than zero.");
        }

        if (options.FileBufferSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "FileBufferSize must be greater than zero.");
        }

        if (options.RetainedFileCountLimit < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "RetainedFileCountLimit must be greater than or equal to zero.");
        }

        if (options.RetryCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "RetryCount must be greater than or equal to zero.");
        }

        if (options.RetryDelay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "RetryDelay must be greater than or equal to zero.");
        }

        _filePath = Path.GetFullPath(options.FilePath);
        _directoryPath = Path.GetDirectoryName(_filePath) ?? Directory.GetCurrentDirectory();
        _archiveFileNamePrefix = Path.GetFileNameWithoutExtension(_filePath);
        _archiveFileExtension = Path.GetExtension(_filePath);

        Directory.CreateDirectory(_directoryPath);

        Encoding = options.Encoding;
        NewLine = options.NewLine;
        Formatter = options.Formatter;
        _newLineBytes = Encoding.GetBytes(NewLine);

        _fileSizeLimitBytes = options.FileSizeLimitBytes;
        _rollingInterval = options.RollingInterval;
        _retainedFileCountLimit = options.RetainedFileCountLimit;
        _autoFlush = options.AutoFlush;
        _useSegments = options.UseSegments;
        _fileShare = options.FileShare;
        _fileBufferSize = options.FileBufferSize;
        _fileOptions = options.FileOptions;
        _flushToDisk = options.FlushToDisk;
        _archiveTimestampMode = options.ArchiveTimestampMode;
        _failureMode = options.FailureMode;
        _failureHandler = options.FailureHandler;
        _retryCount = options.RetryCount;
        _retryDelay = options.RetryDelay;

        _stream = OpenFileStream(append: true);
        _currentFileLength = _stream.Length;
        _currentIntervalKey = GetIntervalKey(DateTime.UtcNow, _rollingInterval);

        if (options.RollOnStartup && _currentFileLength > 0)
        {
            RollFile(DateTime.UtcNow);
        }
    }

    /// <summary>
    /// Gets the destination file path.
    /// </summary>
    public string FilePath => _filePath;

    /// <summary>
    /// Gets the encoding used to persist log text.
    /// </summary>
    public Encoding Encoding { get; }

    /// <summary>
    /// Gets the newline appended after each entry.
    /// </summary>
    public string NewLine { get; }

    /// <summary>
    /// Gets the formatter used to render a log message before writing.
    /// </summary>
    /// <remarks>
    /// Set this property through <see cref="FileLogWriterOptions.Formatter"/> to avoid concurrent mutation while logging.
    /// </remarks>
    public LogFormatter Formatter { get; init; }

    /// <summary>
    /// Gets the configured file size limit in bytes.
    /// </summary>
    public long? FileSizeLimitBytes => _fileSizeLimitBytes;

    /// <summary>
    /// Gets the configured rolling interval.
    /// </summary>
    public FileRollingInterval RollingInterval => _rollingInterval;

    /// <summary>
    /// Gets the configured archive retention limit.
    /// </summary>
    public int? RetainedFileCountLimit => _retainedFileCountLimit;

    /// <summary>
    /// Gets the configured failure mode used for write/roll I/O errors.
    /// </summary>
    public FileLogWriterFailureMode FailureMode => _failureMode;

    /// <summary>
    /// Gets a value indicating whether flush operations force writes to durable storage.
    /// </summary>
    public bool FlushToDisk => _flushToDisk;

    /// <summary>
    /// Gets the clock mode used to generate archive file timestamps.
    /// </summary>
    public FileArchiveTimestampMode ArchiveTimestampMode => _archiveTimestampMode;

    /// <summary>
    /// Flushes the underlying stream.
    /// </summary>
    public override void Flush()
    {
        lock (_syncObject)
        {
            if (_disposed)
            {
                return;
            }

            if (_stream is { } stream)
            {
                FlushStream(stream, _flushToDisk);
            }
        }
    }

    /// <summary>
    /// Disposes this writer and closes the underlying file stream.
    /// </summary>
    public override void Dispose()
    {
        lock (_syncObject)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            if (_stream is { } stream)
            {
                FlushStream(stream, _flushToDisk);
                stream.Dispose();
                _stream = null;
            }
        }
    }

    /// <inheritdoc />
    protected sealed override void Log(in LogMessage logMessage)
    {
        using var formatterBuffer = new LogFormatterBuffer();
        var segments = new LogMessageFormatSegments(_useSegments);
        char[]? strippedBuffer = null;

        try
        {
            var formatter = Formatter;
            var text = formatterBuffer.Format(logMessage, formatter, ref segments);
            if (logMessage.IsMarkup && text.Length > 0)
            {
                strippedBuffer = ArrayPool<char>.Shared.Rent(MarkupStripper.GetMaxOutputLength(text.Length));
                var charsWritten = MarkupStripper.Strip(text, strippedBuffer);
                text = strippedBuffer.AsSpan(0, charsWritten);
            }

            WriteEntry(logMessage.Timestamp, text);
        }
        finally
        {
            if (strippedBuffer is not null)
            {
                ArrayPool<char>.Shared.Return(strippedBuffer);
            }

            segments.Dispose();
        }
    }

    private void WriteEntry(DateTime timestamp, ReadOnlySpan<char> text)
    {
        using var encoderBuffer = new LogEncoderBuffer();
        var entryBytes = encoderBuffer.Encode(text, Encoding);
        var totalBytes = checked(entryBytes.Length + _newLineBytes.Length);

        var attempt = 0;
        while (true)
        {
            try
            {
                lock (_syncObject)
                {
                    ThrowIfDisposed();
                    EnsureStream();

                    RollIfNeededByInterval(timestamp);
                    RollIfNeededBySize(totalBytes, timestamp);

                    _stream!.Write(entryBytes);
                    _stream.Write(_newLineBytes);
                    _currentFileLength += totalBytes;

                    if (_autoFlush)
                    {
                        FlushStream(_stream!, _flushToDisk);
                    }
                }
                return;
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                attempt++;
                var shouldRetry = _failureMode == FileLogWriterFailureMode.Retry && attempt <= _retryCount;
                _failureHandler?.Invoke(new FileLogWriterFailureContext(_filePath, attempt, exception, shouldRetry));

                switch (_failureMode)
                {
                    case FileLogWriterFailureMode.Throw:
                        throw;
                    case FileLogWriterFailureMode.Ignore:
                        return;
                    case FileLogWriterFailureMode.Retry:
                        if (!shouldRetry)
                        {
                            throw;
                        }

                        if (_retryDelay > TimeSpan.Zero)
                        {
                            Thread.Sleep(_retryDelay);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(_failureMode), _failureMode, null);
                }
            }
        }
    }

    private void RollIfNeededByInterval(DateTime timestamp)
    {
        if (_rollingInterval == FileRollingInterval.None)
        {
            return;
        }

        var intervalKey = GetIntervalKey(timestamp, _rollingInterval);
        if (_currentFileLength == 0)
        {
            _currentIntervalKey = intervalKey;
            return;
        }

        if (intervalKey != _currentIntervalKey)
        {
            RollFile(timestamp);
            _currentIntervalKey = intervalKey;
        }
    }

    private void RollIfNeededBySize(int messageByteLength, DateTime timestamp)
    {
        var fileSizeLimitBytes = _fileSizeLimitBytes;
        if (!fileSizeLimitBytes.HasValue || _currentFileLength == 0)
        {
            return;
        }

        if (_currentFileLength + messageByteLength <= fileSizeLimitBytes.Value)
        {
            return;
        }

        RollFile(timestamp);
        _currentIntervalKey = GetIntervalKey(timestamp, _rollingInterval);
    }

    private void RollFile(DateTime timestamp)
    {
        var previousStream = _stream;
        if (previousStream is not null)
        {
            FlushStream(previousStream, _flushToDisk);
            previousStream.Dispose();
        }

        _stream = null;

        try
        {
            if (TryMoveCurrentFileToArchive(timestamp))
            {
                ApplyRetentionPolicy();
            }

            _stream = OpenFileStream(append: false);
            _currentFileLength = 0;
        }
        catch
        {
            _stream = null;
            _stream = OpenFileStream(append: true);
            _currentFileLength = _stream.Length;
            throw;
        }
    }

    private bool TryMoveCurrentFileToArchive(DateTime timestamp)
    {
        var archiveTimestamp = _archiveTimestampMode switch
        {
            FileArchiveTimestampMode.Utc => timestamp.Kind == DateTimeKind.Utc ? timestamp : timestamp.ToUniversalTime(),
            FileArchiveTimestampMode.Local => timestamp.Kind == DateTimeKind.Local ? timestamp : timestamp.ToLocalTime(),
            _ => throw new ArgumentOutOfRangeException(nameof(_archiveTimestampMode), _archiveTimestampMode, null)
        };

        var stamp = archiveTimestamp.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        var sequence = 0L;
        while (true)
        {
            var archivePath = GetArchivePath(stamp, sequence);
            try
            {
                File.Move(_filePath, archivePath, overwrite: false);
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (IOException) when (File.Exists(archivePath))
            {
                // A process created the same archive name concurrently, retry with next suffix.
            }

            sequence++;
        }
    }

    private string GetArchivePath(string stamp, long sequence)
    {
        var suffix = sequence == 0
            ? stamp
            : $"{stamp}.{sequence.ToString(CultureInfo.InvariantCulture)}";
        return Path.Combine(_directoryPath, $"{_archiveFileNamePrefix}.{suffix}{_archiveFileExtension}");
    }

    private void ApplyRetentionPolicy()
    {
        var retainedFileCountLimit = _retainedFileCountLimit;
        if (!retainedFileCountLimit.HasValue)
        {
            return;
        }

        var pattern = $"{_archiveFileNamePrefix}.*{_archiveFileExtension}";
        var files = Directory.GetFiles(_directoryPath, pattern, SearchOption.TopDirectoryOnly);
        if (files.Length <= retainedFileCountLimit.Value)
        {
            return;
        }

        Array.Sort(files, static (left, right) => string.CompareOrdinal(right, left));

        for (var index = retainedFileCountLimit.Value; index < files.Length; index++)
        {
            File.Delete(files[index]);
        }
    }

    private static long GetIntervalKey(DateTime timestamp, FileRollingInterval interval)
    {
        if (interval == FileRollingInterval.None)
        {
            return 0;
        }

        var utcTimestamp = timestamp.Kind == DateTimeKind.Utc ? timestamp : timestamp.ToUniversalTime();
        return interval switch
        {
            FileRollingInterval.Hourly => utcTimestamp.Ticks / TimeSpan.TicksPerHour,
            FileRollingInterval.Daily => utcTimestamp.Ticks / TimeSpan.TicksPerDay,
            _ => throw new ArgumentOutOfRangeException(nameof(interval), interval, null)
        };
    }

    private FileStream OpenFileStream(bool append)
    {
        return new FileStream(
            _filePath,
            append ? FileMode.Append : FileMode.Create,
            FileAccess.Write,
            _fileShare,
            _fileBufferSize,
            _fileOptions);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(FileLogWriter));
        }
    }

    private void EnsureStream()
    {
        if (_stream is not null)
        {
            return;
        }

        _stream = OpenFileStream(append: true);
        _currentFileLength = _stream.Length;
    }

    /// <summary>
    /// Flushes a stream, optionally forcing writes to durable storage.
    /// </summary>
    /// <param name="stream">The stream to flush.</param>
    /// <param name="flushToDisk">Whether to force writes to durable storage.</param>
    protected virtual void FlushStream(FileStream stream, bool flushToDisk)
    {
        if (flushToDisk)
        {
            stream.Flush(flushToDisk: true);
        }
        else
        {
            stream.Flush();
        }
    }
}
