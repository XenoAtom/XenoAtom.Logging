// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Formatters;

public sealed class DefaultLogFormatter : LogFormatter
{
    private DefaultLogFormatter()
    {
    }

    public static DefaultLogFormatter Instance { get; } = new();

    // TODO: Use a source generator instead
    public override bool TryFormat(in LogMessage logMessage, Span<char> destination, out int charsWritten, ref LogMessageFormatSegments segments)
    {
        charsWritten = 0;
        if (!logMessage.DateTime.TryFormat(destination, out int tempCharsWritten, "yyyy-MM-dd hh\\:mm\\:ss\\.fffffff"))
        {
            return false;
        }

        destination = destination.Slice(tempCharsWritten);
        if (destination.Length < 2) return false;

        destination[0] = ' ';
        tempCharsWritten++;
        destination = destination.Slice(1);
        
        
        var levelStr = logMessage.Level.ToShortString();
        if (levelStr.Length >= destination.Length)
        {
            return false;
        }

        levelStr.AsSpan().CopyTo(destination);
        tempCharsWritten += levelStr.Length;

        destination = destination.Slice(levelStr.Length);
        if (destination.Length < 2) return false;

        destination[0] = ' ';
        tempCharsWritten++;
        destination = destination.Slice(1);

        var loggerName = logMessage.Logger.Name;
        if (loggerName.Length >= destination.Length)
        {
            return false;
        }

        loggerName.AsSpan().CopyTo(destination);
        tempCharsWritten += loggerName.Length;

        destination = destination.Slice(loggerName.Length);
        if (destination.Length < 2) return false;

        destination[0] = ' ';
        tempCharsWritten++;
        destination = destination.Slice(1);

        var message = logMessage.Text;
        if (message.Length >= destination.Length)
        {
            return false;
        }

        message.CopyTo(destination);
        tempCharsWritten += message.Length;
        charsWritten = tempCharsWritten;

        return true;
    }
}