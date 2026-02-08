// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging;

/// <summary>
/// Represents configuration for a single logger category.
/// </summary>
public sealed class LoggerConfig
{
    internal LoggerConfig(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Gets the logger category name this configuration applies to.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets or sets the minimum level for this logger category.
    /// </summary>
    public LogLevel? MinimumLevel { get; set; }

    /// <summary>
    /// Gets or sets the overflow behavior for this logger category.
    /// </summary>
    public LoggerOverflowMode? OverflowMode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether parent writers are inherited.
    /// </summary>
    public bool IncludeParentWriters { get; set; } = true;

    /// <summary>
    /// Gets the writer configurations associated with this logger category.
    /// </summary>
    /// <remarks>
    /// Mutate this collection only during configuration updates, then call <see cref="LogManagerConfig.ApplyChanges"/>.
    /// </remarks>
    public LogWriterConfigCollection Writers { get; } = new();
}

/// <summary>
/// Represents a collection of <see cref="LogWriterConfig"/> values.
/// </summary>
public sealed class LogWriterConfigCollection : List<LogWriterConfig>
{
    /// <summary>
    /// Adds a writer with its own <see cref="LogWriter.MinimumLevel"/>.
    /// </summary>
    /// <param name="writer">The writer to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="writer"/> is <see langword="null"/>.</exception>
    public void Add(LogWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer);
        Add(new LogWriterConfig(writer));
    }

    /// <summary>
    /// Adds a writer with an explicit minimum level override.
    /// </summary>
    /// <param name="writer">The writer to add.</param>
    /// <param name="minimumLevel">The minimum level for the writer entry.</param>
    /// <exception cref="ArgumentNullException"><paramref name="writer"/> is <see langword="null"/>.</exception>
    public void Add(LogWriter writer, LogLevel minimumLevel)
    {
        ArgumentNullException.ThrowIfNull(writer);
        Add(new LogWriterConfig(writer, minimumLevel));
    }
}
