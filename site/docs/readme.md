---
title: "User Guide"
---

# User Guide

This guide helps you adopt `XenoAtom.Logging` in production without getting overwhelmed:

- Start with a minimal setup.
- Choose sync or async behavior intentionally.
- Add structure, sinks, and formatting incrementally.

If you are new to the library, begin with [Getting Started](getting-started.md).

## Installation

Core package:

```sh
dotnet add package XenoAtom.Logging
```

Optional terminal package:

```sh
dotnet add package XenoAtom.Logging.Terminal
```

## A minimal working setup

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

LogManager.Initialize(config);

var logger = LogManager.GetLogger("App.Main");
logger.Info("Application started");

LogManager.Shutdown();
```

## Core concepts

### 1) Lifecycle

- `LogManager.Initialize(...)` for synchronous processing.
- `LogManager.InitializeForAsync(...)` for queued asynchronous processing.
- `LogManager.Shutdown()` to flush and dispose writers.

### 2) Logger hierarchy

- `RootLogger` defines defaults (level, writers, overflow mode).
- `Loggers.Add("App.Http", ...)` overrides by name prefix.
- `includeParents` controls whether parent/root writers are inherited.

See [Filtering and Routing](filtering.md).

### 3) Structured data

- Per-message properties with `LogProperties`.
- Scoped properties with `BeginScope`.

```csharp
logger.Info(
    properties: new LogProperties { ("requestId", 1423), ("tenant", "alpha") },
    msg: "Processing request");
```

## Choosing sync vs async

Use sync mode when sink I/O is low latency and predictable.

Use async mode when throughput is high or when sink I/O can block:

```csharp
config.AsyncLogMessageQueueCapacity = 4096;
config.AsyncErrorHandler = ex => Console.Error.WriteLine(ex.Message);
config.RootLogger.OverflowMode = LoggerOverflowMode.Block;

LogManager.InitializeForAsync(config);
```

Overflow guidance:

- `Block`: safest for correctness (recommended default for critical logs).
- `Drop` / `DropAndNotify`: lower producer latency, but allow loss.
- `Allocate`: temporary queue growth under pressure.

See [Shutdown Semantics](shutdown.md) and [Thread Safety](thread-safety.md) for operational behavior.

## Writer choices

### File and JSON pipelines

- `FileLogWriter` for rolling text logs.
- `JsonFileLogWriter` for JSONL ingestion pipelines.

See [File and JSON Writers](file-writer.md).

### Terminal and UI rendering

- `TerminalLogWriter` for terminal output.
- `TerminalLogControlWriter` for `XenoAtom.Terminal.UI` `LogControl`.
- `InfoMarkup`/`ErrorMarkup` for markup-aware payloads.

See [Terminal Integration](terminal.md) and [Terminal Visual Examples](terminal-visuals.md).

## Formatter choices

Text formatters:

- `StandardLogFormatter`
- `CompactLogFormatter`
- `DetailedLogFormatter`

Custom text formatters:

- `[LogFormatter("...")]` on partial records
- shared runtime formatter settings (`LevelFormat`, `TimestampFormat`)

See [Log Formatters](log-formatter.md).

## Source-generated logging methods

Use `[LogMethod]` to define strongly-typed logging APIs:

```csharp
public static partial class AppLogs
{
    [LogMethod(LogLevel.Info, "User {userId} connected")]
    public static partial void UserConnected(Logger logger, int userId);
}
```

See [Source-generated Logging](source-generator.md).

## Coming from Microsoft.Extensions.Logging

Use [Migration from Microsoft.Extensions.Logging](microsoft-extensions-logging.md) for API mapping and rollout guidance.

## Documentation map

- [Getting Started](getting-started.md)
- [Migration from Microsoft.Extensions.Logging](microsoft-extensions-logging.md)
- [Package Consumption Guide](packages.md)
- [Log Formatters](log-formatter.md)
- [Source-generated Logging](source-generator.md)
- [Filtering and Routing](filtering.md)
- [File and JSON Writers](file-writer.md)
- [Terminal Integration](terminal.md)
- [Terminal Visual Examples](terminal-visuals.md)
- [Thread Safety](thread-safety.md)
- [Shutdown Semantics](shutdown.md)
- [Native AOT and Trimming](aot.md)
- [Benchmarks](benchmarks.md)
