// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace XenoAtom.Logging;

public abstract class LogMessageProcessor : IDisposable
{
    private protected LogMessageProcessor(LogManagerConfig config)
    {
        // TODO: Fetch configuration from LogManagerConfig
        BufferPool = new LogBufferPool(1024, 4096);
    }

    internal LogBufferPool BufferPool;

    internal abstract void Log(LogMessageHandle message);

    public abstract void Dispose();

    public abstract void Initialize();
}