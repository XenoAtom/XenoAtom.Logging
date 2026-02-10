// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace XenoAtom.Logging;

/// <summary>
/// Base type for components that process log messages.
/// </summary>
internal abstract class LogMessageProcessor : IDisposable
{
    private protected LogMessageProcessor(LogManagerConfig config)
    {
        Config = config;
    }

    /// <summary>
    /// Gets the active configuration used by this processor.
    /// </summary>
    protected LogManagerConfig Config { get; }

    internal abstract bool Log(LogMessageInternal message, LoggerOverflowMode overflowMode);

    /// <summary>
    /// Releases resources used by this processor.
    /// </summary>
    public abstract void Dispose();

    /// <summary>
    /// Initializes the processor before messages are dispatched.
    /// </summary>
    public abstract void Initialize();
}
