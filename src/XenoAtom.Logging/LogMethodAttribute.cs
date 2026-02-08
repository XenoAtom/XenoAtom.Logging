// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

/// <summary>
/// Declares a logging method that is implemented by the XenoAtom.Logging source generator.
/// </summary>
/// <remarks>
/// The attributed method must be <see langword="static"/> and <see langword="partial"/>, and its
/// first parameter must be a <see cref="Logger"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class LogMethodAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogMethodAttribute"/> class.
    /// </summary>
    /// <param name="level">The log level used when emitting this message.</param>
    /// <param name="message">The compile-time message template.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="message"/> is <see langword="null"/>.</exception>
    public LogMethodAttribute(LogLevel level, string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        Level = level;
        Message = message;
    }

    /// <summary>
    /// Gets the log level used by this generated method.
    /// </summary>
    public LogLevel Level { get; }

    /// <summary>
    /// Gets the message template used by this generated method.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets or sets the event identifier. When set to a non-zero value, an event id is emitted.
    /// </summary>
    public int EventId { get; set; }

    /// <summary>
    /// Gets or sets the event name. When not specified, the method name is used.
    /// </summary>
    public string? EventName { get; set; }
}
