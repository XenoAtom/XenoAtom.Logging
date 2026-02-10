// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Buffers;
using XenoAtom.Logging.Formatters;
using XenoAtom.Logging.Helpers;

namespace XenoAtom.Logging.Writers;

/// <summary>
/// Base writer for terminal-oriented sinks that share rich formatting and markup logic.
/// </summary>
public abstract class TerminalLogWriterBase : LogWriter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalLogWriterBase"/> class.
    /// </summary>
    protected TerminalLogWriterBase()
    {
        Formatter = StandardLogFormatter.Instance;
        Styles = new TerminalLogStyleConfiguration();
    }

    /// <summary>
    /// Gets or sets the formatter used to render messages.
    /// </summary>
    public LogFormatter Formatter { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether segment-based rich formatting is enabled.
    /// </summary>
    /// <remarks>
    /// When enabled, formatted segments such as timestamp, level and logger name are emitted as terminal markup
    /// and rendered with styles.
    /// </remarks>
    public bool EnableRichFormatting { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether message payloads marked as markup should be interpreted as markup.
    /// </summary>
    /// <remarks>
    /// Markup payloads are emitted by the <c>*Markup</c> logger extension methods in <c>XenoAtom.Logging.Terminal</c>.
    /// </remarks>
    public bool EnableMarkupMessages { get; set; } = true;

    /// <summary>
    /// Gets the style mapping used for segment-based rich formatting.
    /// </summary>
    /// <remarks>
    /// This object is mutable and can be updated to change terminal styling at runtime.
    /// </remarks>
    public TerminalLogStyleConfiguration Styles { get; }

    /// <summary>
    /// Gets or sets an optional delegate resolving a markup style token for each formatted segment kind.
    /// </summary>
    /// <remarks>
    /// When this delegate returns <see langword="null"/> or an empty value, the writer falls back to <see cref="Styles"/>.
    /// </remarks>
    public Func<LogMessageFormatSegmentKind, LogLevel, string?>? SegmentStyleResolver { get; set; }

    /// <inheritdoc />
    protected sealed override void Log(LogMessage logMessage)
    {
        using var formatterBuffer = new LogFormatterBuffer();
        var shouldCollectSegments = EnableRichFormatting;
        var segments = new LogMessageFormatSegments(shouldCollectSegments);
        try
        {
            var text = formatterBuffer.Format(logMessage, Formatter, ref segments);
            var hasMarkupMessage = EnableMarkupMessages && logMessage.IsMarkup;

            if (!EnableRichFormatting && !hasMarkupMessage)
            {
                AppendLine(text);
                WriteAttachment(logMessage.Attachment);
                return;
            }

            if (!EnableRichFormatting && hasMarkupMessage)
            {
                AppendMarkupLine(text);
                WriteAttachment(logMessage.Attachment);
                return;
            }

            var segmentSpan = segments.AsSpan();
            if (hasMarkupMessage && segmentSpan.Length == 0)
            {
                AppendMarkupLine(text);
                WriteAttachment(logMessage.Attachment);
                return;
            }

            WriteMarkupLine(text, segmentSpan, logMessage.Level, hasMarkupMessage);
            WriteAttachment(logMessage.Attachment);
        }
        finally
        {
            segments.Dispose();
        }
    }

    /// <summary>
    /// Appends a plain text line.
    /// </summary>
    /// <param name="text">The rendered line.</param>
    protected abstract void AppendLine(scoped ReadOnlySpan<char> text);

    /// <summary>
    /// Appends a markup line.
    /// </summary>
    /// <param name="markupText">The markup-rendered line.</param>
    protected abstract void AppendMarkupLine(scoped ReadOnlySpan<char> markupText);

    /// <summary>
    /// Writes an optional attachment associated with the log message.
    /// </summary>
    /// <param name="attachment">The attachment object.</param>
    protected virtual void WriteAttachment(object? attachment)
    {
    }

    private void WriteMarkupLine(
        ReadOnlySpan<char> text,
        ReadOnlySpan<LogMessageFormatSegment> segments,
        LogLevel level,
        bool hasMarkupMessage)
    {
        var buffer = new LogStringBuffer((text.Length + 128) * sizeof(char));
        try
        {
            var position = 0;
            foreach (var segment in segments)
            {
                if (segment.Start > position)
                {
                    AppendEscaped(ref buffer, text[position..segment.Start]);
                }

                var segmentText = text.Slice(segment.Start, segment.Length);
                var styleToken = ResolveSegmentStyle(segment.Kind, level);
                var useStyle = !string.IsNullOrWhiteSpace(styleToken);
                if (useStyle)
                {
                    buffer.Append("[".AsSpan());
                    buffer.Append(styleToken!.AsSpan());
                    buffer.Append("]".AsSpan());
                }

                if (hasMarkupMessage && segment.Kind == LogMessageFormatSegmentKind.Text)
                {
                    buffer.Append(segmentText);
                }
                else
                {
                    AppendEscaped(ref buffer, segmentText);
                }

                if (useStyle)
                {
                    buffer.Append("[/]".AsSpan());
                }

                position = segment.Start + segment.Length;
            }

            if (position < text.Length)
            {
                AppendEscaped(ref buffer, text[position..]);
            }

            AppendMarkupLine(buffer.UnsafeAsSpan());
        }
        finally
        {
            buffer.Dispose();
        }
    }

    private static readonly SearchValues<char> MarkupEscapeChars = SearchValues.Create("[]");

    private static void AppendEscaped(ref LogStringBuffer buffer, ReadOnlySpan<char> text)
    {
        while (!text.IsEmpty)
        {
            var idx = text.IndexOfAny(MarkupEscapeChars);
            if (idx < 0)
            {
                buffer.Append(text);
                return;
            }

            if (idx > 0)
            {
                buffer.Append(text[..idx]);
            }

            buffer.Append(text[idx] == '[' ? "[[".AsSpan() : "]]".AsSpan());
            text = text[(idx + 1)..];
        }
    }

    private string? ResolveSegmentStyle(LogMessageFormatSegmentKind kind, LogLevel level)
    {
        if (SegmentStyleResolver is not null)
        {
            var customStyle = SegmentStyleResolver(kind, level);
            if (!string.IsNullOrWhiteSpace(customStyle))
            {
                return customStyle;
            }
        }

        return Styles.ResolveStyle(kind, level);
    }
}
