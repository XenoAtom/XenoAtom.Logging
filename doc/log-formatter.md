# Log Formatters

`XenoAtom.Logging` formats a `LogMessage` into text using `LogFormatter` implementations.

Key goals:

- Zero allocations on the hot path.
- Formatting into `Span<char>` (and optionally emitting segment metadata for styling).
- User-defined text formats via source generation (`[LogFormatter]`).
- Common formatter settings (`LevelFormat`, `TimestampFormat`) centralized on `LogFormatter`.

This page covers the user-facing API and common customization patterns. For the full template spec, see `doc/specs/formatter_specs.md`.

## Built-in formatters

The core package includes a few ready-to-use text formatters:

- `StandardLogFormatter` (single line: timestamp, level, logger name, optional event id, message, optional exception)
- `CompactLogFormatter` (time, level, message)
- `DetailedLogFormatter` (like standard + thread id and a sequence id)

There is also `JsonLogFormatter` which is not template-based and has different semantics (JSON lines for ingestion).

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

`LevelFormat` and `TimestampFormat` are inherited from `LogFormatter`. Template-generated formatters initialize these values in generated constructors, and you can still override them with `with { ... }`:

```csharp
using XenoAtom.Logging;
using XenoAtom.Logging.Formatters;

var formatter = StandardLogFormatter.Instance with
{
    TimestampFormat = "HH:mm:ss",
    LevelFormat = LogLevelFormat.Long
};
```

Default `LevelFormat` is `Tri` (aligned 3-character levels like `INF`, `WRN`, `ERR`).

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

### Enabling the generator in your app

The generator/analyzer is embedded in the `XenoAtom.Logging` package, so a single package reference is enough:

```xml
<ItemGroup>
  <PackageReference Include="XenoAtom.Logging" Version="*" />
</ItemGroup>
```

When working from source with project references, referencing `XenoAtom.Logging.csproj` is generally enough. If your setup disables transitive analyzers, add a direct analyzer reference to `XenoAtom.Logging.Generators.csproj`.

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

## Template quick reference

Fields are case-insensitive:

| Field | Meaning |
|---|---|
| `{Timestamp}` | Timestamp from the message (`TimestampFormat` controls formatting) |
| `{Level}` | Log level (`LevelFormat` controls representation) |
| `{LoggerName}` | Logger name |
| `{EventId}` | Event id (`id`, `name`, or `id:name` depending on specifier) |
| `{Text}` | Rendered message text |
| `{Exception}` | Exception (type/message depending on specifier) |
| `{Thread}` | Thread id/name depending on specifier |
| `{SequenceId}` | Sequence id (use standard numeric format strings like `D6`) |
| `{Scope}` | Rendered scope values |
| `{Properties}` | Rendered properties |
| `{NewLine}` | New line |

Conditional blocks omit content when the referenced field is empty:

```csharp
[LogFormatter("{LoggerName}{? [{EventId}]?} {Text}{? | {Exception}?}")]
public sealed partial record Standardish : LogFormatter;
```

## Segments and terminal styling

Text formatters can optionally emit segment metadata (`LogMessageFormatSegments`) while formatting. This enables writers to style output by segment kind (timestamp/level/logger name, etc.).

`TerminalLogWriter` uses these segment kinds to apply styles:

```csharp
terminalWriter.Styles.Clear();
terminalWriter.Styles.SetStyle(LogMessageFormatSegmentKind.Timestamp, "dim");
terminalWriter.Styles.SetLevelStyle(LogLevel.Error, "bold white on red");
```

See `doc/terminal.md` for terminal-specific styling and markup logging.

## Troubleshooting

Common generator diagnostics:

- `XLF0004`: `[LogFormatter]` is on a non-`partial` type, or the type does not inherit `LogFormatter`.
- `XLF0005`: `[LogFormatter]` is on a property that is not `static partial`, or the containing type is not `partial`.
- `XLF0001`/`XLF0002`: malformed template or invalid field/specifier.

If the generator does not run, make sure `XenoAtom.Logging.Generators` is referenced as an analyzer (see "Enabling the generator in your app").
