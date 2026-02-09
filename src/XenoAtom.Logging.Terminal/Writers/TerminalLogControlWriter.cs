// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using XenoAtom.Terminal.UI.Controls;

namespace XenoAtom.Logging.Writers;

/// <summary>
/// A <see cref="LogWriter"/> that writes formatted messages to <see cref="LogControl"/>.
/// </summary>
public sealed class TerminalLogControlWriter : TerminalLogWriterBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalLogControlWriter"/> class.
    /// </summary>
    /// <param name="logControl">The log control instance receiving output.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="logControl"/> is <see langword="null"/>.</exception>
    public TerminalLogControlWriter(LogControl logControl)
    {
        ArgumentNullException.ThrowIfNull(logControl);
        LogControl = logControl;
    }

    /// <summary>
    /// Gets the target log control instance.
    /// </summary>
    public LogControl LogControl { get; }

    /// <inheritdoc />
    protected override void AppendLine(scoped ReadOnlySpan<char> text)
    {
        if (LogControl.Dispatcher.CheckAccess())
        {
            LogControl.AppendLine(text.ToString());
            return;
        }

        var captured = text.ToString();
        var app = LogControl.App;
        if (app is not null)
        {
            app.Post(() => LogControl.AppendLine(captured));
            return;
        }

        LogControl.Dispatcher.Post(() => LogControl.AppendLine(captured));
    }

    /// <inheritdoc />
    protected override void AppendMarkupLine(scoped ReadOnlySpan<char> markupText)
    {
        if (LogControl.Dispatcher.CheckAccess())
        {
            LogControl.AppendMarkupLine(markupText.ToString());
            return;
        }

        var captured = markupText.ToString();
        var app = LogControl.App;
        if (app is not null)
        {
            app.Post(() => LogControl.AppendMarkupLine(captured));
            return;
        }

        LogControl.Dispatcher.Post(() => LogControl.AppendMarkupLine(captured));
    }
}
