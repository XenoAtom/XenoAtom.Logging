# XenoAtom.Logging.Terminal

`XenoAtom.Logging.Terminal` provides terminal output for XenoAtom.Logging using `XenoAtom.Terminal`.

It supports:

- rich segment styling (timestamp, level, logger name, event id, exception)
- markup payload rendering via `Logger.*Markup(...)` extension methods
- escaped interpolated markup values via `AnsiMarkupInterpolatedStringHandler`

For details on template-based text formatting and segment kinds, see `doc/log-formatter.md`.

## Why a separate package

- `XenoAtom.Logging` stays dependency-minimal and does not use `System.Console`.
- Terminal concerns (ANSI, markup, terminal capabilities, virtual backends) are delegated to `XenoAtom.Terminal`.

## Usage

```csharp
using XenoAtom.Logging;
using XenoAtom.Logging.Writers;
using XenoAtom.Terminal;
using XenoAtom.Terminal.Backends;

var backend = new InMemoryTerminalBackend();
using (Terminal.Open(backend, force: true))
{
    var config = new LogManagerConfig
    {
        RootLogger =
        {
            MinimumLevel = LogLevel.Info,
            Writers =
            {
                new TerminalLogWriter(Terminal.Instance)
            }
        }
    };

    LogManager.Initialize<LogMessageSyncProcessor>(config);
    var logger = LogManager.GetLogger("Sample.Terminal");
    logger.Info("Hello terminal");
    LogManager.Shutdown();
}
```

## Rich styling

`TerminalLogWriter` enables rich formatting by default:

```csharp
var writer = new TerminalLogWriter(Terminal.Instance)
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

## Testing

Use `InMemoryTerminalBackend` to assert output deterministically in tests.

## Sample

See `samples/HelloLogging/Program.cs` for a complete terminal demo with rich styling, markup logs, scopes, event IDs, and exception output.

For a quick rendered-output walkthrough, see [`terminal-visuals.md`](terminal-visuals.md).
