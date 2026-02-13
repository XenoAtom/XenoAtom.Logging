---
title: "User Guide"
---

# XenoAtom.Logging User Guide

This guide covers configuration, logging APIs, properties/scopes, processors, formatter behavior, and terminal integration.

If you are looking specifically for the template-based text formatting system, see [Log Formatters](log-formatter.md).

## Documentation map

- [Log Formatters](log-formatter.md): text formatter templates, generated formatters, `LevelFormat`/`TimestampFormat`
- [Terminal Integration](terminal.md): terminal writer, markup methods, visual attachments
- [Terminal Visual Examples](terminal-visuals.md): rendered examples and styling walkthrough
- [File and JSON Writers](file-writer.md): file rolling, retention, and failure modes
- [Filtering and Routing](filtering.md): writer filters and routing patterns
- [Thread Safety](thread-safety.md): concurrency guarantees and mutation guidance
- [Shutdown Semantics](shutdown.md): shutdown and flush guarantees
- [Source-generated Logging](source-generator.md): `LogMethod` usage and diagnostics
- [Benchmarks](benchmarks.md): benchmark suites and interpretation notes
- [Samples](https://github.com/XenoAtom/XenoAtom.Logging/tree/main/samples): runnable samples

## 1. Initialize and shutdown

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
            new FileLogWriter("logs/app.log")
        }
    }
};

LogManager.Initialize(config); // sync processor by default

var logger = LogManager.GetLogger("App.Main");
logger.Info("Application started");

LogManager.Shutdown();
```

If you need asynchronous processing, initialize with:

```csharp
LogManager.InitializeForAsync(config);
```

You can quickly check lifecycle state with `LogManager.IsInitialized`.

## 2. Logger hierarchy and levels

- `RootLogger` defines baseline level/writers.
- `Loggers.Add("A.B", LogLevel.Warn)` overrides by logger name prefix.
- `IncludeParentWriters` controls whether child logger configs inherit parent/root writers.

```csharp
config.RootLogger.MinimumLevel = LogLevel.Info;
config.Loggers.Add("App.Http", LogLevel.Debug);
```

## 3. Message APIs

Use generated methods from `LoggerExtensions`:

```csharp
logger.Trace("...");
logger.Debug($"value = {42}");
logger.Info(new LogEventId(10, "UserLogin"), "logged in");
logger.Error(new InvalidOperationException("boom"), "failed");
```

The interpolated handlers are designed for allocation-aware capture on the hot path.

## 4. Properties and scopes

Attach structured values to one message with `LogProperties`:

```csharp
var properties = new LogProperties
{
    ("UserId", 42),
    ("Tenant", "alpha")
};
logger.Info(properties: properties, msg: $"processed request");
```

Attach contextual properties to all logs in a scope:

```csharp
using (logger.BeginScope(new LogProperties { ("RequestId", 1234) }))
{
    logger.Info("inside request scope");
}
```

## 5. Async queue and overflow behavior

The asynchronous processor uses `LogManagerConfig.AsyncLogMessageQueueCapacity` and `LoggerOverflowMode`.

- `Drop`: drop new logs when saturated.
- `DropAndNotify`: drop and optionally notify.
- `Block`: producer blocks until queue has capacity.
- `Allocate`: grow past configured capacity.

```csharp
config.AsyncLogMessageQueueCapacity = 4096;
config.RootLogger.OverflowMode = LoggerOverflowMode.Drop;
```

In async mode, writer failures are handled on the background consumer thread and do not propagate back to caller threads.
Configure `AsyncErrorHandler` to observe these failures:

```csharp
config.AsyncErrorHandler = exception =>
{
    Console.Error.WriteLine($"[async logging failure] {exception.Message}");
};
```

You can inspect runtime async pressure, drop counters, and async error count:

```csharp
var diagnostics = LogManager.GetDiagnostics();
if (diagnostics.IsAsyncProcessor)
{
    Console.WriteLine(
        $"Queue {diagnostics.AsyncQueueLength}/{diagnostics.AsyncQueueCapacity} " +
        $"Dropped={diagnostics.DroppedMessages} " +
        $"Errors={diagnostics.ErrorCount}");
}
```

## 6. Writers

### `StreamLogWriter`

- Formats log entries via `LogFormatter` (default: `StandardLogFormatter`)
- Encodes text using the configured `Encoding`
- Writes bytes to the target stream
- Supports optional `AutoFlush` for immediate stream flush per entry

### `FileLogWriter`

- Writes formatted text to a file path
- Supports rolling by size (`FileSizeLimitBytes`)
- Supports rolling by UTC interval (`FileRollingInterval.Hourly`/`Daily`)
- Supports archived file retention (`RetainedFileCountLimit`)
- Supports archive timestamp clock mode (`ArchiveTimestampMode`)
- Supports optional durable flush (`FlushToDisk`)
- Supports write/roll failure policies (`FailureMode`, `RetryCount`, `RetryDelay`, `FailureHandler`)

```csharp
var fileWriter = new FileLogWriter(
    new FileLogWriterOptions("logs/app.log")
    {
        FileSizeLimitBytes = 10 * 1024 * 1024,
        RollingInterval = FileRollingInterval.Daily,
        RetainedFileCountLimit = 14
    });
