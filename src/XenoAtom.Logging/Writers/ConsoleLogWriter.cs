// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using XenoAtom.Logging.Helpers;

namespace XenoAtom.Logging.Writers;

public class ConsoleLogWriter : StreamLogWriter
{
    private readonly bool _isConsoleOutputRedirected;

    public ConsoleLogWriter() : base(Console.OpenStandardOutput(), Console.OutputEncoding)
    {
        _isConsoleOutputRedirected = Console.IsOutputRedirected;
        EnableAnsiColor = !_isConsoleOutputRedirected;
        AnsiStyler = ConsoleAnsiStyles.Default;
    }
    
    public bool EnableAnsiColor { get; set; }

    public ConsoleAnsiStyler AnsiStyler { get; set; }

    protected override bool UseSegments => EnableAnsiColor && !_isConsoleOutputRedirected;
    
    protected override void Write(LogLevel level, ReadOnlySpan<char> text, in LogMessageFormatSegments segments)
    {
        if (segments.IsEnabled)
        {
            const int mediumNumberOfCharPerAnsiEscapeCode = 16;
            // Reserve enough space for the worst case scenario: each segment is a different color
            using var textWithColors = new LogStringBuffer(text.Length + (2 * segments.Count) * mediumNumberOfCharPerAnsiEscapeCode);
            var span = segments.UnsafeAsSpan();
            var regularStyle = AnsiStyler(TextSegmentKind.Text);

            var previousIndex = 0;

            var currentStyle = ReadOnlySpan<char>.Empty;
            foreach (ref readonly var item in span)
            {
                if (previousIndex != item.Start)
                {
                    if (currentStyle != regularStyle)
                    {
                        textWithColors.Append(regularStyle);
                        currentStyle = regularStyle;
                    }

                    textWithColors.Append(text.Slice(previousIndex, item.Start - previousIndex));
                }

                var segmentKind = item.Kind.ToTextSegmentKind(level);
                var segmentStyle = AnsiStyler(segmentKind);

                // When the style is changing, we append the new style
                if (currentStyle != segmentStyle)
                {
                    textWithColors.Append(segmentStyle);
                    currentStyle = segmentStyle;
                }

                textWithColors.Append(text.Slice(item.Start, item.Length));
                previousIndex = item.Start + item.Length;
            }

            if (previousIndex < text.Length)
            {
                textWithColors.Append(regularStyle);
                textWithColors.Append(text.Slice(previousIndex));
            }

            base.Write(level, textWithColors.UnsafeAsSpan(), segments);
        }
        else
        {
            base.Write(level, text, segments);
        }
    }
}