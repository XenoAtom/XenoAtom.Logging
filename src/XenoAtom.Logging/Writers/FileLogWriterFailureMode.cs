// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Writers;

/// <summary>
/// Defines how <see cref="FileLogWriter"/> handles write/roll I/O failures.
/// </summary>
public enum FileLogWriterFailureMode
{
    /// <summary>
    /// Rethrows the I/O exception.
    /// </summary>
    Throw,

    /// <summary>
    /// Silently drops the failed write operation.
    /// </summary>
    Ignore,

    /// <summary>
    /// Retries the failed operation, then rethrows if all retries fail.
    /// </summary>
    Retry
}
