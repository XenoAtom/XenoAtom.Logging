---
title: "Migration from Microsoft.Extensions.Logging"
---

# Migration from Microsoft.Extensions.Logging

This guide helps teams move from `Microsoft.Extensions.Logging` (MEL) patterns to `XenoAtom.Logging`.

## Scope of compatibility

`XenoAtom.Logging` is not a MEL provider/bridge today.  
Migration is API-level: update application logging code and startup configuration.

## Quick mapping

{.table}
| MEL concept | XenoAtom.Logging concept |
|---|---|
| `ILogger<TCategoryName>` | `Logger` from `LogManager.GetLogger("Category")` |
| `ILoggerFactory` / provider pipeline | `LogManager.Initialize(...)` with configured writers |
| `LogLevel` | `LogLevel` |
| `EventId` | `LogEventId` |
| `BeginScope(...)` | `BeginScope(LogProperties)` |
| `LoggerMessageAttribute` | `[LogMethod]` |
| `appsettings` provider config | `LogManagerConfig` + code configuration |

## Minimal migration example

### MEL

```csharp
builder.Services.AddLogging(logging =>
{
    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
    logging.AddConsole();
});
```

### XenoAtom.Logging

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
```

## Message templates and source generation

MEL `LoggerMessageAttribute`:

```csharp
[LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Processing item {ItemId}")]
public static partial void ProcessingItem(ILogger logger, int itemId);
```

XenoAtom.Logging equivalent:

```csharp
using XenoAtom.Logging;

public static partial class AppLog
{
    [LogMethod(LogLevel.Info, "Processing item {itemId}", EventId = 1)]
    public static partial void ProcessingItem(Logger logger, int itemId);
}
```

See [Source-generated Logging](source-generator.md) for full generator rules.

## Scopes and structured data

MEL scope style:

```csharp
using (_logger.BeginScope("Request {RequestId}", requestId))
{
    _logger.LogInformation("Handling request");
}
```

XenoAtom.Logging style:

```csharp
using (logger.BeginScope(new LogProperties { ("RequestId", requestId) }))
{
    logger.Info("Handling request");
}
```

Per-message properties are explicit and allocation-aware:

```csharp
logger.Info(
    properties: new LogProperties { ("UserId", 42), ("Feature", "checkout") },
    msg: "Validation failed");
```

## Async behavior differences

MEL provider behavior depends on provider implementation.  
XenoAtom.Logging has explicit async queue settings:

- `InitializeForAsync(config)`
- `AsyncLogMessageQueueCapacity`
- `OverflowMode` (`Drop`, `DropAndNotify`, `Block`, `Allocate`)
- `AsyncErrorHandler`

Use `Block` when correctness is more important than producer latency.

## Recommended migration strategy

1. Introduce `LogManager` startup/shutdown lifecycle.
2. Replace logger injection with `Logger` access (`LogManager.GetLogger(...)`) in app entry points/services.
3. Migrate high-traffic logs first to `[LogMethod]` or interpolated APIs.
4. Move sinks to `FileLogWriter`/`JsonFileLogWriter`/`TerminalLogWriter` as needed.
5. Validate throughput and behavior with [Benchmarks](benchmarks.md) and app-specific load tests.

## Common pitfalls

- Forgetting `LogManager.Shutdown()` on process exit (can lose buffered writes).
- Using `Drop` overflow in critical audit paths.
- Mutating config/filter collections from multiple threads without synchronization.

See [Thread Safety](thread-safety.md) and [Shutdown Semantics](shutdown.md) for operational guidance.
