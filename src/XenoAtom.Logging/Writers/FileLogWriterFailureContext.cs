// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Writers;

/// <summary>
/// Provides contextual information for a failed file write operation.
/// </summary>
/// <param name="FilePath">The target file path.</param>
/// <param name="Attempt">The attempt number (1-based).</param>
/// <param name="Exception">The exception raised by the failed operation.</param>
/// <param name="WillRetry">A value indicating whether the writer will retry.</param>
public readonly record struct FileLogWriterFailureContext(string FilePath, int Attempt, Exception Exception, bool WillRetry);
