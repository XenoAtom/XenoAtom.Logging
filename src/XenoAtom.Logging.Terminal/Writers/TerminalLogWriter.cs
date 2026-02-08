// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Buffers;
using XenoAtom.Logging.Formatters;
using XenoAtom.Logging.Helpers;
using XenoAtom.Terminal;

namespace XenoAtom.Logging.Writers;

/// <summary>
/// A <see cref="LogWriter"/> that writes formatted messages to <see cref="TerminalInstance"/>.
/// </summary>
public sealed class TerminalLogWriter : LogWriter
{
    private static ReadOnlySpan<char> MarkupPropertyName => LoggerMarkupExtensions.MarkupPropertyName.AsSpan();
    private static ReadOnlySpan<char> MarkupTrueValue => bool.TrueString.AsSpan();

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalLogWriter"/> class using <see cref="XenoAtom.Terminal.Terminal.Instance"/>.
    /// </summary>
    public TerminalLogWriter() : this(XenoAtom.Terminal.Terminal.Instance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalLogWriter"/> class.
    /// </summary>
    /// <param name="terminal">The terminal instance receiving output.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="terminal"/> is <see langword="null"/>.</exception>
    public TerminalLogWriter(TerminalInstance terminal)
    {
        ArgumentNullException.ThrowIfNull(terminal);
        Terminal = terminal;
        Formatter = StandardLogFormatter.Instance;
        Styles = new TerminalLogStyleConfiguration();
    }

    /// <summary>
    /// Gets the target terminal instance.
    /// </summary>
    public TerminalInstance Terminal { get; }

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
    protected override void Log(in LogMessage logMessage)
    {
        using var formatterBuffer = new LogFormatterBuffer();
        var shouldCollectSegments = EnableRichFormatting;
        var segments = new LogMessageFormatSegments(shouldCollectSegments);
        try
        {
            var text = formatterBuffer.Format(logMessage, Formatter, ref segments);

            var hasMarkupMessage = EnableMarkupMessages && ContainsMarkupPayload(logMessage.Properties);
            if (!EnableRichFormatting && !hasMarkupMessage)
            {
                Terminal.Write(text);
                Terminal.WriteLine();
                return;
            }

            if (!EnableRichFormatting && hasMarkupMessage)
            {
                Terminal.WriteMarkup(text);
                Terminal.WriteLine();
                return;
            }

            var segmentSpan = segments.AsSpan();
            if (hasMarkupMessage && segmentSpan.Length == 0)
            {
                Terminal.WriteMarkup(text);
                Terminal.WriteLine();
                return;
            }

            WriteMarkupLine(text, segmentSpan, logMessage.Level, hasMarkupMessage);
            Terminal.WriteLine();
        }
        finally
        {
            segments.Dispose();
        }
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

            Terminal.WriteMarkup(buffer.UnsafeAsSpan());
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

            if (text[idx] == '[')
            {
                buffer.Append("[[".AsSpan());
            }
            else
            {
                buffer.Append("]]".AsSpan());
            }

            text = text[(idx + 1)..];
        }
    }

    private static bool ContainsMarkupPayload(LogPropertiesReader properties)
        => properties.Contains(MarkupPropertyName, MarkupTrueValue);

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
