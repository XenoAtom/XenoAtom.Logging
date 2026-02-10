// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using XenoAtom.Ansi;

namespace XenoAtom.Logging;

/// <summary>
/// Markup-aware logging extension methods for <see cref="Logger"/>.
/// </summary>
public static class LoggerMarkupExtensions
{
    /// <summary>
    /// Logs a markup message with the specified level.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> or <paramref name="markupMessage"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="level"/> is outside Trace..Fatal.</exception>
    public static void LogMarkup(this Logger logger, LogLevel level, string markupMessage)
        => LogMarkup(logger, level, attachment: null, markupMessage);

    /// <summary>
    /// Logs a markup message with the specified level and attachment.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> or <paramref name="markupMessage"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="level"/> is outside Trace..Fatal.</exception>
    public static void LogMarkup(this Logger logger, LogLevel level, object? attachment, string markupMessage)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(markupMessage);
        ValidateMarkupLevel(level);
        if (!logger.IsEnabled(level))
        {
            return;
        }

        LogMarkupCore(logger, level, attachment, markupMessage.AsSpan());
    }

    /// <summary>
    /// Logs a markup message with the specified level and structured properties.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> or <paramref name="markupMessage"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="level"/> is outside Trace..Fatal.</exception>
    public static void LogMarkup(this Logger logger, LogLevel level, LogProperties properties, string markupMessage)
        => LogMarkup(logger, level, properties, attachment: null, markupMessage);

    /// <summary>
    /// Logs a markup message with the specified level, structured properties, and attachment.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> or <paramref name="markupMessage"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="level"/> is outside Trace..Fatal.</exception>
    public static void LogMarkup(this Logger logger, LogLevel level, LogProperties properties, object? attachment, string markupMessage)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(markupMessage);
        ValidateMarkupLevel(level);
        if (!logger.IsEnabled(level))
        {
            return;
        }

        LogMarkupCore(logger, level, properties, attachment, markupMessage.AsSpan());
    }

    /// <summary>
    /// Logs an interpolated markup message with the specified level.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="level"/> is outside Trace..Fatal.</exception>
    public static void LogMarkup(this Logger logger, LogLevel level, ref AnsiMarkupInterpolatedStringHandler markupMessage)
        => LogMarkup(logger, level, attachment: null, ref markupMessage);

    /// <summary>
    /// Logs an interpolated markup message with the specified level and attachment.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="level"/> is outside Trace..Fatal.</exception>
    public static void LogMarkup(this Logger logger, LogLevel level, object? attachment, ref AnsiMarkupInterpolatedStringHandler markupMessage)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(logger);
            ValidateMarkupLevel(level);
            if (!logger.IsEnabled(level))
            {
                return;
            }

            LogMarkupCore(logger, level, attachment, markupMessage.WrittenSpan);
        }
        finally
        {
            markupMessage.Dispose();
        }
    }

    /// <summary>
    /// Logs an interpolated markup message with the specified level and structured properties.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="level"/> is outside Trace..Fatal.</exception>
    public static void LogMarkup(this Logger logger, LogLevel level, LogProperties properties, ref AnsiMarkupInterpolatedStringHandler markupMessage)
        => LogMarkup(logger, level, properties, attachment: null, ref markupMessage);

    /// <summary>
    /// Logs an interpolated markup message with the specified level, structured properties, and attachment.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="level"/> is outside Trace..Fatal.</exception>
    public static void LogMarkup(this Logger logger, LogLevel level, LogProperties properties, object? attachment, ref AnsiMarkupInterpolatedStringHandler markupMessage)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(logger);
            ValidateMarkupLevel(level);
            if (!logger.IsEnabled(level))
            {
                return;
            }

            LogMarkupCore(logger, level, properties, attachment, markupMessage.WrittenSpan);
        }
        finally
        {
            markupMessage.Dispose();
        }
    }

    /// <summary>Logs a markup message at <see cref="LogLevel.Trace"/> level.</summary>
    public static void TraceMarkup(this Logger logger, string markupMessage) => logger.LogMarkup(LogLevel.Trace, markupMessage);
    /// <summary>Logs a markup message with attachment at <see cref="LogLevel.Trace"/> level.</summary>
    public static void TraceMarkup(this Logger logger, object? attachment, string markupMessage) => logger.LogMarkup(LogLevel.Trace, attachment, markupMessage);
    /// <summary>Logs a markup message with structured properties at <see cref="LogLevel.Trace"/> level.</summary>
    public static void TraceMarkup(this Logger logger, LogProperties properties, string markupMessage) => logger.LogMarkup(LogLevel.Trace, properties, markupMessage);
    /// <summary>Logs a markup message with properties and attachment at <see cref="LogLevel.Trace"/> level.</summary>
    public static void TraceMarkup(this Logger logger, LogProperties properties, object? attachment, string markupMessage) => logger.LogMarkup(LogLevel.Trace, properties, attachment, markupMessage);
    /// <summary>Logs an interpolated markup message at <see cref="LogLevel.Trace"/> level.</summary>
    public static void TraceMarkup(this Logger logger, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Trace, ref markupMessage);
    /// <summary>Logs an interpolated markup message with attachment at <see cref="LogLevel.Trace"/> level.</summary>
    public static void TraceMarkup(this Logger logger, object? attachment, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Trace, attachment, ref markupMessage);
    /// <summary>Logs an interpolated markup message with properties at <see cref="LogLevel.Trace"/> level.</summary>
    public static void TraceMarkup(this Logger logger, LogProperties properties, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Trace, properties, ref markupMessage);
    /// <summary>Logs an interpolated markup message with properties and attachment at <see cref="LogLevel.Trace"/> level.</summary>
    public static void TraceMarkup(this Logger logger, LogProperties properties, object? attachment, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Trace, properties, attachment, ref markupMessage);

    /// <summary>Logs a markup message at <see cref="LogLevel.Debug"/> level.</summary>
    public static void DebugMarkup(this Logger logger, string markupMessage) => logger.LogMarkup(LogLevel.Debug, markupMessage);
    /// <summary>Logs a markup message with attachment at <see cref="LogLevel.Debug"/> level.</summary>
    public static void DebugMarkup(this Logger logger, object? attachment, string markupMessage) => logger.LogMarkup(LogLevel.Debug, attachment, markupMessage);
    /// <summary>Logs a markup message with structured properties at <see cref="LogLevel.Debug"/> level.</summary>
    public static void DebugMarkup(this Logger logger, LogProperties properties, string markupMessage) => logger.LogMarkup(LogLevel.Debug, properties, markupMessage);
    /// <summary>Logs a markup message with properties and attachment at <see cref="LogLevel.Debug"/> level.</summary>
    public static void DebugMarkup(this Logger logger, LogProperties properties, object? attachment, string markupMessage) => logger.LogMarkup(LogLevel.Debug, properties, attachment, markupMessage);
    /// <summary>Logs an interpolated markup message at <see cref="LogLevel.Debug"/> level.</summary>
    public static void DebugMarkup(this Logger logger, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Debug, ref markupMessage);
    /// <summary>Logs an interpolated markup message with attachment at <see cref="LogLevel.Debug"/> level.</summary>
    public static void DebugMarkup(this Logger logger, object? attachment, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Debug, attachment, ref markupMessage);
    /// <summary>Logs an interpolated markup message with properties at <see cref="LogLevel.Debug"/> level.</summary>
    public static void DebugMarkup(this Logger logger, LogProperties properties, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Debug, properties, ref markupMessage);
    /// <summary>Logs an interpolated markup message with properties and attachment at <see cref="LogLevel.Debug"/> level.</summary>
    public static void DebugMarkup(this Logger logger, LogProperties properties, object? attachment, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Debug, properties, attachment, ref markupMessage);

    /// <summary>Logs a markup message at <see cref="LogLevel.Info"/> level.</summary>
    public static void InfoMarkup(this Logger logger, string markupMessage) => logger.LogMarkup(LogLevel.Info, markupMessage);
    /// <summary>Logs a markup message with attachment at <see cref="LogLevel.Info"/> level.</summary>
    public static void InfoMarkup(this Logger logger, object? attachment, string markupMessage) => logger.LogMarkup(LogLevel.Info, attachment, markupMessage);
    /// <summary>Logs a markup message with structured properties at <see cref="LogLevel.Info"/> level.</summary>
    public static void InfoMarkup(this Logger logger, LogProperties properties, string markupMessage) => logger.LogMarkup(LogLevel.Info, properties, markupMessage);
    /// <summary>Logs a markup message with properties and attachment at <see cref="LogLevel.Info"/> level.</summary>
    public static void InfoMarkup(this Logger logger, LogProperties properties, object? attachment, string markupMessage) => logger.LogMarkup(LogLevel.Info, properties, attachment, markupMessage);
    /// <summary>Logs an interpolated markup message at <see cref="LogLevel.Info"/> level.</summary>
    public static void InfoMarkup(this Logger logger, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Info, ref markupMessage);
    /// <summary>Logs an interpolated markup message with attachment at <see cref="LogLevel.Info"/> level.</summary>
    public static void InfoMarkup(this Logger logger, object? attachment, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Info, attachment, ref markupMessage);
    /// <summary>Logs an interpolated markup message with properties at <see cref="LogLevel.Info"/> level.</summary>
    public static void InfoMarkup(this Logger logger, LogProperties properties, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Info, properties, ref markupMessage);
    /// <summary>Logs an interpolated markup message with properties and attachment at <see cref="LogLevel.Info"/> level.</summary>
    public static void InfoMarkup(this Logger logger, LogProperties properties, object? attachment, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Info, properties, attachment, ref markupMessage);

    /// <summary>Logs a markup message at <see cref="LogLevel.Warn"/> level.</summary>
    public static void WarnMarkup(this Logger logger, string markupMessage) => logger.LogMarkup(LogLevel.Warn, markupMessage);
    /// <summary>Logs a markup message with attachment at <see cref="LogLevel.Warn"/> level.</summary>
    public static void WarnMarkup(this Logger logger, object? attachment, string markupMessage) => logger.LogMarkup(LogLevel.Warn, attachment, markupMessage);
    /// <summary>Logs a markup message with structured properties at <see cref="LogLevel.Warn"/> level.</summary>
    public static void WarnMarkup(this Logger logger, LogProperties properties, string markupMessage) => logger.LogMarkup(LogLevel.Warn, properties, markupMessage);
    /// <summary>Logs a markup message with properties and attachment at <see cref="LogLevel.Warn"/> level.</summary>
    public static void WarnMarkup(this Logger logger, LogProperties properties, object? attachment, string markupMessage) => logger.LogMarkup(LogLevel.Warn, properties, attachment, markupMessage);
    /// <summary>Logs an interpolated markup message at <see cref="LogLevel.Warn"/> level.</summary>
    public static void WarnMarkup(this Logger logger, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Warn, ref markupMessage);
    /// <summary>Logs an interpolated markup message with attachment at <see cref="LogLevel.Warn"/> level.</summary>
    public static void WarnMarkup(this Logger logger, object? attachment, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Warn, attachment, ref markupMessage);
    /// <summary>Logs an interpolated markup message with properties at <see cref="LogLevel.Warn"/> level.</summary>
    public static void WarnMarkup(this Logger logger, LogProperties properties, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Warn, properties, ref markupMessage);
    /// <summary>Logs an interpolated markup message with properties and attachment at <see cref="LogLevel.Warn"/> level.</summary>
    public static void WarnMarkup(this Logger logger, LogProperties properties, object? attachment, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Warn, properties, attachment, ref markupMessage);

    /// <summary>Logs a markup message at <see cref="LogLevel.Error"/> level.</summary>
    public static void ErrorMarkup(this Logger logger, string markupMessage) => logger.LogMarkup(LogLevel.Error, markupMessage);
    /// <summary>Logs a markup message with attachment at <see cref="LogLevel.Error"/> level.</summary>
    public static void ErrorMarkup(this Logger logger, object? attachment, string markupMessage) => logger.LogMarkup(LogLevel.Error, attachment, markupMessage);
    /// <summary>Logs a markup message with structured properties at <see cref="LogLevel.Error"/> level.</summary>
    public static void ErrorMarkup(this Logger logger, LogProperties properties, string markupMessage) => logger.LogMarkup(LogLevel.Error, properties, markupMessage);
    /// <summary>Logs a markup message with properties and attachment at <see cref="LogLevel.Error"/> level.</summary>
    public static void ErrorMarkup(this Logger logger, LogProperties properties, object? attachment, string markupMessage) => logger.LogMarkup(LogLevel.Error, properties, attachment, markupMessage);
    /// <summary>Logs an interpolated markup message at <see cref="LogLevel.Error"/> level.</summary>
    public static void ErrorMarkup(this Logger logger, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Error, ref markupMessage);
    /// <summary>Logs an interpolated markup message with attachment at <see cref="LogLevel.Error"/> level.</summary>
    public static void ErrorMarkup(this Logger logger, object? attachment, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Error, attachment, ref markupMessage);
    /// <summary>Logs an interpolated markup message with properties at <see cref="LogLevel.Error"/> level.</summary>
    public static void ErrorMarkup(this Logger logger, LogProperties properties, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Error, properties, ref markupMessage);
    /// <summary>Logs an interpolated markup message with properties and attachment at <see cref="LogLevel.Error"/> level.</summary>
    public static void ErrorMarkup(this Logger logger, LogProperties properties, object? attachment, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Error, properties, attachment, ref markupMessage);

    /// <summary>Logs a markup message at <see cref="LogLevel.Fatal"/> level.</summary>
    public static void FatalMarkup(this Logger logger, string markupMessage) => logger.LogMarkup(LogLevel.Fatal, markupMessage);
    /// <summary>Logs a markup message with attachment at <see cref="LogLevel.Fatal"/> level.</summary>
    public static void FatalMarkup(this Logger logger, object? attachment, string markupMessage) => logger.LogMarkup(LogLevel.Fatal, attachment, markupMessage);
    /// <summary>Logs a markup message with structured properties at <see cref="LogLevel.Fatal"/> level.</summary>
    public static void FatalMarkup(this Logger logger, LogProperties properties, string markupMessage) => logger.LogMarkup(LogLevel.Fatal, properties, markupMessage);
    /// <summary>Logs a markup message with properties and attachment at <see cref="LogLevel.Fatal"/> level.</summary>
    public static void FatalMarkup(this Logger logger, LogProperties properties, object? attachment, string markupMessage) => logger.LogMarkup(LogLevel.Fatal, properties, attachment, markupMessage);
    /// <summary>Logs an interpolated markup message at <see cref="LogLevel.Fatal"/> level.</summary>
    public static void FatalMarkup(this Logger logger, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Fatal, ref markupMessage);
    /// <summary>Logs an interpolated markup message with attachment at <see cref="LogLevel.Fatal"/> level.</summary>
    public static void FatalMarkup(this Logger logger, object? attachment, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Fatal, attachment, ref markupMessage);
    /// <summary>Logs an interpolated markup message with properties at <see cref="LogLevel.Fatal"/> level.</summary>
    public static void FatalMarkup(this Logger logger, LogProperties properties, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Fatal, properties, ref markupMessage);
    /// <summary>Logs an interpolated markup message with properties and attachment at <see cref="LogLevel.Fatal"/> level.</summary>
    public static void FatalMarkup(this Logger logger, LogProperties properties, object? attachment, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Fatal, properties, attachment, ref markupMessage);

    private static void LogMarkupCore(Logger logger, LogLevel level, object? attachment, ReadOnlySpan<char> markupMessage)
    {
        logger.Log(new Logger.InterpolatedLogMessageInternal(
            logger,
            level,
            LogEventId.Empty,
            LogPropertiesSnapshot.Empty,
            attachment,
            isMarkup: true,
            markupMessage));
    }

    private static void LogMarkupCore(Logger logger, LogLevel level, LogProperties properties, object? attachment, ReadOnlySpan<char> markupMessage)
    {
        logger.Log(new Logger.InterpolatedLogMessageInternal(
            logger,
            level,
            LogEventId.Empty,
            properties.Snapshot(),
            attachment,
            isMarkup: true,
            markupMessage));
    }

    private static void ValidateMarkupLevel(LogLevel level)
    {
        if (level is not LogLevel.Trace and not LogLevel.Debug and not LogLevel.Info and not LogLevel.Warn and not LogLevel.Error and not LogLevel.Fatal)
        {
            throw new ArgumentOutOfRangeException(nameof(level), level, "Only Trace, Debug, Info, Warn, Error and Fatal are supported.");
        }
    }
}
