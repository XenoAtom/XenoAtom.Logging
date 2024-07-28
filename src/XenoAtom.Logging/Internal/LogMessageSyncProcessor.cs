// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

internal class LogMessageSyncProcessor : LogMessageProcessor
{
    public LogMessageSyncProcessor(LogManagerConfig config) : base(config)
    {
    }

    public override void Log(LogMessageHandler message)
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