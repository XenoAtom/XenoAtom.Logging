// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

/// <summary>
/// Defines a factory contract used by <see cref="LogManager"/> to create message processors.
/// </summary>
public interface ILogMessageProcessorFactory
{
    /// <summary>
    /// Creates a processor for the specified configuration.
    /// </summary>
    /// <param name="config">The active log manager configuration.</param>
    /// <returns>A new message processor instance.</returns>
    static abstract LogMessageProcessor Create(LogManagerConfig config);
}
