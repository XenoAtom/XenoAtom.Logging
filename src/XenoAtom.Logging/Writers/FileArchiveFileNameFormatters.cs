// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Globalization;

namespace XenoAtom.Logging.Writers;

/// <summary>
/// Built-in helpers for <see cref="FileLogWriterOptions.ArchiveFileNameFormatter"/>.
/// </summary>
public static class FileArchiveFileNameFormatters
{
    /// <summary>
    /// Produces the default compact file name format:
    /// <c>&lt;base&gt;.&lt;yyyyMMddHHmmssfff&gt;[.&lt;sequence&gt;]&lt;extension&gt;</c>.
    /// </summary>
    /// <param name="context">The archive naming context.</param>
    /// <returns>The archive file name.</returns>
    public static string Compact(FileArchiveFileNameContext context)
        => Format(context, "yyyyMMddHHmmssfff");

    /// <summary>
    /// Produces a human-readable file name format:
    /// <c>&lt;base&gt;.&lt;yyyy-MM-dd-HH_mm_ss&gt;[.&lt;sequence&gt;]&lt;extension&gt;</c>.
    /// </summary>
    /// <param name="context">The archive naming context.</param>
    /// <returns>The archive file name.</returns>
    public static string DateTime(FileArchiveFileNameContext context)
        => Format(context, "yyyy-MM-dd-HH_mm_ss");

    /// <summary>
    /// Produces a human-readable file name format with milliseconds:
    /// <c>&lt;base&gt;.&lt;yyyy-MM-dd-HH_mm_ss_fff&gt;[.&lt;sequence&gt;]&lt;extension&gt;</c>.
    /// </summary>
    /// <param name="context">The archive naming context.</param>
    /// <returns>The archive file name.</returns>
    public static string DateTimeWithMilliseconds(FileArchiveFileNameContext context)
        => Format(context, "yyyy-MM-dd-HH_mm_ss_fff");

    private static string Format(in FileArchiveFileNameContext context, string timestampFormat)
    {
        var stamp = context.Timestamp.ToString(timestampFormat, CultureInfo.InvariantCulture);
        if (context.Sequence == 0)
        {
            return $"{context.BaseFileName}.{stamp}{context.Extension}";
        }

        return $"{context.BaseFileName}.{stamp}.{context.Sequence.ToString(CultureInfo.InvariantCulture)}{context.Extension}";
    }
}
