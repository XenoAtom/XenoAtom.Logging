// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace XenoAtom.Logging;

/// <summary>
/// Manages logger instances and global logging configuration.
/// </summary>
/// <remarks>
/// Static operations are thread-safe. Configure <see cref="LogManagerConfig"/> on a single thread before calling
/// <see cref="Initialize(LogManagerConfig)"/> or <see cref="Initialize{TMessageProcessorFactory}(LogManagerConfig)"/>.
/// </remarks>
public sealed class LogManager
{
    private static readonly ConcurrentDictionary<string, Logger> Loggers = new();
    private static LogManager? _instance;
    private readonly LogManagerConfig _config;
    private readonly LogMessageProcessor _processor;
    private long _configVersion;

    internal static TimeProvider TimeProvider => _instance?._config.TimeProvider ?? TimeProvider.System;

    internal static LogMessageProcessor? Processor => _instance?._processor;

    /// <summary>
    /// Gets a value indicating whether the log manager is initialized.
    /// </summary>
    public static bool IsInitialized => Volatile.Read(ref _instance) is not null;

    private LogManager(LogManagerConfig config, LogMessageProcessor processor)
    {
        _config = config;
        _processor = processor;
        config.ApplyChangesCallback = Configure;
    }

    /// <summary>
    /// Initializes the logging system using the default synchronous processor.
    /// </summary>
    /// <param name="config">The logging configuration.</param>
    /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The configured async queue capacity is not greater than zero.</exception>
    /// <exception cref="ArgumentException">A writer configuration is invalid.</exception>
    /// <exception cref="InvalidOperationException">The manager has already been initialized.</exception>
    public static void Initialize(LogManagerConfig config) => Initialize<LogMessageSyncProcessor>(config);

    /// <summary>
    /// Initializes the logging system using a custom processor type.
    /// </summary>
    /// <typeparam name="TMessageProcessorFactory">The processor type implementing <see cref="ILogMessageProcessorFactory"/>.</typeparam>
    /// <param name="config">The logging configuration.</param>
    /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The configured async queue capacity is not greater than zero.</exception>
    /// <exception cref="ArgumentException">A writer configuration is invalid.</exception>
    /// <exception cref="InvalidOperationException">The manager has already been initialized.</exception>
    public static void Initialize<TMessageProcessorFactory>(LogManagerConfig config) where TMessageProcessorFactory: LogMessageProcessor, ILogMessageProcessorFactory
    {
        ArgumentNullException.ThrowIfNull(config);
        ValidateConfiguration(config);

        var instance = new LogManager(config, TMessageProcessorFactory.Create(config));
        if (Interlocked.CompareExchange(ref _instance, instance, null) is not null)
        {
            instance._processor.Dispose();
            throw new InvalidOperationException("The LogManager is already initialized");
        }

        try
        {
            instance.Initialize();
            instance.Configure();
        }
        catch
        {
            if (Interlocked.CompareExchange(ref _instance, null, instance) == instance)
            {
                instance.ShutdownCore();
                ResetLoggersAfterShutdown();
            }

            throw;
        }
    }

    private void Initialize()
    {
        _processor.Initialize();
    }

    /// <summary>
    /// Shuts down logging and disposes configured writers.
    /// </summary>
    public static void Shutdown()
    {
        var instance = Interlocked.Exchange(ref _instance, null);
        if (instance is null) return;

        instance.ShutdownCore();
        ResetLoggersAfterShutdown();
    }
    
    /// <summary>
    /// Gets a logger for the fully qualified name of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type used to derive the logger name.</typeparam>
    /// <returns>A logger instance.</returns>
    /// <exception cref="InvalidOperationException">The type full name is unavailable.</exception>
    public static Logger GetLogger<T>() => GetLogger(typeof(T).FullName ?? throw new InvalidOperationException("The typeof(T).FullName returned null"));

    /// <summary>
    /// Gets a logger for the specified category <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The logger category name.</param>
    /// <returns>A logger instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="name"/> is empty or whitespace.</exception>
    public static Logger GetLogger(string name)
    {
        ThrowIfNullOrWhiteSpace(name);

        var logManager = _instance;

        return (logManager != null) ?
            Loggers.GetOrAdd(name, static (name, instance) =>
            {
                var logger = new Logger(name);
                instance.Configure(logger);
                return logger;
            }, logManager)
            :
            Loggers.GetOrAdd(name, static name => new Logger(name));
    }

