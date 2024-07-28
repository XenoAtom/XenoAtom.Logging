// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

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

    internal LoggerOverflowMode OverflowMode => _state?.OverflowMode ?? LoggerOverflowMode.Default;

    public LogBeginScope BeginScope(LogProperties properties)
    {
        // TODO: make a Roslyn analyzer to check if the properties passed are inlined expressions

        // Copy properties to current global scope


        //LogBufferManager.Current.


        return new LogBeginScope(this);
    }

    internal void Configure(ComputedLoggerState state)
    {
        _state = state;
        _level = state.Level;
    }

    internal LogWriter[] GetLogWriters(LogLevel level) => _state?.WritersPerLevel[(int)level] ?? [];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Log(in InterpolatedLogMessageInternal message)
    {
        if (_state is null) ThrowLogManagerNotInitialized();
        message.Log();
    }

    [DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
    private void ThrowLogManagerNotInitialized()
    {
        throw new InvalidOperationException("The LogManager is not initialized. Call LogManager.Initialize(config) first");
    }
}