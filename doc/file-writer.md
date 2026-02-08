# File and JSON Writers

`XenoAtom.Logging` includes production-oriented file sinks in the core package:

- `FileLogWriter`: text output with rolling/retention support.
- `JsonFileLogWriter`: JSON-lines output (`.jsonl`) for ingestion pipelines.

Both use only BCL APIs and have no external dependencies.

## Basic file logging

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
```

## Rolling and retention

```csharp
var writer = new FileLogWriter(
    new FileLogWriterOptions("logs/app.log")
    {
        FileSizeLimitBytes = 10 * 1024 * 1024, // 10 MB
        RollingInterval = FileRollingInterval.Daily,
        RetainedFileCountLimit = 14,
        AutoFlush = false,
        ArchiveTimestampMode = FileArchiveTimestampMode.Utc,
        FlushToDisk = false
    });
```

Behavior:

- When the size limit is reached, the current file is archived and a new active file is created.
- When the rolling interval boundary is crossed (hour/day in UTC), the active file is archived.
- Archived files are named `app.<timestamp>[.<sequence>].log` using `ArchiveTimestampMode` (`Utc` or `Local`).
- Retention deletes oldest archives beyond `RetainedFileCountLimit`.
- `FlushToDisk = true` forces durable `FileStream.Flush(true)` for crash-critical workloads.

## JSON-lines logging

```csharp
var writer = new JsonFileLogWriter(
    new FileLogWriterOptions("logs/app.jsonl")
    {
        RollingInterval = FileRollingInterval.Daily,
        RetainedFileCountLimit = 7
    });
```

`JsonFileLogWriter` emits one JSON object per line and includes:

- `timestamp`, `level`, `logger`, `message`, `threadId`
- `eventId` (`id`, `name`) when present
- `exception` when present
- `properties` and `scopes` arrays (unless disabled in custom formatter usage)

By default, JSON lines are terminated with `\n` (JSONL-friendly), independent of platform newline conventions.

You can pass `JsonLogFormatterOptions` directly to `JsonFileLogWriter`:

```csharp
using XenoAtom.Logging.Formatters;

var writer = new JsonFileLogWriter(
    new FileLogWriterOptions("logs/app.jsonl")
    {
        RollingInterval = FileRollingInterval.Daily
    },
    new JsonLogFormatterOptions
    {
        SchemaProfile = JsonLogSchemaProfile.ElasticCommonSchema,
        IncludeException = true,
        IncludeScopes = false
    });
```

`FieldNamingPolicy` applies only to `JsonLogSchemaProfile.Default`. ECS output uses fixed field names.

## Advanced formatter customization

`FileLogWriter` uses `StandardLogFormatter` by default. You can replace it:

```csharp
var options = new FileLogWriterOptions("logs/app.log")
{
    Formatter = new JsonLogFormatter(includeProperties: true, includeScopes: true)
};
var writer = new FileLogWriter(options);
```

For template-based text formatters (`StandardLogFormatter`, `CompactLogFormatter`, `DetailedLogFormatter`, and custom `[LogFormatter]` records), see `doc/log-formatter.md`.

## Failure handling policy

`FileLogWriter` supports explicit failure handling for write/roll I/O errors:

```csharp
var options = new FileLogWriterOptions("logs/app.log")
{
    FailureMode = FileLogWriterFailureMode.Retry,
    RetryCount = 5,
    RetryDelay = TimeSpan.FromMilliseconds(100),
    FailureHandler = context =>
    {
        Console.WriteLine(
            $"File log failure on {context.FilePath}. " +
            $"Attempt={context.Attempt} Retry={context.WillRetry} " +
            $"Error={context.Exception.Message}");
    }
};
```

Modes:

- `Throw`: rethrow I/O failures.
- `Ignore`: drop the failed write.
- `Retry`: retry and then rethrow when retries are exhausted.

## Operational guidance

- Use asynchronous processing (`LogManager.Initialize<LogMessageAsyncProcessor>(config)`) for best throughput.
- Set `AutoFlush = true` only when strict durability per message is required.
- Prefer daily rolling plus size limit for long-running services.
- Keep retention finite to cap disk usage.
- Use `LogManager.GetDiagnostics()` to monitor async queue pressure and dropped messages.
