// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

/// <summary>
/// Represents runtime diagnostics for the active <see cref="LogManager"/> instance.
/// </summary>
/// <param name="IsInitialized">Indicates whether the log manager is initialized.</param>
/// <param name="ProcessorType">The active processor type when initialized.</param>
/// <param name="IsAsyncProcessor">Indicates whether the active processor is asynchronous.</param>
/// <param name="AsyncQueueLength">The current asynchronous queue length.</param>
/// <param name="AsyncQueueCapacity">The configured asynchronous queue capacity.</param>
/// <param name="DroppedMessages">The number of dropped messages tracked by the async processor.</param>
/// <param name="ErrorCount">The number of async processing errors observed by the async processor.</param>
public readonly record struct LogManagerDiagnostics(
    bool IsInitialized,
    Type? ProcessorType,
    bool IsAsyncProcessor,
    int AsyncQueueLength,
    int AsyncQueueCapacity,
    long DroppedMessages,
    long ErrorCount)
{
    /// <summary>
    /// Gets diagnostics for an uninitialized manager.
    /// </summary>
    public static LogManagerDiagnostics Uninitialized { get; } = new(
        false,
        null,
        false,
        0,
        0,
        0,
        0);
}
