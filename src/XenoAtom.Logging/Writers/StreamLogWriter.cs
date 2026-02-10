// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Buffers;
using System.Text;
using XenoAtom.Logging.Formatters;
using XenoAtom.Logging.Helpers;

namespace XenoAtom.Logging.Writers;

/// <summary>
/// Writes formatted log messages to a <see cref="Stream"/>.
/// </summary>
/// <remarks>
/// Concurrent writes are synchronized by this writer.
/// </remarks>
public sealed class StreamLogWriter : LogWriter
{
    private readonly object _syncObject = new();
    private readonly bool _ownsStream;
    private bool _disposed;

    /// <summary>
    /// Initializes a writer that uses UTF-8 encoding and does not own the stream.
    /// </summary>
    /// <param name="stream">The destination stream.</param>
    public StreamLogWriter(Stream stream) : this(stream, Encoding.UTF8, ownsStream: false)
    {
    }

    /// <summary>
    /// Initializes a writer that uses UTF-8 encoding.
    /// </summary>
    /// <param name="stream">The destination stream.</param>
    /// <param name="ownsStream"><see langword="true"/> to dispose <paramref name="stream"/> when this writer is disposed; otherwise <see langword="false"/>.</param>
    public StreamLogWriter(Stream stream, bool ownsStream) : this(stream, Encoding.UTF8, ownsStream)
    {
    }

    /// <summary>
    /// Initializes a writer with the specified stream and encoding and does not own the stream.
    /// </summary>
    /// <param name="stream">The destination stream.</param>
    /// <param name="encoding">The text encoding used for output.</param>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> or <paramref name="encoding"/> is null.</exception>
    public StreamLogWriter(Stream stream, Encoding encoding) : this(stream, encoding, ownsStream: false)
    {
    }

    /// <summary>
    /// Initializes a writer with the specified stream and encoding.
    /// </summary>
    /// <param name="stream">The destination stream.</param>
    /// <param name="encoding">The text encoding used for output.</param>
    /// <param name="ownsStream"><see langword="true"/> to dispose <paramref name="stream"/> when this writer is disposed; otherwise <see langword="false"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> or <paramref name="encoding"/> is null.</exception>
    public StreamLogWriter(Stream stream, Encoding encoding, bool ownsStream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(encoding);
        Stream = stream;
        Encoding = encoding;
        _ownsStream = ownsStream;
        Formatter = StandardLogFormatter.Instance;
    }

    /// <summary>
    /// Gets the encoding used to convert formatted text to bytes.
    /// </summary>
    public Encoding Encoding { get; }

    /// <summary>
    /// Gets the destination stream.
    /// </summary>
    public Stream Stream { get; }

    /// <summary>
    /// Gets the formatter used to render log messages.
    /// </summary>
    /// <remarks>
    /// Set this property during object initialization to avoid concurrent mutation while logging.
    /// </remarks>
    public LogFormatter Formatter { get; init; }

    /// <summary>
    /// Gets a value indicating whether segment metadata is requested while formatting.
    /// </summary>
    private bool UseSegments => false;

    /// <summary>
    /// Gets the newline written after each log entry.
    /// </summary>
    public string NewLine { get; init; } = Environment.NewLine;

    /// <summary>
    /// Gets a value indicating whether the underlying stream is flushed after each write.
    /// </summary>
    public bool AutoFlush { get; init; }
    
    /// <inheritdoc />
    protected sealed override void Log(LogMessage logMessage)
    {
        using var formatterBuffer = new LogFormatterBuffer();
        var segments = new LogMessageFormatSegments(UseSegments);
        char[]? strippedBuffer = null;
        try
        {
            var span = formatterBuffer.Format(logMessage, Formatter, ref segments);
            if (logMessage.IsMarkup && span.Length > 0)
            {
                strippedBuffer = ArrayPool<char>.Shared.Rent(MarkupStripper.GetMaxOutputLength(span.Length));
                var charsWritten = MarkupStripper.Strip(span, strippedBuffer);
                span = strippedBuffer.AsSpan(0, charsWritten);
            }

            Write(logMessage.Level, span, in segments);
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

    /// <summary>
    /// Writes formatted text to the destination stream.
    /// </summary>
    /// <param name="level">The log level of the message.</param>
    /// <param name="text">The formatted log text.</param>
    /// <param name="segments">Optional semantic segments for the formatted text.</param>
    private void Write(LogLevel level, ReadOnlySpan<char> text, in LogMessageFormatSegments segments)
    {
        using var encoderBuffer = new LogEncoderBuffer();
        var byteSpan = encoderBuffer.Encode(text, Encoding);
        lock (_syncObject)
        {
            ThrowIfDisposed();
            Stream.Write(byteSpan);
            if (NewLine.Length > 0)
            {
                var newLineBytes = encoderBuffer.Encode(NewLine.AsSpan(), Encoding);
                Stream.Write(newLineBytes);
            }

            if (AutoFlush)
            {
                Stream.Flush();
            }
        }
    }

    /// <inheritdoc />
    public override void Flush()
    {
        lock (_syncObject)
        {
            if (_disposed)
            {
                return;
            }

            Stream.Flush();
        }
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        lock (_syncObject)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            Stream.Flush();
            if (_ownsStream)
            {
                Stream.Dispose();
            }
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(StreamLogWriter));
        }
    }
}
