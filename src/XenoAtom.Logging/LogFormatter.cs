// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

/// <summary>
/// Base class for formatting a <see cref="LogMessage"/> into a <see cref="Span{T}"/> of characters.
/// </summary>
public abstract record LogFormatter
{
    /// <summary>
    /// The default format used for timestamp rendering.
    /// </summary>
    public const string DefaultTimestampFormat = "yyyy-MM-dd HH:mm:ss.fffffff";

    /// <summary>
    /// The default format used for log level rendering.
    /// </summary>
    public const LogLevelFormat DefaultLevelFormat = LogLevelFormat.Tri;

    /// <summary>
    /// Initializes a new instance of <see cref="LogFormatter"/> with default level and timestamp formats.
    /// </summary>
    protected LogFormatter()
        : this(DefaultLevelFormat, DefaultTimestampFormat)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="LogFormatter"/> with explicit level and timestamp formats.
    /// </summary>
    /// <param name="levelFormat">The level rendering format.</param>
    /// <param name="timestampFormat">The timestamp rendering format string.</param>
    /// <exception cref="ArgumentNullException"><paramref name="timestampFormat"/> is null.</exception>
    protected LogFormatter(LogLevelFormat levelFormat, string timestampFormat)
    {
        ArgumentNullException.ThrowIfNull(timestampFormat);
        LevelFormat = levelFormat;
        TimestampFormat = timestampFormat;
    }

    /// <summary>
    /// Gets the level rendering style used by text formatters.
    /// </summary>
    public LogLevelFormat LevelFormat { get; init; }

    /// <summary>
    /// Gets the timestamp format string used by text formatters.
    /// </summary>
    public string TimestampFormat { get; init; }

    /// <summary>
    /// Formats a <see cref="LogMessage"/> into a <see cref="Span{T}"/> of characters.
    /// </summary>
    /// <param name="logMessage">The log message to format.</param>
    /// <param name="destination">The destination characters span.</param>
    /// <param name="charsWritten">The number of characters written.</param>
    /// <param name="segments">The segments computed during the formatting. (optional)</param>
    /// <returns><c>true</c> if the format was successful; otherwise <c>false</c> if the characters span doesn't have enough space.</returns>
    public abstract bool TryFormat(in LogMessage logMessage, Span<char> destination, out int charsWritten, ref LogMessageFormatSegments segments);
}
