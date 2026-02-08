# Terminal Visual Examples

This page shows example output produced by `XenoAtom.Logging.Terminal`.

Actual colors and style intensity depend on terminal capabilities and your configured styles.

## Default style example

```text
2026-02-09 18:21:04.7253194 INF Sample.App: Service started
2026-02-09 18:21:04.7319831 WRN Sample.App: Queue latency above threshold
2026-02-09 18:21:04.7424107 ERR Sample.App [42]: Failed to process request
System.InvalidOperationException: boom
```

## Markup message example

```text
2026-02-09 18:21:04.7501062 INF Sample.Terminal: ready worker=api-1
2026-02-09 18:21:04.7529450 ERR Sample.Terminal: failed request=REQ-0042
```

The underlying message payload can include markup tags (for example `[green]ready[/]`), and `TerminalLogWriter` renders them when `EnableMarkupMessages` is enabled.

## Style customization sample

```csharp
var writer = new TerminalLogWriter(Terminal.Instance)
{
    EnableRichFormatting = true,
    EnableMarkupMessages = true,
};

writer.Styles.Clear();
writer.Styles.SetStyle(LogMessageFormatSegmentKind.Timestamp, "dim");
writer.Styles.SetStyle(LogMessageFormatSegmentKind.LoggerName, "blue");
writer.Styles.SetLevelStyle(LogLevel.Info, "green");
writer.Styles.SetLevelStyle(LogLevel.Warn, "bold yellow");
writer.Styles.SetLevelStyle(LogLevel.Error, "bold white on red");
```

See also:

- [`terminal.md`](terminal.md)
- [`log-formatter.md`](log-formatter.md)

