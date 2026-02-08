// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

/// <summary>
/// Specifies how a <see cref="LogLevel"/> value is displayed in formatted output.
/// </summary>
public enum LogLevelFormat
{
    /// <summary>
    /// Uppercase short name: TRACE, DEBUG, INFO, WARN, ERROR, FATAL.
    /// </summary>
    Short,

    /// <summary>
    /// Full mixed-case name: Trace, Debug, Information, Warning, Error, Fatal.
    /// </summary>
    Long,

    /// <summary>
    /// Three-letter abbreviation: TRC, DBG, INF, WRN, ERR, FTL.
    /// </summary>
    Tri,

    /// <summary>
    /// Single uppercase character: T, D, I, W, E, F.
    /// </summary>
    Char,
}
