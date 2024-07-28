// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

/// <summary>
/// The base class for a log writer.
/// </summary>
public abstract class LogWriter : IDisposable
{
    private LogFilter[] _acceptFilters; // Frozen array of accept filters (to avoid multi-threading issues)
    private LogFilter[] _rejectFilters; // Frozen array of reject filters (to avoid multi-threading issues)
    private bool _hasFilters;
    private long _configVersion;

    public const int DefaultFormatterBufferSize = 16384;

    /// <summary>
    /// Default constructor for <see cref="LogWriter"/>.
    /// </summary>
    protected LogWriter()
    {
        _acceptFilters = [];
        _rejectFilters = [];
    }
    
    /// <summary>
    /// The minimum level this writer will log
    /// </summary>
    public LogLevel MinimumLevel { get; init; } = LogLevel.All;
    
    /// <summary>
    /// Gets the filters to accept a log message. By default, if no accept filters are defined, all log messages are accepted.
    /// </summary>
    public List<LogFilter> AcceptFilters { get; } = new();

    /// <summary>
    /// Gets the filters to reject a log message. By default, if no reject filters are defined, no log messages are rejected.
    /// </summary>
    /// <remarks>
    /// The reject filters are applied before the accept filters and will return immediately if a filter rejects the log message.
    /// </remarks>
    public List<LogFilter> RejectFilters { get; } = new();

    /// <summary>
    /// Disposes this writer.
    /// </summary>
    public virtual void Dispose()
    {
    }

    /// <summary>
    /// Flushes this writer.
    /// </summary>
    public virtual void Flush()
    {
    }

    /// <summary>
    /// Internal method to freeze the configuration of this writer, called by <see cref="LogManager"/>
    /// </summary>
    internal void Configure(long configVersion)
    {
        // Don't reconfigure if the version is the same
        if (_configVersion == configVersion)
        {
            return;
        }

        // Freeze the filters to avoid multi-threading issues
        _acceptFilters = AcceptFilters.ToArray();
        _rejectFilters = RejectFilters.ToArray();
        _hasFilters = _acceptFilters.Length > 0 || _rejectFilters.Length > 0;
        _configVersion = configVersion;
    }

    /// <summary>
    /// Logs a message to this writer.
    /// </summary>
    /// <param name="logMessage">The message to log.</param>
    protected abstract void Log(in LogMessage logMessage);

    /// <summary>
    /// Logs a message taking into account the level of this writer and the filters.
    /// </summary>
    /// <param name="logMessage">The message to log</param>
    internal void LogInternal(in LogMessage logMessage)
    {
        if (logMessage.Level < MinimumLevel)
        {
            return;
        }

        // If we have filters, we need to apply them
        // otherwise skip the filters checks entirely
        if (_hasFilters)
        {
            // If at least one reject filter then we reject the log message
            var rejectFilters = _rejectFilters;
            foreach (var filter in rejectFilters)
            {
                if (filter(logMessage))
                {
                    return;
                }
            }

            // If at least one accept filter then we accept the log message
            // If we have no accept filters, we accept the log message
            var acceptFilters = _acceptFilters;
            if (acceptFilters.Length > 0)
            {
                bool accepted = false;
                foreach (var filter in acceptFilters)
                {
                    if (filter(logMessage))
                    {
                        accepted = true;
                        break;
                    }
                }

                if (!accepted)
                {
                    return;
                }
            }
        }

        Log(logMessage);
    }
}