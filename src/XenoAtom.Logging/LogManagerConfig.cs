// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Collections;
using System.ComponentModel;

namespace XenoAtom.Logging;

public sealed class LogManagerConfig
{
    internal readonly Dictionary<string, LoggerConfig> LoggerConfigs = new();

    internal Action? ApplyChangesCallback;

    public LogManagerConfig()
    {
        RootLogger = new LoggerConfig("");
        Loggers = new LoggerConfigCollection(this);
    }

    public LogManagerKind Kind { get; init; } = LogManagerKind.Async;
    
    public TimeProvider TimeProvider { get; set; } = TimeProvider.System;
    
    public LoggerConfig RootLogger { get; }

    public LoggerConfigCollection Loggers { get; }

    public LoggerConfig GetLoggerConfig(string loggerName)
    {
        if (string.IsNullOrWhiteSpace(loggerName)) throw new ArgumentNullException(nameof(loggerName));
        if (!LoggerConfigs.TryGetValue(loggerName, out var logger))
        {
            logger = new LoggerConfig(loggerName);
            LoggerConfigs[loggerName] = logger;
        }
        return logger;
    }

    public void RemoveLoggerConfig(string loggerName)
    {
        if (string.IsNullOrWhiteSpace(loggerName)) throw new ArgumentNullException(nameof(loggerName));
        LoggerConfigs.Remove(loggerName);
    }
    
    public void SetLogLevel(string loggerName, LogLevel level)
    {
        if (string.IsNullOrWhiteSpace(loggerName)) throw new ArgumentNullException(nameof(loggerName));
        var loggerConfig = GetLoggerConfig(loggerName);
        loggerConfig.MinimumLevel = level;
    }

    public void ApplyChanges()
    {
        ApplyChangesCallback?.Invoke();
    }
}

/// <summary>
/// A collection of <see cref="LoggerConfig"/>.
/// </summary>
public class LoggerConfigCollection : IEnumerable<LoggerConfig>
{
    private readonly LogManagerConfig _config;

    internal LoggerConfigCollection(LogManagerConfig config)
    {
        _config = config;
    }

    public void Add(string name, LogLevel level)
    {
        _config.SetLogLevel(name, level);
    }

    public void Add(string name, LogLevel level, ReadOnlySpan<LogWriterConfig> writerConfigs, bool includeParents = true)
    {
        var loggerConfig = _config.GetLoggerConfig(name);
        loggerConfig.MinimumLevel = level;
        loggerConfig.IncludeParentWriters = includeParents;
        foreach (var writerConfig in writerConfigs)
        {
            loggerConfig.Writers.Add(writerConfig);
        }
    }

    public void Add(string name, ReadOnlySpan<LogWriterConfig> writerConfigs, bool includeParents = true)
    {
        var loggerConfig = _config.GetLoggerConfig(name);
        loggerConfig.IncludeParentWriters = includeParents;
        foreach (var writerConfig in writerConfigs)
        {
            loggerConfig.Writers.Add(writerConfig);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Dictionary<string, LoggerConfig>.ValueCollection.Enumerator GetEnumerator() => _config.LoggerConfigs.Values.GetEnumerator();

    IEnumerator<LoggerConfig> IEnumerable<LoggerConfig>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
