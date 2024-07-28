// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace XenoAtom.Logging;

public sealed class LogManager
{
    private static readonly ConcurrentDictionary<string, Logger> Loggers = new();
    private static LogManager? _instance;
    private readonly LogManagerConfig _config;
    private readonly LogMessageProcessor _processor;
    private long _configVersion;

    internal static TimeProvider TimeProvider => _instance?._config.TimeProvider ?? TimeProvider.System;

    internal static LogMessageProcessor? Processor => _instance?._processor;

    private LogManager(LogManagerConfig config)
    {
        _config = config;
        _processor = config.Kind switch
        {
            LogManagerKind.Async => new LogMessageAsyncProcessor(config),
            LogManagerKind.Sync => new LogMessageSyncProcessor(config),
            _ => throw new ArgumentOutOfRangeException()
        };
        config.ApplyChangesCallback = Configure;
    }

    public static void Initialize(LogManagerConfig config)
    {
        if (_instance != null)
        {
            throw new InvalidOperationException("The LogManager is already initialized");
        }

        _instance = new LogManager(config);
        _instance.Initialize();
        _instance.Configure();
    }

    private void Initialize()
    {
        _processor.Initialize();
    }

    public static void Shutdown()
    {
        if (_instance is null) return;

        // TODO: shutdown asynchronously all loggers, runners and writers
        
        _instance = null;
    }
    
    public static Logger GetLogger<T>() => GetLogger(typeof(T).FullName ?? throw new InvalidOperationException("The typeof(T).FullName returned null"));

    public static Logger GetLogger(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

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
        var finalOverflowMode = _config.RootLogger.OverflowMode;

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
            state.WritersPerLevel[level] = writers.Count == 0 ? [] : levelToWriters[level].ToArray();
        }

        logger.Configure(state);
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

    [InlineArray(Length)]
    private struct LogWriterPerLevel
    {
        public const int Length = (int)LogLevel.None + 1;

        [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
        private HashSet<LogWriter> _writers;
    }
}