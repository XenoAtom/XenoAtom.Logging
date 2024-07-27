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

    internal static TimeProvider TimeProvider => _instance?._config.TimeProvider ?? TimeProvider.System;

    private LogManager(LogManagerConfig config)
    {
        _config = config;
        config.ApplyChangesCallback = Configure;
    }

    public static void Initialize(LogManagerConfig config)
    {
        if (_instance != null)
        {
            throw new InvalidOperationException("The LogManager is already initialized");
        }

        _instance = new LogManager(config);
        _instance.Configure();
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
                // TODO: initialize logger with manager config
                return logger;
            }, logManager)
            :
            Loggers.GetOrAdd(name, static name => new Logger(name));
    }

    private void Configure()
    {
        var loggerConfigs = _config.Loggers.ToArray();
        Array.Sort(loggerConfigs, (a, b) => string.CompareOrdinal(a.Name, b.Name));

        var loggers = Loggers.Values.ToArray();
        Array.Sort(loggers, (a, b) => string.CompareOrdinal(a.Name, b.Name));

        var loggerConfigMatches = new List<LoggerConfig>();
        var levelToWriters = new LogWriterPerLevel();

        for(int i = 0; i < LogWriterPerLevel.Length; ++i)
        {
            levelToWriters[i] = new HashSet<LogWriter>(ReferenceEqualityComparer.Instance);
        }

        var allWriters = new HashSet<LogWriter>();

        // For each logger, we will compute the final level and writers
        foreach (var logger in loggers)
        {
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

            foreach (var loggerConfig in loggerConfigMatches)
            {
                finalLevel = loggerConfig.MinimumLevel ?? finalLevel;

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
                        writerConfig.Writer.Configure();
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

            ComputedLoggerState state = new(finalLevel);
            for(int level = 0; level < (int)LogLevel.None; level++)
            {
                var writers = levelToWriters[level];
                state.WritersPerLevel[level] = writers.Count == 0 ? [] : levelToWriters[level].ToArray();
            }

            logger.Update(state);
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