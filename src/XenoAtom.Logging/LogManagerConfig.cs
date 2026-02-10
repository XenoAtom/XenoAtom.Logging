// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Collections;
using System.ComponentModel;

namespace XenoAtom.Logging;

/// <summary>
/// Represents the root configuration used to initialize <see cref="LogManager"/>.
/// </summary>
public sealed class LogManagerConfig
{
    internal readonly Dictionary<string, LoggerConfig> LoggerConfigs = new();

    internal Action? ApplyChangesCallback;

    /// <summary>
    /// Initializes a new instance of <see cref="LogManagerConfig"/>.
    /// </summary>
    public LogManagerConfig()
    {
        RootLogger = new LoggerConfig("");
        Loggers = new LoggerConfigCollection(this);
    }

    /// <summary>
    /// Gets or sets the time provider used to timestamp log entries.
    /// </summary>
    public TimeProvider TimeProvider { get; set; } = TimeProvider.System;

    /// <summary>
    /// Gets or sets the target queue capacity for asynchronous processing.
    /// </summary>
    /// <remarks>
    /// This value is used by <see cref="LogMessageAsyncProcessor"/> to decide when overflow policies
    /// (drop, block, allocate) are applied and must be greater than zero.
    /// </remarks>
    public int AsyncLogMessageQueueCapacity { get; set; } = 8192;

    /// <summary>
    /// Gets or sets an optional callback invoked when the asynchronous processor observes a writer/dispatch error.
    /// </summary>
    /// <remarks>
    /// This callback is invoked on the async processor thread and should execute quickly.
    /// </remarks>
    public Action<Exception>? AsyncErrorHandler { get; set; }
    
    /// <summary>
    /// Gets the root logger configuration applied to all categories.
    /// </summary>
    public LoggerConfig RootLogger { get; }

    /// <summary>
    /// Gets or sets the default overflow mode for asynchronous processing.
    /// </summary>
    public LoggerOverflowMode? OverflowMode { get; set; }
    
    /// <summary>
    /// Gets the collection of per-category logger configurations.
    /// </summary>
    public LoggerConfigCollection Loggers { get; }

    /// <summary>
    /// Gets or creates a logger configuration for <paramref name="loggerName"/>.
    /// </summary>
    /// <param name="loggerName">The logger category name.</param>
    /// <returns>The existing or created <see cref="LoggerConfig"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="loggerName"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="loggerName"/> is empty or whitespace.</exception>
    public LoggerConfig GetLoggerConfig(string loggerName)
    {
        ThrowIfNullOrWhiteSpace(loggerName);
        if (!LoggerConfigs.TryGetValue(loggerName, out var logger))
        {
            logger = new LoggerConfig(loggerName);
            LoggerConfigs[loggerName] = logger;
        }
        return logger;
    }

    /// <summary>
    /// Removes a logger configuration.
    /// </summary>
    /// <param name="loggerName">The logger category name.</param>
    /// <exception cref="ArgumentNullException"><paramref name="loggerName"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="loggerName"/> is empty or whitespace.</exception>
    public void RemoveLoggerConfig(string loggerName)
    {
        ThrowIfNullOrWhiteSpace(loggerName);
        LoggerConfigs.Remove(loggerName);
    }
    
    /// <summary>
    /// Sets the minimum level for a logger configuration.
    /// </summary>
    /// <param name="loggerName">The logger category name.</param>
    /// <param name="level">The minimum level to assign.</param>
    /// <exception cref="ArgumentNullException"><paramref name="loggerName"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="loggerName"/> is empty or whitespace.</exception>
    public void SetLogLevel(string loggerName, LogLevel level)
    {
        ThrowIfNullOrWhiteSpace(loggerName);
        var loggerConfig = GetLoggerConfig(loggerName);
        loggerConfig.MinimumLevel = level;
    }

    /// <summary>
    /// Applies pending configuration changes to already-created loggers.
    /// </summary>
    public void ApplyChanges()
    {
        ApplyChangesCallback?.Invoke();
    }

    private static void ThrowIfNullOrWhiteSpace(string? value, [System.Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName);
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be empty or whitespace.", paramName);
        }
    }
}

/// <summary>
/// A collection of <see cref="LoggerConfig"/>.
/// </summary>
public sealed class LoggerConfigCollection : IEnumerable<LoggerConfig>
{
    private readonly LogManagerConfig _config;

    internal LoggerConfigCollection(LogManagerConfig config)
    {
        _config = config;
    }

    /// <summary>
    /// Adds or updates a logger configuration with a minimum level.
    /// </summary>
    /// <param name="name">The logger category name.</param>
    /// <param name="level">The minimum level for the category.</param>
    public void Add(string name, LogLevel level)
    {
        _config.SetLogLevel(name, level);
    }

    /// <summary>
    /// Adds or updates a logger configuration with a minimum level and writers.
    /// </summary>
    /// <param name="name">The logger category name.</param>
    /// <param name="level">The minimum level for the category.</param>
    /// <param name="writerConfigs">The writer configurations to attach.</param>
    /// <param name="includeParents">Whether parent logger writers are inherited.</param>
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

    /// <summary>
    /// Adds or updates a logger configuration with writers.
    /// </summary>
    /// <param name="name">The logger category name.</param>
    /// <param name="writerConfigs">The writer configurations to attach.</param>
    /// <param name="includeParents">Whether parent logger writers are inherited.</param>
    public void Add(string name, ReadOnlySpan<LogWriterConfig> writerConfigs, bool includeParents = true)
    {
        var loggerConfig = _config.GetLoggerConfig(name);
        loggerConfig.IncludeParentWriters = includeParents;
        foreach (var writerConfig in writerConfigs)
        {
            loggerConfig.Writers.Add(writerConfig);
        }
    }

    /// <summary>
    /// Gets an enumerator over configured loggers.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Dictionary<string, LoggerConfig>.ValueCollection.Enumerator GetEnumerator() => _config.LoggerConfigs.Values.GetEnumerator();

    IEnumerator<LoggerConfig> IEnumerable<LoggerConfig>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
