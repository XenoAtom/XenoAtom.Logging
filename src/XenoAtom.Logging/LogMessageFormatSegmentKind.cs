// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

/// <summary>
/// Defines the kind of segment in a formatted log message.
/// </summary>
public enum LogMessageFormatSegmentKind
{
    /// <summary>
    /// A timestamp segment.
    /// </summary>
    Timestamp,

    /// <summary>
    /// A logger name segment.
    /// </summary>
    LoggerName,

    /// <summary>
    /// The log level segment.
    /// </summary>
    Level,

    /// <summary>
    /// The event id segment.
    /// </summary>
    EventId,

    /// <summary>
    /// The sequence id segment.
    /// </summary>
    SequenceId,

    /// <summary>
    /// The thread name segment.
    /// </summary>
    ThreadName,

    /// <summary>
    /// The thread id segment.
    /// </summary>
    ThreadId,

    /// <summary>
    /// A text segment.
    /// </summary>
    Text,

    /// <summary>
    /// A secondary text segment.
    /// </summary>
    SecondaryText,

    /// <summary>
    /// The exception segment.
    /// </summary>
    Exception,

    /// <summary>
    /// A scalar segment.
    /// </summary>
    Scalar,

    /// <summary>
    /// A string segment.
    /// </summary>
    String,

    /// <summary>
    /// A boolean segment.
    /// </summary>
    Boolean,

    /// <summary>
    /// A null segment.
    /// </summary>
    Null,

    /// <summary>
    /// A name segment.
    /// </summary>
    Name,

    /// <summary>
    /// A separator segment.
    /// </summary>
    Separator,
}