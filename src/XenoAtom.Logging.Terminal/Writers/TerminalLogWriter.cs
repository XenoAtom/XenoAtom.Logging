// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using XenoAtom.Terminal;
using XenoAtom.Terminal.UI;

namespace XenoAtom.Logging.Writers;

/// <summary>
/// A <see cref="LogWriter"/> that writes formatted messages to <see cref="TerminalInstance"/>.
/// </summary>
public sealed class TerminalLogWriter : TerminalLogWriterBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalLogWriter"/> class using <see cref="XenoAtom.Terminal.Terminal.Instance"/>.
    /// </summary>
    public TerminalLogWriter() : this(XenoAtom.Terminal.Terminal.Instance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalLogWriter"/> class.
    /// </summary>
    /// <param name="terminal">The terminal instance receiving output.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="terminal"/> is <see langword="null"/>.</exception>
    public TerminalLogWriter(TerminalInstance terminal)
    {
        ArgumentNullException.ThrowIfNull(terminal);
        Terminal = terminal;
    }

    /// <summary>
    /// Gets the target terminal instance.
    /// </summary>
    public TerminalInstance Terminal { get; }

    /// <inheritdoc />
    protected override void AppendLine(scoped ReadOnlySpan<char> text)
    {
        Terminal.Write(text);
        Terminal.WriteLine();
    }

    /// <inheritdoc />
    protected override void AppendMarkupLine(scoped ReadOnlySpan<char> markupText)
    {
        Terminal.WriteMarkup(markupText);
        Terminal.WriteLine();
    }

    /// <inheritdoc />
    protected override void WriteAttachment(object? attachment)
    {
        if (attachment is Visual visual)
        {
            Terminal.Write(visual);
        }
    }
}
