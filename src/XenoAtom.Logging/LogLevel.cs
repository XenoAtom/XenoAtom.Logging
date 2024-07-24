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