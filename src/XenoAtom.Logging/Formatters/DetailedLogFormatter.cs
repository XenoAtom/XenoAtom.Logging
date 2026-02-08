// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Formatters;

/// <summary>
/// Detailed format including thread id and sequence id.
/// <c>2026-02-09 12:34:56.1234567 INFO  [14] #000042 MyApp.Service [42:Connect] Hello world | System.Exception: ...</c>
/// </summary>
[LogFormatter("{Timestamp:yyyy-MM-dd HH:mm:ss.fffffff} {Level,-5} [{Thread}] #{SequenceId:D6} {LoggerName}{? [{EventId}]?} {Text}{? | {Exception}?}")]
public sealed partial record DetailedLogFormatter : LogFormatter;
