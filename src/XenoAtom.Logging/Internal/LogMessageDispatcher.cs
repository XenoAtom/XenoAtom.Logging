// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

internal static class LogMessageDispatcher
{
    internal static void Dispatch(LogMessageProcessor processor, LogMessageInternal message, ref long sequenceId)
    {
        var currentSequenceId = Interlocked.Increment(ref sequenceId);
        message.SequenceId = currentSequenceId;
        var logMessage = new LogMessage(message);

        var writers = message.Logger.GetLogWriters(message.Level);
        foreach (var writer in writers)
        {
            writer.LogInternal(in logMessage);
        }
    }
}
