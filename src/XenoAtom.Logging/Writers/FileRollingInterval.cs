// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Writers;

/// <summary>
/// Defines the time-based rolling interval for <see cref="FileLogWriter"/>.
/// </summary>
public enum FileRollingInterval
{
    /// <summary>
    /// Disables time-based rolling.
    /// </summary>
    None,

    /// <summary>
    /// Rolls the file when the UTC hour changes.
    /// </summary>
    Hourly,

    /// <summary>
    /// Rolls the file when the UTC day changes.
    /// </summary>
    Daily
}
