---
title: "Log Formatters"
---

# Log Formatters

`XenoAtom.Logging` formats a `LogMessage` into text using `LogFormatter` implementations.

Goals:

- Zero allocations on the hot path.
- Formatting into `Span<char>` (and optionally emitting segment metadata for styling).
- User-defined text formats via source generation (`[LogFormatter]`).
- Common formatter settings (`LevelFormat`, `TimestampFormat`) centralized on `LogFormatter`.

This page covers user-facing APIs and customization patterns. For implementation-level details, see the internal formatter spec (`site/docs/specs/formatter_specs.md`) in the repository.

## Built-in formatters

The core package includes ready-to-use text formatters:

- `StandardLogFormatter` (single line: timestamp, level, logger name, optional event id, message, optional exception)
- `CompactLogFormatter` (time, level, message)
- `DetailedLogFormatter` (like standard plus thread id and sequence id)

`JsonLogFormatter` is also available. It is not template-based and targets JSON-line ingestion scenarios.

## Selecting a formatter

`StreamLogWriter` and `FileLogWriter` are formatter-based. You can set a formatter explicitly:

```csharp
using XenoAtom.Logging;
using XenoAtom.Logging.Formatters;
using XenoAtom.Logging.Writers;

var config = new LogManagerConfig
{
    RootLogger =
    {
        Writers =
        {
            new StreamLogWriter(Console.OpenStandardOutput())
            {
                Formatter = CompactLogFormatter.Instance
            }
        }
    }
};

LogManager.Initialize(config);
```

Or configure `FileLogWriter` via options:

```csharp
using XenoAtom.Logging.Formatters;
using XenoAtom.Logging.Writers;

var writer = new FileLogWriter(
    new FileLogWriterOptions("logs/app.log")
    {
        Formatter = DetailedLogFormatter.Instance
    });
```

For terminal output, `TerminalLogWriter` also accepts any `LogFormatter`:

```csharp
var terminalWriter = new TerminalLogWriter(Terminal.Instance)
{
    Formatter = StandardLogFormatter.Instance
};
```

## Runtime customization

`LevelFormat` and `TimestampFormat` are inherited from `LogFormatter`. Template-generated formatters initialize these values in generated constructors, and you can override them with `with { ... }`:

```csharp
using XenoAtom.Logging;
using XenoAtom.Logging.Formatters;

var formatter = StandardLogFormatter.Instance with
{
    TimestampFormat = "HH:mm:ss",
    LevelFormat = LogLevelFormat.Long
};
```

The default `LevelFormat` is `Tri` (aligned 3-character levels such as `INF`, `WRN`, `ERR`).

`TimestampFormat` defaults to `"yyyy-MM-dd HH:mm:ss.fffffff"` unless changed by the formatter template (for example `{Timestamp:HH:mm:ss}`).

## Writing your own formatter (template-driven)

Add a partial record inheriting `LogFormatter` and decorate it with `[LogFormatter]`:

```csharp
using XenoAtom.Logging;

namespace MyApp.Logging;

[LogFormatter("{Timestamp:HH:mm:ss} {Level,-5} {LoggerName} {Text}{? | {Exception}?}")]
public sealed partial record MyLogFormatter : LogFormatter;
```

The generator emits a `TryFormat(...)` implementation and a singleton `Instance` property:

```csharp
var formatter = MyLogFormatter.Instance;
```

## Template syntax (user-facing)

Formatter templates look like .NET composite format strings. The generator validates templates at compile time and emits a specialized `TryFormat(...)` implementation.

### Literals and escaping braces

Everything outside `{...}` is copied verbatim. To output literal braces, escape them:

- two opening braces (`{` then `{`) produce `{`
- two closing braces (`}` then `}`) produce `}`

### Field placeholders

A field placeholder has this shape:

```text
{FieldName[,alignment][:format]}
```

Rules:

- Field names are **case-insensitive** (`{Level}` == `{level}`).
- Whitespace around the name/alignment is allowed (the generator trims it).
- If a value is longer than its alignment width, it is **not truncated** (same behavior as composite formatting).

Alignment examples:

```csharp
[LogFormatter("{Level,-5} {LoggerName,30} {Text}")]
public sealed partial record Example : LogFormatter;
```

### Conditional sections: `{? ... ?}`

Conditional sections let you omit a region when referenced fields are empty:

```csharp
[LogFormatter("{Timestamp} {Level} {LoggerName}{? [{EventId}]?} {Text}{? | {Exception}?}")]
public sealed partial record Standardish : LogFormatter;
```

In that template:

- `[{EventId}]` is emitted only when `EventId` is present.
- `| {Exception}` is emitted only when an exception is attached.

Rules and behavior:

- A conditional section starts with `{?` and ends with `?}`.
- Conditional sections **cannot be nested**.
- A conditional section must contain at least one field placeholder.
- A conditional section is emitted only if **all emptyable fields referenced in that section are non-empty**.
- If a conditional contains no emptyable fields, the generator warns with `XLF0006` (it is always emitted).