    /// <summary>
    /// Gets runtime diagnostics for the active log manager.
    /// </summary>
    /// <returns>The current diagnostics snapshot.</returns>
    public static LogManagerDiagnostics GetDiagnostics()
    {
        var instance = _instance;
        if (instance is null)
        {
            return LogManagerDiagnostics.Uninitialized;
        }

        var processor = instance._processor;
        if (processor is LogMessageAsyncProcessor asyncProcessor)
        {
            return new LogManagerDiagnostics(
                true,
                processor.GetType(),
                true,
                asyncProcessor.QueueLength,
                asyncProcessor.QueueCapacity,
                asyncProcessor.DroppedCount);
        }

        return new LogManagerDiagnostics(
            true,
            processor.GetType(),
            false,
            0,
            0,
            0);
    }

    private void Configure()
    {
        _configVersion++;
        var context = GetConfigurationContext();

        var loggers = Loggers.Values.ToArray();
        Array.Sort(loggers, (a, b) => string.CompareOrdinal(a.Name, b.Name));

        // For each logger, we will compute the final level and writers
        foreach (var logger in loggers)
        {
            context.ResetPerLogger();
            Configure(logger, context);
        }

        context.ResetAll();
    }

    private void Configure(Logger logger)
    {
        var context = GetConfigurationContext();
        Configure(logger, context);
        context.ResetAll();
    }

