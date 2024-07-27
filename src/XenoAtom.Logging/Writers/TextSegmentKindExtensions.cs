// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Writers;

/// <summary>
/// Extensions methods for <see cref="LogMessageFormatSegmentKind"/> with <see cref="TextSegmentKind"/>.
/// </summary>
public static class TextSegmentKindExtensions
{
    /// <summary>
    /// Converts a <see cref="LogMessageFormatSegmentKind"/> to a <see cref="TextSegmentKind"/>.
    /// </summary>
    /// <param name="segmentKind">The format segment kind.</param>
    /// <param name="level">The current level.</param>
    /// <returns>The converted segment kind.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If any of the enums are out of range.</exception>
    public static TextSegmentKind ToTextSegmentKind(this LogMessageFormatSegmentKind segmentKind, LogLevel level)
    {
        return segmentKind switch
        {
            LogMessageFormatSegmentKind.LoggerName => TextSegmentKind.LoggerName,
            LogMessageFormatSegmentKind.Timestamp => TextSegmentKind.Timestamp,
            LogMessageFormatSegmentKind.SequenceId => TextSegmentKind.SequenceId,
            LogMessageFormatSegmentKind.ThreadName => TextSegmentKind.ThreadName,
            LogMessageFormatSegmentKind.ThreadId => TextSegmentKind.ThreadId,
            LogMessageFormatSegmentKind.Text => TextSegmentKind.Text,
            LogMessageFormatSegmentKind.SecondaryText => TextSegmentKind.SecondaryText,
            LogMessageFormatSegmentKind.Exception => TextSegmentKind.Exception,
            LogMessageFormatSegmentKind.Scalar => TextSegmentKind.Scalar,
            LogMessageFormatSegmentKind.String => TextSegmentKind.String,
            LogMessageFormatSegmentKind.Boolean => TextSegmentKind.Boolean,
            LogMessageFormatSegmentKind.Null => TextSegmentKind.Null,
            LogMessageFormatSegmentKind.Name => TextSegmentKind.Name,
            LogMessageFormatSegmentKind.Separator => TextSegmentKind.Separator,
            LogMessageFormatSegmentKind.Level => level switch
            {
                LogLevel.Trace => TextSegmentKind.LevelTrace,
                LogLevel.Debug => TextSegmentKind.LevelDebug,
                LogLevel.Info => TextSegmentKind.LevelInfo,
                LogLevel.Warn => TextSegmentKind.LevelWarn,
                LogLevel.Error => TextSegmentKind.LevelError,
                LogLevel.Fatal => TextSegmentKind.LevelFatal,
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            },
            LogMessageFormatSegmentKind.EventId => TextSegmentKind.EventId,
            _ => throw new ArgumentOutOfRangeException(nameof(segmentKind), segmentKind, null)
        };
    }
}