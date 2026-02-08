// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Writers;

/// <summary>
/// Defines which clock is used to build archive file timestamps.
/// </summary>
public enum FileArchiveTimestampMode
{
    /// <summary>
    /// Uses UTC timestamps for archive file names.
    /// </summary>
    Utc,

    /// <summary>
    /// Uses local timestamps for archive file names.
    /// </summary>
    Local
}
