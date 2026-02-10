// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

/// <summary>
/// Represents the severity of the log message.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Not used for writing log messages but specifies that a logging category should log all messages.
    /// </summary>
    All,
    /// <summary>
    /// Logs that contain the most detailed messages. These messages may contain sensitive application data.
    /// These messages are disabled by default and should never be enabled in a production environment.
    /// </summary>
    Trace,
    /// <summary>
    /// Logs that are used for interactive investigation during development.  These logs should primarily contain
    /// information useful for debugging and have no long-term value.
    /// </summary>
    Debug,
    /// <summary>
    /// Logs that track the general flow of the application. These logs should have long-term value.
    /// </summary>
    Info,
    /// <summary>
    /// Logs that highlight an abnormal or unexpected event in the application flow, but do not otherwise cause the
    /// application execution to stop.
    /// </summary>
    Warn,
    /// <summary>
    /// Logs that highlight when the current flow of execution is stopped due to a failure. These should indicate a
    /// failure in the current activity, not an application-wide failure.
    /// </summary>
    Error,
    /// <summary>
    /// Logs that describe an unrecoverable application or system crash, or a catastrophic failure that requires
    /// immediate attention.
    /// </summary>
    Fatal,
    /// <summary>
    /// Not used for writing log messages. Specifies that a logging category should not write any messages.
    /// </summary>
    None,
}

/// <summary>
/// Provides conversion helpers for <see cref="LogLevel"/>.
/// </summary>
public static class LogLevelExtensions
{
    /// <summary>
    /// Converts a level to its short uppercase representation.
    /// </summary>
    /// <param name="level">The level to convert.</param>
    /// <returns>The short text representation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The level is not supported.</exception>
    public static string ToShortString(this LogLevel level) => level switch
    {
        LogLevel.Trace => "TRACE",
        LogLevel.Debug => "DEBUG",
        LogLevel.Info => "INFO",
        LogLevel.Warn => "WARN",
        LogLevel.Error => "ERROR",
        LogLevel.Fatal => "FATAL",
        _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
    };

    /// <summary>
    /// Converts a level to its long text representation.
    /// </summary>
    /// <param name="level">The level to convert.</param>
    /// <returns>The long text representation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The level is not supported.</exception>
    public static string ToLongString(this LogLevel level) => level switch
    {
        LogLevel.Trace => "Trace",
        LogLevel.Debug => "Debug",
        LogLevel.Info => "Information",
        LogLevel.Warn => "Warning",
        LogLevel.Error => "Error",
        LogLevel.Fatal => "Fatal",
        _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
    };

    /// <summary>
    /// Converts a level to its three-letter representation.
    /// </summary>
    /// <param name="level">The level to convert.</param>
    /// <returns>The three-letter text representation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The level is not supported.</exception>
    public static string ToTriString(this LogLevel level) => level switch
    {
        LogLevel.Trace => "TRC",
        LogLevel.Debug => "DBG",
        LogLevel.Info => "INF",
        LogLevel.Warn => "WRN",
        LogLevel.Error => "ERR",
        LogLevel.Fatal => "FTL",
        _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
    };

    /// <summary>
    /// Converts a level to its single-character representation.
    /// </summary>
    /// <param name="level">The level to convert.</param>
    /// <returns>The single-character text representation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The level is not supported.</exception>
    public static string ToCharString(this LogLevel level) => level switch
    {
        LogLevel.Trace => "T",
        LogLevel.Debug => "D",
        LogLevel.Info => "I",
        LogLevel.Warn => "W",
        LogLevel.Error => "E",
        LogLevel.Fatal => "F",
        _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
    };
}
