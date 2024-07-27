// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

public readonly ref struct LogMessage
{
    public Logger Logger { get; }

    public LogLevel Level { get; }

    public long SequenceId { get; }

    public DateTime DateTime { get; }

    public LogEventId EventId { get; }

    public Thread Thread { get; }

    public LogScope Scope { get; }

    public ReadOnlySpan<char> Text { get; }

    public LogPropertiesReader Properties { get; }

    public Exception? Exception { get; }

    public IFormatProvider FormatProvider { get; }
}

public readonly ref struct LogPropertiesReader
{

}