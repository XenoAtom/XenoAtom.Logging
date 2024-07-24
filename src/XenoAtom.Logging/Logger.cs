// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace XenoAtom.Logging;

public sealed partial class Logger
{
    //private readonly int _index;

    //[Obsolete("This constructor cannot be used directly. Use LogManager.GetLogger() instead", error: true)]
    //public Logger()
    //{
    //}
    
    internal Logger()
    {
    }

    public LogLevel Level { get; set; }

    public bool IsEnabled(LogLevel level) => Level <= level;

    internal void Log(in InterpolatedLogMessageInternal message)
    {
    }
}