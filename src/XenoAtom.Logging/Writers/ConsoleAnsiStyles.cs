// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Writers;

public static class ConsoleAnsiStyles
{
    public static ConsoleAnsiStyler Default => static kind => GetAnsiEscapeCodeStyle(kind);
    
    private static ReadOnlySpan<char> GetAnsiEscapeCodeStyle(ConsoleSegmentKind kind)
    {
        return kind switch
        {
            ConsoleSegmentKind.Timestamp => ConsoleAnsiColors.Grey8,
            ConsoleSegmentKind.LoggerName => ConsoleAnsiColors.Grey8,
            ConsoleSegmentKind.LevelTrace => ConsoleAnsiColors.Grey8,
            ConsoleSegmentKind.LevelDebug => ConsoleAnsiColors.Grey8,
            ConsoleSegmentKind.LevelInfo => ConsoleAnsiColors.Green,
            ConsoleSegmentKind.LevelWarn => ConsoleAnsiColors.Olive,
            ConsoleSegmentKind.LevelError => ConsoleAnsiColors.Maroon,
            ConsoleSegmentKind.LevelFatal => ConsoleAnsiColors.Maroon,
            ConsoleSegmentKind.EventId => ConsoleAnsiColors.Grey8,
            ConsoleSegmentKind.SequenceId => ConsoleAnsiColors.Grey8,
            ConsoleSegmentKind.ThreadName => ConsoleAnsiColors.Grey8,
            ConsoleSegmentKind.ThreadId => ConsoleAnsiColors.Grey8,
            ConsoleSegmentKind.Text => ConsoleAnsiColors.White,
            ConsoleSegmentKind.SecondaryText => ConsoleAnsiColors.Color105,
            ConsoleSegmentKind.Exception => ConsoleAnsiColors.Color160,
            ConsoleSegmentKind.Scalar => ConsoleAnsiColors.Grey8,
            ConsoleSegmentKind.String => ConsoleAnsiColors.Grey8,
            ConsoleSegmentKind.Boolean => ConsoleAnsiColors.Grey8,
            ConsoleSegmentKind.Null => ConsoleAnsiColors.Grey8,
            ConsoleSegmentKind.Name => ConsoleAnsiColors.Grey8,
            ConsoleSegmentKind.Separator => ConsoleAnsiColors.Grey8,
            _ => ConsoleAnsiColors.Grey8,
        };
    }
}