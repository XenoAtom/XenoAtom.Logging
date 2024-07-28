// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

internal abstract class LogMessageProcessor : IDisposable
{
    protected LogMessageProcessor(LogManagerConfig config)
    {
    }

    public abstract void Log(LogMessageHandler message);

    public abstract void Dispose();

    public abstract void Initialize();
}