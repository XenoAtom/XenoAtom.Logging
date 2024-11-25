// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Helpers;

internal static class CpuHelper
{
    /// <summary>
    /// Gets the size of the cache line in bytes.
    /// </summary>
    /// <remarks>
    /// Don't bother on macOS to check the exact CPU and assume that it is an M1+ CPU with a cache line of 128 bytes.
    /// Old Intel CPU will bigger cache line, but it is not worth the effort to detect it (although the JIT should be able to do it).
    /// </remarks>
    public static readonly int CacheLineSize  = OperatingSystem.IsMacOS() ? 128 : 64;
}