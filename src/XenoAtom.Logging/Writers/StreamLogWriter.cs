// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Text;
using XenoAtom.Logging.Formatters;
using XenoAtom.Logging.Helpers;

namespace XenoAtom.Logging.Writers;

public class StreamLogWriter : LogWriter
{
    public StreamLogWriter(Stream stream) : this(stream, Encoding.UTF8)
    {
    }
    public StreamLogWriter(Stream stream, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(encoding);
        Stream = stream;
        Encoding = encoding;
        Formatter = DefaultLogFormatter.Instance;
    }

    public Encoding Encoding { get; }

    public Stream Stream { get; }

    public LogFormatter Formatter { get; set; }

    protected virtual bool UseSegments => false;
    
    protected sealed override void Log(in LogMessage logMessage)
    {
        using var formatterBuffer = new LogFormatterBuffer();
        var segments = new LogMessageFormatSegments(UseSegments);
        try
        {
            var span = formatterBuffer.Format(logMessage, Formatter, ref segments);
            Write(logMessage.Level, span, in segments);
        }
        finally
        {
            segments.Dispose();
        }
    }

    protected virtual void Write(LogLevel level, ReadOnlySpan<char> text, in LogMessageFormatSegments segments)
    {
        using var encoderBuffer = new LogEncoderBuffer();
        var byteSpan = encoderBuffer.Encode(text, Encoding);
        Stream.Write(byteSpan);
    }
}