// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace XenoAtom.Logging;

internal class ComputedLoggerState
{
    public ComputedLoggerState(LogLevel level, LoggerOverflowMode overflowMode)
    {
        Level = level;
        OverflowMode = overflowMode;
    }

    public readonly LogLevel Level;

    public readonly LoggerOverflowMode OverflowMode;

    public LogWritersPerLevel WritersPerLevel;

    [InlineArray(Length)]
    public struct LogWritersPerLevel
    {
        public const int Length = (int)LogLevel.None + 1;

        public LogWriter[] Writers;
    }
}