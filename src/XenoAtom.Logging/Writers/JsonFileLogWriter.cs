// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using XenoAtom.Logging.Formatters;

namespace XenoAtom.Logging.Writers;

/// <summary>
/// A <see cref="FileLogWriter"/> configured to emit one JSON object per line.
/// </summary>
/// <remarks>
/// Inherits the thread-safety characteristics of <see cref="FileLogWriter"/>.
/// </remarks>
public sealed class JsonFileLogWriter : FileLogWriter
{
    private const string JsonLinesNewLine = "\n";

    /// <summary>
    /// Initializes a new instance of <see cref="JsonFileLogWriter"/> with default options.
    /// </summary>
    /// <param name="filePath">The destination file path.</param>
    public JsonFileLogWriter(string filePath) : this(CreateDefaultOptions(filePath))
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="JsonFileLogWriter"/> with JSON formatter options.
    /// </summary>
    /// <param name="filePath">The destination file path.</param>
    /// <param name="formatterOptions">Optional JSON formatter options.</param>
    public JsonFileLogWriter(string filePath, JsonLogFormatterOptions? formatterOptions)
        : this(CreateDefaultOptions(filePath), formatterOptions)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="JsonFileLogWriter"/>.
    /// </summary>
    /// <param name="options">The base file writer options.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
    public JsonFileLogWriter(FileLogWriterOptions options) : base(CreateOptions(options))
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="JsonFileLogWriter"/>.
    /// </summary>
    /// <param name="options">The base file writer options.</param>
    /// <param name="formatterOptions">Optional JSON formatter options.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
    public JsonFileLogWriter(FileLogWriterOptions options, JsonLogFormatterOptions? formatterOptions)
        : base(CreateOptions(options, formatterOptions))
    {
    }

    private static FileLogWriterOptions CreateDefaultOptions(string filePath)
    {
        return CreateDefaultOptions(filePath, formatterOptions: null);
    }

    private static FileLogWriterOptions CreateDefaultOptions(string filePath, JsonLogFormatterOptions? formatterOptions)
    {
        var options = new FileLogWriterOptions(filePath);
        options.NewLine = JsonLinesNewLine;
        options.Formatter = formatterOptions is null
            ? JsonLogFormatter.Instance
            : new JsonLogFormatter(formatterOptions);
        return options;
    }

    private static FileLogWriterOptions CreateOptions(FileLogWriterOptions options)
    {
        return CreateOptions(options, formatterOptions: null);
    }

    private static FileLogWriterOptions CreateOptions(FileLogWriterOptions options, JsonLogFormatterOptions? formatterOptions)
    {
        ArgumentNullException.ThrowIfNull(options);
        var copiedOptions = new FileLogWriterOptions(options)
        {
            NewLine = options.NewLine == Environment.NewLine ? JsonLinesNewLine : options.NewLine,
            Formatter = formatterOptions is null
                ? JsonLogFormatter.Instance
                : new JsonLogFormatter(formatterOptions)
        };
        return copiedOptions;
    }
}
