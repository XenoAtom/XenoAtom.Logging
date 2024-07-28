// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

public sealed class LoggerConfig
{
    internal LoggerConfig(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public LogLevel? MinimumLevel { get; set; }

    public LoggerOverflowMode? OverflowMode { get; set; }

    public bool IncludeParentWriters { get; set; } = true;

    public List<LogWriterConfig> Writers { get; } = new();
}
