// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

/// <summary>
/// Processes log messages synchronously on the calling thread.
/// </summary>
public sealed class LogMessageSyncProcessor : LogMessageProcessor, ILogMessageProcessorFactory
{
    private long _sequenceId;

    private LogMessageSyncProcessor(LogManagerConfig config) : base(config)
    {
    }

    static LogMessageProcessor ILogMessageProcessorFactory.Create(LogManagerConfig config) => new LogMessageSyncProcessor(config);

    internal override bool Log(LogMessageInternal message, LoggerOverflowMode overflowMode)
    {
        LogMessageDispatcher.Dispatch(this, message, ref _sequenceId);
        return true;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
    }
}
