// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

public sealed partial class Logger
{
    private LogLevel _level;
    private ComputedLoggerState? _state;

    internal Logger(string name)
    {
        Name = name;
        _level = LogLevel.None;
    }

    public string Name { get; }

    public bool IsEnabled(LogLevel level) => level >= _level;

    public LogBeginScope BeginScope(LogProperties properties)
    {
        // TODO: make a Roslyn analyzer to check if the properties passed are inlined expressions

        // Copy properties to current global scope


        //LogBufferManager.Current.


        return new LogBeginScope(this);
    }

    internal void Update(ComputedLoggerState state)
    {
        _level = state.Level;
        _state = state;
    }

    internal LogWriter[] GetLogWriters(LogLevel level) => _state?.WritersPerLevel[(int)level] ?? [];

    internal void Log(in InterpolatedLogMessageInternal message)
    {
    }
}