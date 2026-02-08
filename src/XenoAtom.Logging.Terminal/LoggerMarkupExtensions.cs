// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using XenoAtom.Ansi;

namespace XenoAtom.Logging;

/// <summary>
/// Markup-aware logging extension methods for <see cref="Logger"/>.
/// </summary>
/// <remarks>
/// These methods mark the message payload as markup so <see cref="Writers.TerminalLogWriter"/> can render it through
/// <see cref="XenoAtom.Terminal.TerminalInstance.WriteMarkup(string)"/>.
/// </remarks>
public static class LoggerMarkupExtensions
{
    internal const string MarkupPropertyName = "__xenoatom.logging.terminal.markup";

    /// <summary>
    /// Logs a markup message with the specified level.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="level">The log level.</param>
    /// <param name="markupMessage">The markup message to log.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="logger"/> or <paramref name="markupMessage"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="level"/> is outside Trace..Fatal.</exception>
    public static void LogMarkup(this Logger logger, LogLevel level, string markupMessage)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(markupMessage);
        ValidateMarkupLevel(level);
        if (!logger.IsEnabled(level))
        {
            return;
        }

        var properties = CreateMarkupProperties();
        try
        {
            LogMarkupCore(logger, level, properties, markupMessage.AsSpan());
        }
        finally
        {
            properties.Dispose();
        }
    }

    /// <summary>
    /// Logs a markup message with the specified level and structured properties.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="level">The log level.</param>
    /// <param name="properties">The structured properties to include.</param>
    /// <param name="markupMessage">The markup message to log.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="logger"/> or <paramref name="markupMessage"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="level"/> is outside Trace..Fatal.</exception>
    public static void LogMarkup(this Logger logger, LogLevel level, LogProperties properties, string markupMessage)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(markupMessage);
        ValidateMarkupLevel(level);
        if (!logger.IsEnabled(level))
        {
            return;
        }

        var mergedProperties = CreateMarkupProperties(properties);
        try
        {
            LogMarkupCore(logger, level, mergedProperties, markupMessage.AsSpan());
        }
        finally
        {
            mergedProperties.Dispose();
        }
    }

    /// <summary>
    /// Logs a markup message with the specified level using an interpolated markup handler.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="level">The log level.</param>
    /// <param name="markupMessage">The interpolated markup handler.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="logger"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="level"/> is outside Trace..Fatal.</exception>
    public static void LogMarkup(this Logger logger, LogLevel level, ref AnsiMarkupInterpolatedStringHandler markupMessage)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(logger);
            ValidateMarkupLevel(level);
            if (!logger.IsEnabled(level))
            {
                return;
            }

            var properties = CreateMarkupProperties();
            try
            {
                LogMarkupCore(logger, level, properties, markupMessage.WrittenSpan);
            }
            finally
            {
                properties.Dispose();
            }
        }
        finally
        {
            markupMessage.Dispose();
        }
    }

    /// <summary>
    /// Logs a markup message with the specified level and structured properties using an interpolated markup handler.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="level">The log level.</param>
    /// <param name="properties">The structured properties to include.</param>
    /// <param name="markupMessage">The interpolated markup handler.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="logger"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="level"/> is outside Trace..Fatal.</exception>
    public static void LogMarkup(this Logger logger, LogLevel level, LogProperties properties, ref AnsiMarkupInterpolatedStringHandler markupMessage)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(logger);
            ValidateMarkupLevel(level);
            if (!logger.IsEnabled(level))
            {
                return;
            }

            var mergedProperties = CreateMarkupProperties(properties);
            try
            {
                LogMarkupCore(logger, level, mergedProperties, markupMessage.WrittenSpan);
            }
            finally
            {
                mergedProperties.Dispose();
            }
        }
        finally
        {
            markupMessage.Dispose();
        }
    }

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    public static void TraceMarkup(this Logger logger, string markupMessage) => logger.LogMarkup(LogLevel.Trace, markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Trace"/> level and structured properties.
    /// </summary>
    public static void TraceMarkup(this Logger logger, LogProperties properties, string markupMessage) => logger.LogMarkup(LogLevel.Trace, properties, markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Trace"/> level.
    /// </summary>
    public static void TraceMarkup(this Logger logger, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Trace, ref markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Trace"/> level and structured properties.
    /// </summary>
    public static void TraceMarkup(this Logger logger, LogProperties properties, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Trace, properties, ref markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    public static void DebugMarkup(this Logger logger, string markupMessage) => logger.LogMarkup(LogLevel.Debug, markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Debug"/> level and structured properties.
    /// </summary>
    public static void DebugMarkup(this Logger logger, LogProperties properties, string markupMessage) => logger.LogMarkup(LogLevel.Debug, properties, markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Debug"/> level.
    /// </summary>
    public static void DebugMarkup(this Logger logger, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Debug, ref markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Debug"/> level and structured properties.
    /// </summary>
    public static void DebugMarkup(this Logger logger, LogProperties properties, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Debug, properties, ref markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    public static void InfoMarkup(this Logger logger, string markupMessage) => logger.LogMarkup(LogLevel.Info, markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Info"/> level and structured properties.
    /// </summary>
    public static void InfoMarkup(this Logger logger, LogProperties properties, string markupMessage) => logger.LogMarkup(LogLevel.Info, properties, markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Info"/> level.
    /// </summary>
    public static void InfoMarkup(this Logger logger, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Info, ref markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Info"/> level and structured properties.
    /// </summary>
    public static void InfoMarkup(this Logger logger, LogProperties properties, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Info, properties, ref markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    public static void WarnMarkup(this Logger logger, string markupMessage) => logger.LogMarkup(LogLevel.Warn, markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Warn"/> level and structured properties.
    /// </summary>
    public static void WarnMarkup(this Logger logger, LogProperties properties, string markupMessage) => logger.LogMarkup(LogLevel.Warn, properties, markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Warn"/> level.
    /// </summary>
    public static void WarnMarkup(this Logger logger, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Warn, ref markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Warn"/> level and structured properties.
    /// </summary>
    public static void WarnMarkup(this Logger logger, LogProperties properties, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Warn, properties, ref markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    public static void ErrorMarkup(this Logger logger, string markupMessage) => logger.LogMarkup(LogLevel.Error, markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Error"/> level and structured properties.
    /// </summary>
    public static void ErrorMarkup(this Logger logger, LogProperties properties, string markupMessage) => logger.LogMarkup(LogLevel.Error, properties, markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Error"/> level.
    /// </summary>
    public static void ErrorMarkup(this Logger logger, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Error, ref markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Error"/> level and structured properties.
    /// </summary>
    public static void ErrorMarkup(this Logger logger, LogProperties properties, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Error, properties, ref markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    public static void FatalMarkup(this Logger logger, string markupMessage) => logger.LogMarkup(LogLevel.Fatal, markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Fatal"/> level and structured properties.
    /// </summary>
    public static void FatalMarkup(this Logger logger, LogProperties properties, string markupMessage) => logger.LogMarkup(LogLevel.Fatal, properties, markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Fatal"/> level.
    /// </summary>
    public static void FatalMarkup(this Logger logger, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Fatal, ref markupMessage);

    /// <summary>
    /// Logs a markup message with <see cref="LogLevel.Fatal"/> level and structured properties.
    /// </summary>
    public static void FatalMarkup(this Logger logger, LogProperties properties, ref AnsiMarkupInterpolatedStringHandler markupMessage) => logger.LogMarkup(LogLevel.Fatal, properties, ref markupMessage);

    private static void LogMarkupCore(Logger logger, LogLevel level, LogProperties properties, ReadOnlySpan<char> markupMessage)
    {
        switch (level)
        {
            case LogLevel.Trace:
                logger.Trace(properties, $"{markupMessage}");
                break;
            case LogLevel.Debug:
                logger.Debug(properties, $"{markupMessage}");
                break;
            case LogLevel.Info:
                logger.Info(properties, $"{markupMessage}");
                break;
            case LogLevel.Warn:
                logger.Warn(properties, $"{markupMessage}");
                break;
            case LogLevel.Error:
                logger.Error(properties, $"{markupMessage}");
                break;
            case LogLevel.Fatal:
                logger.Fatal(properties, $"{markupMessage}");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, "Only Trace, Debug, Info, Warn, Error and Fatal are supported.");
        }
    }

    private static LogProperties CreateMarkupProperties()
    {
        var properties = new LogProperties();
        properties.Add(MarkupPropertyName, true);
        return properties;
    }

    private static LogProperties CreateMarkupProperties(LogProperties properties)
    {
        var mergedProperties = new LogProperties();
        mergedProperties.AddRange(properties);
        mergedProperties.Add(MarkupPropertyName, true);
        return mergedProperties;
    }

    private static void ValidateMarkupLevel(LogLevel level)
    {
        if (level is not LogLevel.Trace and not LogLevel.Debug and not LogLevel.Info and not LogLevel.Warn and not LogLevel.Error and not LogLevel.Fatal)
        {
            throw new ArgumentOutOfRangeException(nameof(level), level, "Only Trace, Debug, Info, Warn, Error and Fatal are supported.");
        }
    }
}