    private LoggerConfigurationContext GetConfigurationContext()
    {
        var instance = LoggerConfigurationContext.GetInstance();
        // ReSharper disable once InconsistentlySynchronizedField
        instance.LoggerConfigs.AddRange(_config.Loggers);
        instance.LoggerConfigs.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));
        return instance;
    }

    private void Configure(Logger logger, LoggerConfigurationContext context)
    {
        var loggerConfigMatches = context.LoggerConfigMatches;
        var loggerConfigs = context.LoggerConfigs;
        ref var levelToWriters = ref context.LevelToWriters;
        var allWriters = context.AllWriters;

        // Clear the list of matching logger configs
        loggerConfigMatches.Clear();
        // The root logger is always included
        loggerConfigMatches.Add(_config.RootLogger);

        // If we find a logger config that matches the logger name, we will add it to the list of matching logger configs
        foreach (var loggerConfig in loggerConfigs)
        {
            // config: x.y loggers: x.y, x.y.z a.b.c
            // should match x.y, x.y.z
            if (logger.Name.StartsWith(loggerConfig.Name) && (logger.Name.Length == loggerConfig.Name.Length || logger.Name[loggerConfig.Name.Length] == '.'))
            {
                loggerConfigMatches.Add(loggerConfig);
            }
        }

        // The final level is the minimum level defined by the last config or the root level
        var finalLevel = _config.RootLogger.MinimumLevel ?? LogLevel.None;
        var finalOverflowMode = _config.OverflowMode ?? _config.RootLogger.OverflowMode;

        foreach (var loggerConfig in loggerConfigMatches)
        {
            finalLevel = loggerConfig.MinimumLevel ?? finalLevel;
            finalOverflowMode = loggerConfig.OverflowMode ?? finalOverflowMode;

            // If a config doesn't include parent writers, we will clear all writers
            if (!loggerConfig.IncludeParentWriters)
            {
                foreach(var writers in levelToWriters)
                {
                    writers.Clear();
                }
            }

            foreach(var writerConfig in loggerConfig.Writers)
            {
                // Configure only once writers
                if (allWriters.Add(writerConfig.Writer))
                {
                    // Double verify with version number
                    writerConfig.Writer.Configure(_configVersion);
                }

                var minLevelFromWriter = Math.Max((int)writerConfig.MinimumLevel, (int)writerConfig.Writer.MinimumLevel);

                // For all levels from the writer level to the final level, we will add the writer
                for (int level = minLevelFromWriter; level < (int)LogLevel.None; ++level)
                {
                    levelToWriters[level].Add(writerConfig.Writer);
                }
            }
        }

        // Clear all level writers before the final level
        for (int level = 0; level < (int)finalLevel; level++)
        {
            levelToWriters[level].Clear();
        }

        ComputedLoggerState state = new(finalLevel, finalOverflowMode ?? LoggerOverflowMode.Default);
        for(int level = 0; level < (int)LogLevel.None; level++)
        {
            var writers = levelToWriters[level];
            var levelWriters = writers.Count == 0 ? [] : levelToWriters[level].ToArray();
            state.WritersPerLevel[level] = levelWriters;
        }

        logger.Configure(state);
    }

    private static void ResetLoggersAfterShutdown()
    {
        foreach (var logger in Loggers.Values)
        {
            logger.ResetAfterShutdown();
        }
    }

    private class LoggerConfigurationContext
    {
        [ThreadStatic]
        private static LoggerConfigurationContext? _context;

        private LoggerConfigurationContext()
        {
            LoggerConfigMatches = new List<LoggerConfig>();
            LoggerConfigs = new List<LoggerConfig>();
            LevelToWriters = new LogWriterPerLevel();

            for (int i = 0; i < LogWriterPerLevel.Length; ++i)
            {
                LevelToWriters[i] = new HashSet<LogWriter>(ReferenceEqualityComparer.Instance);
            }
            AllWriters = new HashSet<LogWriter>(ReferenceEqualityComparer.Instance);
        }

        public static LoggerConfigurationContext GetInstance()
        {
            var context = _context;
            if (context is null)
            {
                _context = context = new LoggerConfigurationContext();
            }
            return context;
        }

        public void ResetPerLogger()
        {
            LoggerConfigMatches.Clear();
            for (int i = 0; i < LogWriterPerLevel.Length; ++i)
            {
                LevelToWriters[i].Clear();
            }
        }

        public void ResetAll()
        {
            ResetPerLogger();
            LoggerConfigs.Clear();
            AllWriters.Clear();
        }

        public readonly List<LoggerConfig> LoggerConfigMatches;

        public readonly List<LoggerConfig> LoggerConfigs;

        public LogWriterPerLevel LevelToWriters;

        public readonly HashSet<LogWriter> AllWriters;
    }

    private void ShutdownCore()
    {
        _config.ApplyChangesCallback = null;

        _processor.Dispose();

        var uniqueWriters = new HashSet<LogWriter>(ReferenceEqualityComparer.Instance);
        foreach (var writerConfig in _config.RootLogger.Writers)
        {
            uniqueWriters.Add(writerConfig.Writer);
        }

        foreach (var loggerConfig in _config.Loggers)
        {
            foreach (var writerConfig in loggerConfig.Writers)
            {
                uniqueWriters.Add(writerConfig.Writer);
            }
        }

        foreach (var writer in uniqueWriters)
        {
            writer.Flush();
            writer.Dispose();
        }
    }

    private static void ValidateConfiguration(LogManagerConfig config)
    {
        if (config.AsyncLogMessageQueueCapacity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(config.AsyncLogMessageQueueCapacity),
                config.AsyncLogMessageQueueCapacity,
                "AsyncLogMessageQueueCapacity must be greater than zero.");
        }

        ValidateLoggerConfig(config.RootLogger, nameof(config.RootLogger));

        foreach (var loggerConfig in config.Loggers)
        {
            ValidateLoggerConfig(loggerConfig, $"Loggers['{loggerConfig.Name}']");
        }
    }

    private static void ValidateLoggerConfig(LoggerConfig loggerConfig, string configPath)
    {
        var writerConfigs = loggerConfig.Writers;
        var index = 0;
        foreach (var writerConfig in writerConfigs)
        {
            if (writerConfig is null)
            {
                throw new ArgumentException(
                    $"{configPath}.Writers[{index}] cannot be null.",
                    nameof(LogManagerConfig));
            }

            if (writerConfig.Writer is null)
            {
                throw new ArgumentException(
                    $"{configPath}.Writers[{index}].Writer cannot be null.",
                    nameof(LogManagerConfig));
            }

            index++;
        }
    }

    private static void ThrowIfNullOrWhiteSpace(string? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
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

    [InlineArray(Length)]
    private struct LogWriterPerLevel
    {
        public const int Length = (int)LogLevel.None + 1;

        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        private HashSet<LogWriter> _writers;
    }
}
