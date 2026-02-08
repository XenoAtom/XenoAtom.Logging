// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

/// <summary>
/// Configuration for a <see cref="LogWriter"/>.
/// </summary>
/// <param name="writer">The log writer instance.</param>
public sealed class LogWriterConfig(LogWriter writer)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogWriterConfig"/> class.
    /// </summary>
    /// <param name="writer">The log writer.</param>
    /// <param name="minimumLevel">The minimum level to log.</param>
    public LogWriterConfig(LogWriter writer, LogLevel minimumLevel) : this(writer)
    {
        MinimumLevel = minimumLevel;
    }
    
    /// <summary>
    /// Gets the log writer instance.
    /// </summary>
    public LogWriter Writer { get; } = writer;

    /// <summary>
    /// Gets or sets the level of this writer that can be higher than the level from the <see cref="LogWriter.MinimumLevel"/>.
    /// </summary>
    public LogLevel MinimumLevel { get; set; } = writer.MinimumLevel;

    /// <summary>
    /// Converts an instance of <see cref="LogWriter"/> to an instance of <see cref="LogWriterConfig"/>.
    /// </summary>
    /// <param name="writer">The source writer.</param>
    public static explicit operator LogWriterConfig(LogWriter writer)  => new(writer);
}
