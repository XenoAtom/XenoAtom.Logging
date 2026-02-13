---
title: "Getting Started"
---

# Getting Started

This guide walks through a minimal setup, then shows the async and terminal variants.

## 1. Install packages

Core runtime:

```sh
dotnet add package XenoAtom.Logging
```

Optional terminal integration:

```sh
dotnet add package XenoAtom.Logging.Terminal
```

## 2. Minimal synchronous setup

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
            new StreamLogWriter(Console.OpenStandardOutput())
        }
    }
};

LogManager.Initialize(config);

var logger = LogManager.GetLogger("App");
logger.Info("Application started");
logger.Warn($"Retry attempt = {3}");

LogManager.Shutdown();
```

Key points:

- `Initialize` defaults to sync processing.
- `GetLogger("App")` creates/gets a logger category.
- `Shutdown()` flushes and disposes writers.

## 3. Switch to asynchronous processing

When logging throughput is high, initialize for async:

```csharp
var config = new LogManagerConfig
{
    AsyncLogMessageQueueCapacity = 4096,
    AsyncErrorHandler = exception =>
    {
        Console.Error.WriteLine($"[async logging error] {exception.Message}");
    },
    RootLogger =
    {
        MinimumLevel = LogLevel.Info,
        OverflowMode = LoggerOverflowMode.Block,
        Writers =
        {
            new FileLogWriter("logs/app.log")
        }
    }
};

LogManager.InitializeForAsync(config);
```

`OverflowMode` behavior:

- `Block` (default recommendation): preserve correctness, backpressure producers.
- `Drop` / `DropAndNotify`: prioritize producer latency, allow loss.
- `Allocate`: temporarily exceed configured queue capacity.

## 4. Structured values and scopes

Per-message structured data:

```csharp
var properties = new LogProperties
{
    ("requestId", 1423),
    ("tenant", "alpha")
};

logger.Info(properties: properties, msg: "Processing request");
```

Scoped contextual data:

```csharp
using (logger.BeginScope(new LogProperties { ("traceId", "abc-123") }))
{
    logger.Info("Inside request scope");
}
```

## 5. Terminal output (optional)

```csharp
using XenoAtom.Logging.Writers;
using XenoAtom.Terminal;
using XenoAtom.Terminal.Backends;

using (Terminal.Open(new InMemoryTerminalBackend(), force: true))
{
    var writer = new TerminalLogWriter(Terminal.Instance);
    var config = new LogManagerConfig();
    config.RootLogger.Writers.Add(writer);

    LogManager.Initialize(config);
    LogManager.GetLogger("App").InfoMarkup("[green]ready[/]");
    LogManager.Shutdown();
}
```

See [Terminal Integration](terminal.md) for styling, markup, and `LogControl`.

## 6. What to read next

- [User Guide](readme.md)
- [Migration from Microsoft.Extensions.Logging](microsoft-extensions-logging.md)
- [Log Formatters](log-formatter.md)
- [File and JSON Writers](file-writer.md)
