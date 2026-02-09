// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Writers;

/// <summary>
/// Provides style mappings used by <see cref="TerminalLogWriterBase"/> when rendering rich terminal output.
/// </summary>
/// <remarks>
/// Styles are markup style tokens understood by <c>XenoAtom.Terminal</c> (for example <c>bold red</c> or <c>gray</c>).
/// This type is not thread-safe for concurrent mutation and read operations; configure styles during startup.
/// </remarks>
public sealed class TerminalLogStyleConfiguration
{
    private const int SegmentStyleCount = (int)LogMessageFormatSegmentKind.Separator + 1;
    private readonly string?[] _segmentStyles;
    private readonly string?[] _levelStyles;

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalLogStyleConfiguration"/> class with default styles.
    /// </summary>
    public TerminalLogStyleConfiguration()
    {
        _segmentStyles = new string?[SegmentStyleCount];
        _levelStyles = new string?[(int)LogLevel.None + 1];
        ResetToDefaults();
    }

    /// <summary>
    /// Gets the style mapped to a formatted segment kind.
    /// </summary>
    /// <param name="kind">The segment kind.</param>
    /// <returns>The mapped style token, or <see langword="null"/> when unstyled.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="kind"/> is not a valid value.</exception>
    public string? GetStyle(LogMessageFormatSegmentKind kind)
    {
        ValidateSegmentKind(kind);
        return _segmentStyles[(int)kind];
    }

    /// <summary>
    /// Sets the style mapped to a formatted segment kind.
    /// </summary>
    /// <param name="kind">The segment kind.</param>
    /// <param name="style">The style token to apply, or <see langword="null"/> to remove styling.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="kind"/> is not a valid value.</exception>
    public void SetStyle(LogMessageFormatSegmentKind kind, string? style)
    {
        ValidateSegmentKind(kind);
        _segmentStyles[(int)kind] = NormalizeStyle(style);
    }

    /// <summary>
    /// Gets the style mapped to a specific log level for the level segment.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <returns>The mapped style token, or <see langword="null"/> when unstyled.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="level"/> is outside Trace..Fatal.</exception>
    public string? GetLevelStyle(LogLevel level)
    {
        ValidateLevel(level);
        return _levelStyles[(int)level];
    }

    /// <summary>
    /// Sets the style mapped to a specific log level for the level segment.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="style">The style token to apply, or <see langword="null"/> to remove styling.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="level"/> is outside Trace..Fatal.</exception>
    public void SetLevelStyle(LogLevel level, string? style)
    {
        ValidateLevel(level);
        _levelStyles[(int)level] = NormalizeStyle(style);
    }

    /// <summary>
    /// Removes all segment and level styles.
    /// </summary>
    public void Clear()
    {
        Array.Clear(_segmentStyles);
        Array.Clear(_levelStyles);
    }

    /// <summary>
    /// Restores the built-in default styles.
    /// </summary>
    public void ResetToDefaults()
    {
        Clear();

        _segmentStyles[(int)LogMessageFormatSegmentKind.Timestamp] = "gray";
        _segmentStyles[(int)LogMessageFormatSegmentKind.LoggerName] = "blue";
        _segmentStyles[(int)LogMessageFormatSegmentKind.EventId] = "magenta";
        _segmentStyles[(int)LogMessageFormatSegmentKind.Exception] = "bold red";

        _levelStyles[(int)LogLevel.Trace] = "dim";
        _levelStyles[(int)LogLevel.Debug] = "cyan";
        _levelStyles[(int)LogLevel.Info] = "green";
        _levelStyles[(int)LogLevel.Warn] = "bold yellow";
        _levelStyles[(int)LogLevel.Error] = "bold red";
        _levelStyles[(int)LogLevel.Fatal] = "bold white on red";
    }

    internal string? ResolveStyle(LogMessageFormatSegmentKind kind, LogLevel level)
    {
        if ((uint)kind >= SegmentStyleCount)
        {
            return null;
        }

        if (kind == LogMessageFormatSegmentKind.Level &&
            level is >= LogLevel.Trace and <= LogLevel.Fatal)
        {
            var levelStyle = _levelStyles[(int)level];
            if (!string.IsNullOrWhiteSpace(levelStyle))
            {
                return levelStyle;
            }
        }

        return _segmentStyles[(int)kind];
    }

    private static string? NormalizeStyle(string? style)
        => string.IsNullOrWhiteSpace(style) ? null : style;

    private static void ValidateSegmentKind(LogMessageFormatSegmentKind kind)
    {
        if ((uint)kind >= SegmentStyleCount)
        {
            throw new ArgumentOutOfRangeException(nameof(kind), kind, "The segment kind is outside the valid range.");
        }
    }

    private static void ValidateLevel(LogLevel level)
    {
        if (level is < LogLevel.Trace or > LogLevel.Fatal)
        {
            throw new ArgumentOutOfRangeException(nameof(level), level, "Only Trace, Debug, Info, Warn, Error and Fatal are supported.");
        }
    }
}
