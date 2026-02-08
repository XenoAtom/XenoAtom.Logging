// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

/// <summary>
/// Defines the behavior when the asynchronous queue reaches capacity.
/// </summary>
public enum LoggerOverflowMode
{
    /// <summary>
    /// Drops the message and increments the dropped-message counter.
    /// </summary>
    DropAndNotify,

    /// <summary>
    /// Silently drops the message.
    /// </summary>
    Drop,

    /// <summary>
    /// Blocks the producer thread until capacity is available.
    /// </summary>
    Block,

    /// <summary>
    /// Allows queue growth beyond capacity.
    /// </summary>
    Allocate,

    /// <summary>
    /// Uses the default overflow mode.
    /// </summary>
    Default = DropAndNotify
}
