// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Formatters;

/// <summary>
/// Compact format with time, level and message.
/// <c>12:34:56 INF Hello world</c>
/// </summary>
[LogFormatter("{Timestamp:HH:mm:ss} {Level} {Text}")]
public sealed partial record CompactLogFormatter : LogFormatter;