Emptyable fields are:

- `{EventId}` (empty when `logMessage.EventId.IsEmpty`)
- `{Exception}` (empty when `logMessage.Exception is null`)
- `{Scope}` (empty when `logMessage.Scope.Count == 0`)
- `{Properties}` (empty when `logMessage.Properties.Count == 0`)
- `{Text}` (empty when `logMessage.Text.IsEmpty`)

All other fields are always considered present.

### Supported fields and format specifiers

Fields are case-insensitive and map to `LogMessage` data. The set is closed.

| Field | Meaning | Format (`:...`) |
|---|---|---|
| `{Timestamp}` | Timestamp | .NET `DateTime` format string (default comes from `TimestampFormat`) |
| `{Level}` | Level | `short`, `long`, `tri`, `char` (default comes from `LevelFormat`) |
| `{LoggerName}` | Logger category | No format specifier |
| `{EventId}` | Event id | `id`, `name` (default: `id` and `:name` only when available) |
| `{Text}` | Rendered message payload | No format specifier |
| `{Exception}` | Exception text | `message`, `type` (default: `Exception.ToString()`) |
| `{Thread}` | Thread | `id`, `name` (default: id) |
| `{SequenceId}` | Sequence id | Standard numeric formats (e.g. `D6`) |
| `{Scope}` | Flattened scope properties | `separator=...` (default `", "`) |
| `{Properties}` | Structured properties | `separator=...` (default `", "`) |
| `{NewLine}` | New line | No format specifier (uses `Environment.NewLine`) |

Notes:

- `{Timestamp:...}` sets the default timestamp format for the formatter. A single template cannot specify multiple different timestamp formats.
- `{Level:...}` sets the default `LevelFormat` for the formatter. A single template cannot specify multiple different level formats.
- `{Scope}` and `{Properties}` render as `name=value` pairs, flattened and separated by the configured separator.
  Example: `{Properties:separator= | }` renders `a=1 | b=2`.

## Using formatters with writers

Most text sinks take a formatter (`StreamLogWriter`, `FileLogWriter`, `TerminalLogWriter`):

```csharp
var writer = new FileLogWriter(
    new FileLogWriterOptions("logs/app.log")
    {
        Formatter = StandardLogFormatter.Instance with
        {
            TimestampFormat = "HH:mm:ss",
            LevelFormat = LogLevelFormat.Tri
        }
    });
```

### Enabling the generator in your app

The generator/analyzer is embedded in the `XenoAtom.Logging` package, so a single package reference is enough:

```xml
<ItemGroup>
  <PackageReference Include="XenoAtom.Logging" Version="*" />
</ItemGroup>
```

When working from source with project references, referencing `XenoAtom.Logging.csproj` is generally enough. If analyzers do not run in your setup, add an explicit analyzer reference to `XenoAtom.Logging.Generators.csproj`.

## Attachments and markup

Formatters receive `LogMessage` with:

- `Attachment`: optional attachment object (for example `Exception` or terminal `Visual`)
- `Exception`: convenience view (`Attachment as Exception`)
- `IsMarkup`: whether message text contains markup tags

Text-based non-terminal writers (`StreamLogWriter`, `FileLogWriter`, `JsonLogFormatter`) strip markup tags from `{Text}` when `IsMarkup` is true. Terminal writers can render markup directly.

Example:

```csharp
logger.InfoMarkup("[green]ready[/]");
```

## Segments and terminal styling

Text formatters can optionally emit segment metadata (`LogMessageFormatSegments`) while formatting. This lets writers style output by segment kind (timestamp/level/logger name, etc.).

`TerminalLogWriter` uses these segment kinds to apply styles:

```csharp
terminalWriter.Styles.Clear();
terminalWriter.Styles.SetStyle(LogMessageFormatSegmentKind.Timestamp, "dim");
terminalWriter.Styles.SetLevelStyle(LogLevel.Error, "bold white on red");
```

See [Terminal Integration](terminal.md) for terminal-specific styling and markup logging.

## Troubleshooting

Common generator diagnostics:

- `XLF0004`: `[LogFormatter]` is on a non-`partial` type, or the type does not inherit `LogFormatter`.
- `XLF0005`: `[LogFormatter]` is on a property that is not `static partial`, or the containing type is not `partial`.
- `XLF0001`/`XLF0002`: malformed template or invalid field/specifier.
- `XLF0003`: invalid format specifier (e.g., `{Level:oops}` or `{Properties:sep=, }`).
- `XLF0006`: conditional section is always emitted (no emptyable fields inside).

If you consume NuGet packages and the generator does not run, verify that you use the .NET 10 SDK and analyzers are enabled. If you use project references and analyzers are not flowing, add an explicit analyzer reference to `XenoAtom.Logging.Generators.csproj`.
