// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

public sealed class LogMessageSyncProcessor : LogMessageProcessor, ILogMessageProcessorFactory
{
    private LogMessageSyncProcessor(LogManagerConfig config) : base(config)
    {
    }

    static LogMessageProcessor ILogMessageProcessorFactory.Create(LogManagerConfig config) => new LogMessageSyncProcessor(config);

    internal override void Log(LogMessageHandle message)
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
    }

    public override void Initialize()
    {
    }
}