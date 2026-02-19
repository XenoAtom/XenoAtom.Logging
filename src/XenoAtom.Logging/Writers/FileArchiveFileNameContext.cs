// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Writers;

/// <summary>
/// Context passed to <see cref="FileLogWriterOptions.ArchiveFileNameFormatter"/> when rolling files.
/// </summary>
/// <param name="BaseFileName">The base file name without extension (for example <c>app</c>).</param>
/// <param name="Extension">The active file extension (for example <c>.log</c>).</param>
/// <param name="Timestamp">The roll timestamp converted according to <see cref="FileLogWriterOptions.ArchiveTimestampMode"/>.</param>
/// <param name="Sequence">The sequence suffix used to disambiguate name collisions. The first attempt uses <c>0</c>.</param>
public readonly record struct FileArchiveFileNameContext(
    string BaseFileName,
    string Extension,
    DateTime Timestamp,
    long Sequence);
