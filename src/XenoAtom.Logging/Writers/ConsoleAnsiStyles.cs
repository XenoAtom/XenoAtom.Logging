// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Writers;

public static class ConsoleAnsiStyles
{
    public static ConsoleAnsiStyler Default => static kind => GetAnsiEscapeCodeStyle(kind);
    
    private static ReadOnlySpan<char> GetAnsiEscapeCodeStyle(TextSegmentKind kind)
    {
        return kind switch
        {
            TextSegmentKind.Timestamp => ConsoleAnsiColors.Grey8,
            TextSegmentKind.LoggerName => ConsoleAnsiColors.Grey8,
            TextSegmentKind.LevelTrace => ConsoleAnsiColors.Grey8,
            TextSegmentKind.LevelDebug => ConsoleAnsiColors.Grey8,
            TextSegmentKind.LevelInfo => ConsoleAnsiColors.Green,
            TextSegmentKind.LevelWarn => ConsoleAnsiColors.Olive,
            TextSegmentKind.LevelError => ConsoleAnsiColors.Maroon,
            TextSegmentKind.LevelFatal => ConsoleAnsiColors.Maroon,
            TextSegmentKind.EventId => ConsoleAnsiColors.Grey8,
            TextSegmentKind.SequenceId => ConsoleAnsiColors.Grey8,
            TextSegmentKind.ThreadName => ConsoleAnsiColors.Grey8,
            TextSegmentKind.ThreadId => ConsoleAnsiColors.Grey8,
            TextSegmentKind.Text => ConsoleAnsiColors.White,
            TextSegmentKind.SecondaryText => ConsoleAnsiColors.Color105,
            TextSegmentKind.Exception => ConsoleAnsiColors.Color160,
            TextSegmentKind.Scalar => ConsoleAnsiColors.Grey8,
            TextSegmentKind.String => ConsoleAnsiColors.Grey8,
            TextSegmentKind.Boolean => ConsoleAnsiColors.Grey8,
            TextSegmentKind.Null => ConsoleAnsiColors.Grey8,
            TextSegmentKind.Name => ConsoleAnsiColors.Grey8,
            TextSegmentKind.Separator => ConsoleAnsiColors.Grey8,
            _ => ConsoleAnsiColors.Grey8,
        };
    }
}