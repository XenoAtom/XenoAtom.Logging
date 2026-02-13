---
title: "Terminal Integration"
---

# XenoAtom.Logging.Terminal

`XenoAtom.Logging.Terminal` provides terminal output for XenoAtom.Logging using `XenoAtom.Terminal` and `XenoAtom.Terminal.UI`.

It supports:

- rich segment styling (timestamp, level, logger name, event id, exception)
- markup payload rendering via `Logger.*Markup(...)` extension methods
- visual attachments (`XenoAtom.Terminal.UI.Visual`) rendered under the log line
- `LogControl` sink integration for fullscreen UI apps
- escaped interpolated markup values via `AnsiMarkupInterpolatedStringHandler`

For details on template-based text formatting and segment kinds, see [Log Formatters](log-formatter.md).

For terminal primitives, capabilities, and runtime behavior, see the `XenoAtom.Terminal` docs: https://xenoatom.github.io/terminal/docs/terminal/

## Why a separate package

- `XenoAtom.Logging` stays dependency-minimal and does not use `System.Console`.
- Terminal concerns (ANSI, markup, terminal capabilities, virtual backends) are delegated to `XenoAtom.Terminal`.

## Usage

```csharp
using XenoAtom.Logging;
using XenoAtom.Logging.Writers;

var config = new LogManagerConfig
{
    RootLogger =
    {
        MinimumLevel = LogLevel.Info,
        Writers =
        {
            new TerminalLogWriter()
        }
    }
};

LogManager.Initialize(config);
var logger = LogManager.GetLogger("Sample.Terminal");
logger.Info("Hello terminal");
LogManager.Shutdown();
```

## Writers

### `TerminalLogWriter`

- Writes to a `TerminalInstance`
- Supports rich segment styling + markup payload rendering
- Renders `Visual` attachments after the log line

### `TerminalLogControlWriter`

- Writes to a `LogControl` (`XenoAtom.Terminal.UI.Controls`)
- Uses the same formatting/styling pipeline as `TerminalLogWriter`
- Marshals background thread writes to the UI thread (`logControl.App.Post(...)` when available)
- Does not render attachments by default (override point lives in the shared base)

```csharp
using XenoAtom.Logging.Writers;
using XenoAtom.Terminal.UI.Controls;

var logControl = new LogControl();
var writer = new TerminalLogControlWriter(logControl)
{
    EnableRichFormatting = true,
    EnableMarkupMessages = true
};
```

## Rich styling

`TerminalLogWriter` enables rich formatting by default:

```csharp
var writer = new TerminalLogWriter()
{
    EnableRichFormatting = true,
    EnableMarkupMessages = true,
};
```

`TerminalLogWriter.Styles` lets you configure styles by segment kind and by log level:

```csharp
writer.Styles.Clear();
writer.Styles.SetStyle(LogMessageFormatSegmentKind.Timestamp, "dim");
writer.Styles.SetStyle(LogMessageFormatSegmentKind.LoggerName, "blue");
writer.Styles.SetLevelStyle(LogLevel.Info, "green");
writer.Styles.SetLevelStyle(LogLevel.Error, "bold white on red");
```

`TerminalLogStyleConfiguration` is intended to be configured during startup; avoid mutating `Styles` concurrently with active logging unless you provide your own synchronization.

You can restore defaults at any time:

```csharp
writer.Styles.ResetToDefaults();
```

For advanced scenarios, use `SegmentStyleResolver` as an override hook. If it returns `null`/empty, the writer falls back to `Styles`:

```csharp
writer.SegmentStyleResolver = (kind, level) =>
    kind == LogMessageFormatSegmentKind.EventId ? "bold magenta" : null;
```

## Markup logging extensions

Use `*Markup` level methods to opt-in to markup message rendering:

```csharp
logger.InfoMarkup("[green]ready[/] [gray]service started[/]");
logger.ErrorMarkup($"[red]failed[/] request={requestId}");
```

When using interpolated markup messages, formatted values are escaped:

```csharp
var userInput = "[red]not a tag[/]";
logger.InfoMarkup($"User input: {userInput}");
```

The rendered text includes the literal `[red]not a tag[/]` value instead of interpreting it as markup tags.

## Visual attachments

`TerminalLogWriter` renders `Visual` attachments (from `XenoAtom.Terminal.UI`) after the formatted message line:

```csharp
using XenoAtom.Terminal.UI;
using XenoAtom.Terminal.UI.Controls;

var table = new Table()
    .Headers("Step", "Status")
    .AddRow("Build", "OK")
    .AddRow("Deploy", "Done");

logger.Info(table, "Deployment summary");
logger.InfoMarkup(table, "[bold]Deployment report[/]");
```

Behavior by sink:

- `TerminalLogWriter`: renders the text line, then renders the attached `Visual`
- `FileLogWriter` / `StreamLogWriter`: write only text payload
- `JsonLogFormatter`: writes message/fields, attachment visuals are not serialized

## Testing

Use `InMemoryTerminalBackend` to assert output deterministically in tests.

## Sample

See `samples/HelloLogging/Program.cs` for a complete terminal demo with rich styling, markup logs, scopes, event IDs, and exception output.

See `samples/HelloLogControl/Program.cs` for a fullscreen `LogControl` demo with:

- explanatory `Markup` + `TextBlock` content
- buttons that emit plain and markup logs
- background thread logging at regular intervals

For a quick rendered-output walkthrough, see [Terminal Visual Examples](terminal-visuals.md).