```

### `JsonFileLogWriter`

- Convenience wrapper over `FileLogWriter` configured with `JsonLogFormatter`
- Emits one JSON object per line for ingestion systems
- Supports `JsonLogFormatterOptions` for schema profile, naming policy, and field inclusion toggles

```csharp
var jsonWriter = new JsonFileLogWriter("logs/app.jsonl");
```

```csharp
using XenoAtom.Logging.Formatters;

var ecsWriter = new JsonFileLogWriter(
    new FileLogWriterOptions("logs/app.ecs.jsonl"),
    new JsonLogFormatterOptions
    {
        SchemaProfile = JsonLogSchemaProfile.ElasticCommonSchema,
        IncludeScopes = false
    });
```

When `SchemaProfile` is `ElasticCommonSchema`, field names are fixed by ECS and `FieldNamingPolicy` is ignored.

### `TerminalLogWriter` (`XenoAtom.Logging.Terminal`)

- Terminal rendering is outside the core package
- Uses `XenoAtom.Terminal` for ANSI/markup-aware output
- Core package does not depend on `System.Console`
- Supports rich segment styling (`writer.Styles`) and markup payload rendering

```csharp
using XenoAtom.Logging.Writers;
using XenoAtom.Terminal;
using XenoAtom.Terminal.Backends;

var backend = new InMemoryTerminalBackend();
using (Terminal.Open(backend, force: true))
{
    var terminalWriter = new TerminalLogWriter(Terminal.Instance);
    config.RootLogger.Writers.Add(terminalWriter);
}
```

Markup payload logging is available via terminal-specific extensions:

```csharp
logger.InfoMarkup("[green]ready[/]");
logger.ErrorMarkup($"[red]failed[/] id={requestId}");
```

Style configuration example:

```csharp
terminalWriter.Styles.Clear();
terminalWriter.Styles.SetStyle(LogMessageFormatSegmentKind.Timestamp, "dim");
terminalWriter.Styles.SetLevelStyle(LogLevel.Warn, "bold yellow");
terminalWriter.Styles.SetLevelStyle(LogLevel.Error, "bold white on red");
```

### `TerminalLogControlWriter` (`XenoAtom.Logging.Terminal`)

- Writes to `XenoAtom.Terminal.UI.Controls.LogControl`
- Uses the same rich formatting/markup pipeline as `TerminalLogWriter`
- Marshals background logging calls to the UI thread when hosted in a running `TerminalApp`

```csharp
using XenoAtom.Terminal.UI.Controls;

var logControl = new LogControl();
var controlWriter = new TerminalLogControlWriter(logControl)
{
    EnableRichFormatting = true,
    EnableMarkupMessages = true
};
```

## 7. Notes on performance

- Best hot-path behavior is achieved with enabled log calls that use supported interpolated values (string, bool, unmanaged `ISpanFormattable` values, spans).
- Formatting and sink I/O are performed on the consumer side (especially with async processing).
- For throughput-sensitive scenarios, prefer long-lived writers and avoid frequent reconfiguration.

## 8. Thread-safety guidance

- `LogManager` and `Logger` are safe for concurrent logging calls.
- Configure `LogManagerConfig`, `LoggerConfig.Writers`, and writer filter collections from a single thread.
- Apply runtime changes through `LogManagerConfig.ApplyChanges()` after you finish mutating configuration.
- `LogProperties` is a mutable value type; avoid copying populated instances and dispose only the owner instance.

## 9. Source-generated logging

Use `[LogMethod]` on static partial methods to generate strongly-typed logging wrappers from message templates.

```csharp
public static partial class AppLogs
{
    [LogMethod(LogLevel.Info, "User {userId} connected")]
    public static partial void UserConnected(Logger logger, int userId);
}
```

See [Source-generated Logging](source-generator.md) for full details and diagnostics.

## 10. Additional docs

- Log formatters: [Log Formatters](log-formatter.md)
- Filtering and routing: [Filtering and Routing](filtering.md)
- File and JSON writers: [File and JSON Writers](file-writer.md)
- Shutdown semantics: [Shutdown Semantics](shutdown.md)
- Terminal sink: [Terminal Integration](terminal.md)
- Terminal visual examples: [Terminal Visual Examples](terminal-visuals.md)
- Native AOT and trimming: [Native AOT and Trimming](aot.md)
- Package consumption: [Package Consumption Guide](packages.md)
- Source generators: [Source-generated Logging](source-generator.md)
- Benchmarks: [Benchmarks](benchmarks.md)
- Thread safety: [Thread Safety](thread-safety.md)
- Samples: [Samples](https://github.com/XenoAtom/XenoAtom.Logging/tree/main/samples)
