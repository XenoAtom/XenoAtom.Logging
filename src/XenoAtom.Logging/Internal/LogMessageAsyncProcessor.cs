// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

internal class LogMessageAsyncProcessor : LogMessageProcessor
{
    public LogMessageAsyncProcessor(LogManagerConfig config) : base(config)
    {
    }

    public override void Log(LogMessageHandler message)
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        throw new NotImplementedException();
    }

    public override void Initialize()
    {
        throw new NotImplementedException();
    }
}