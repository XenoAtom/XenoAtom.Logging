// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace XenoAtom.Logging;

/// <summary>
/// Represents a logger bound to a category name.
/// </summary>
/// <remarks>
/// Instances are safe for concurrent log calls after <see cref="LogManager"/> initialization.
/// </remarks>
public sealed partial class Logger
{
    private int _level;
    private ComputedLoggerState? _state;

    internal Logger(string name)
    {
        Name = name;
        _level = (int)LogLevel.None;
    }

    /// <summary>
    /// Gets the logger category name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Determines whether this logger is enabled for the specified <paramref name="level"/>.
    /// </summary>
    /// <param name="level">The log level to evaluate.</param>
    /// <returns><see langword="true"/> when the level is enabled; otherwise <see langword="false"/>.</returns>
    public bool IsEnabled(LogLevel level) => (int)level >= Volatile.Read(ref _level);

    internal LoggerOverflowMode OverflowMode => Volatile.Read(ref _state)?.OverflowMode ?? LoggerOverflowMode.Default;

    /// <summary>
    /// Begins a logging scope that appends <paramref name="properties"/> to subsequent log messages.
    /// </summary>
    /// <param name="properties">The scope properties.</param>
    /// <returns>A scope token that must be disposed to close the scope.</returns>
    public LogBeginScope BeginScope(LogProperties properties)
    {
        var token = LogScopeContext.Push(in properties);
        return new LogBeginScope(token);
    }

    internal void Configure(ComputedLoggerState state)
    {
        Volatile.Write(ref _state, state);
        Volatile.Write(ref _level, (int)state.Level);
    }

    internal LogWriter[] GetLogWriters(LogLevel level) => Volatile.Read(ref _state)?.WritersPerLevel[(int)level] ?? [];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Log(InterpolatedLogMessageInternal message)
    {
        if (Volatile.Read(ref _state) is null) ThrowLogManagerNotInitialized();
        message.Log();
    }

    internal void ResetAfterShutdown()
    {
        Volatile.Write(ref _state, null);
    }

    [DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowLogManagerNotInitialized()
    {
        throw new InvalidOperationException("The LogManager is not initialized. Call LogManager.Initialize(config) first");
    }
}
