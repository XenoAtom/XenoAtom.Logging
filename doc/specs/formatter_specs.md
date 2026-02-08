# Source-Generated Log Formatter — Specification

**Date:** 2026-02-09  
**Status:** Draft  
**Affects:** `XenoAtom.Logging`, `XenoAtom.Logging.Generators`

---

## 1. Goals

1. **Replace `DefaultLogFormatter`** with a source-generated system that produces compile-time-optimized `TryFormat` implementations from declarative format templates.
2. **Zero-allocation hot path** — generated code writes directly into `Span<char>` using `TryFormat`/`TryAppend` patterns, exactly like the hand-written `DefaultLogFormatter` today.
3. **Familiar syntax** — use .NET composite format conventions (`{Field}`, `{Field:format}`, `{Field,alignment}`) so the template reads like a standard interpolated string.
4. **Conditional sections** — support omitting template regions when a field is null or empty (e.g., skip `| Exception` when no exception is attached).
5. **Runtime-configurable presentation** — allow field-level presentation options (e.g., level display style) to be changed without recompiling, through immutable `init`-only properties on the generated formatter record.
6. **Segment generation** — automatically emit `LogMessageFormatSegments` for each field, enabling downstream consumers (terminal writers) to style output by semantic kind.
7. **Compile-time validation** — the generator reports diagnostics for unknown fields, invalid format specifiers, and malformed templates.
8. **Ship predefined formatters** — provide a set of built-in templates as `partial record` types in the library so users have good defaults out of the box.

### Non-Goals

- JSON/structured output. The `JsonLogFormatter` has fundamentally different output semantics; it is not a template-driven text formatter and stays as-is.
- Custom user-defined fields beyond `LogMessage` properties. Properties and scopes are rendered as a unit, not as individually-addressable template fields.
- Full expression language (no `{#if condition}` blocks). Conditional sections are field-presence-driven, not expression-driven.

---

## 2. Format Template Syntax

### 2.1 Field References

A field reference is enclosed in curly braces and names a property of `LogMessage`:

```
{Timestamp} {Level} {LoggerName} {Text}
```

Everything outside curly braces is literal text, written verbatim.

Escaped braces: `{{` produces a literal `{`, `}}` produces a literal `}`.

### 2.2 Format Specifiers

Standard .NET format-specifier syntax after a colon:

```
{Timestamp:yyyy-MM-dd HH:mm:ss.fff}
{SequenceId:D8}
```

Field-specific custom specifiers are defined in §3.

### 2.3 Alignment

Standard .NET composite-format alignment before the colon:

```
{Level,-5}          left-align in a 5-char field
{LoggerName,30}     right-align in a 30-char field
{Level,-5:short}    left-align + format specifier
```

Generated code pads with spaces using span fill. If the rendered value exceeds the field width, no truncation occurs (consistent with .NET composite formatting).

### 2.4 Conditional Sections

A conditional section is delimited by `{?` and `?}`. The enclosed content is emitted **only if every field reference within the section evaluates to a non-null / non-empty value**:

```
{Timestamp} {Level} {LoggerName}{? [{EventId}]?} {Text}{? | {Exception}?}
```

| Input state | Emitted output |
|---|---|
| EventId present, exception present | `2026-02-09 ... INFO MyApp [42:Connect] Hello | System.Exception: ...` |
| EventId empty, exception null | `2026-02-09 ... INFO MyApp Hello` |
| EventId present, exception null | `2026-02-09 ... INFO MyApp [42:Connect] Hello` |

Rules:
- Conditional sections cannot be nested.
- A conditional section must contain at least one field reference.
- Literal text inside the section (e.g., ` [`, `]`, ` | `) is part of the conditional output.
- Fields inside conditional sections support alignment and format specifiers.

**How "emptiness" is determined per field:**

| Field | Empty when |
|---|---|
| `Exception` | `logMessage.Exception is null` |
| `EventId` | `logMessage.EventId.IsEmpty` |
| `Scope` | `logMessage.Scope.IsEmpty` |
| `Properties` | `logMessage.Properties.Count == 0` |
| `Text` | `logMessage.Text.IsEmpty` |
| All others | Never empty (always emitted) |

### 2.5 Complete Syntax Grammar

```
template       = (literal | field | conditional)*
literal        = <any text except '{' and '}'> | '{{' | '}}'
field          = '{' field-name [',' alignment] [':' format] '}'
field-name     = identifier              // see §3 for the closed set
alignment      = ['-'] digits            // negative = left-align
format         = <any text except '}'>   // field-specific, see §3
conditional    = '{?' (literal | field)+ '?}'
```

---

## 3. Available Fields

Each field maps to a `LogMessage` property and produces a specific `LogMessageFormatSegmentKind`.

| Template Field | LogMessage Access | Type | Segment Kind | Default Format | Supported Format Specifiers |
|---|---|---|---|---|---|
| `{Timestamp}` | `.Timestamp` | `DateTime` | `Timestamp` | `yyyy-MM-dd HH:mm:ss.fffffff` | Any standard/custom `DateTime` format string |
| `{Level}` | `.Level` | `LogLevel` | `Level` | `short` | `short` · `long` · `tri` · `char` (see §3.1) |
| `{LoggerName}` | `.Logger.Name` | `string` | `LoggerName` | As-is | None |
| `{EventId}` | `.EventId` | `LogEventId` | `EventId` | `Id:Name` or `Id` | `id` · `name` · (default) (see §3.2) |
| `{Text}` | `.Text` | `ReadOnlySpan<char>` | `Text` | As-is | None |
| `{Exception}` | `.Exception` | `Exception?` | `Exception` | `ToString()` | `message` · `type` · (default) (see §3.3) |
| `{Thread}` | `.Thread` | `Thread` | `ThreadId` | `ManagedThreadId` | `id` · `name` (see §3.4) |
| `{SequenceId}` | `.SequenceId` | `long` | `SequenceId` | Decimal | Any standard numeric format string |
| `{Scope}` | `.Scope` | `LogScope` | `SecondaryText` | Concatenated name=value pairs | `separator=X` (see §3.5) |
| `{Properties}` | `.Properties` | `LogPropertiesReader` | `SecondaryText` | Concatenated name=value pairs | `separator=X` (see §3.5) |
| `{NewLine}` | N/A | Special | — | `Environment.NewLine` | None (no segment emitted) |

Field names are **case-insensitive** in the template but recommended as PascalCase.

### 3.1 Level Format Specifiers

| Specifier | Example Output | Method Used |
|---|---|---|
| `short` | `INFO`, `DEBUG`, `WARN` | `ToShortString()` |
| `long` | `Information`, `Debug`, `Warning` | `ToLongString()` |
| `tri` (default) | `INF`, `DBG`, `WRN` | `ToTriString()` |
| `char` | `I`, `D`, `W` | First character of `ToShortString()` |

If no specifier is given, the default is `tri`. This default can be overridden by the `LevelFormat` `init` property on the generated record (see §5).

### 3.2 EventId Format Specifiers

| Specifier | Renders |
|---|---|
| (default) | `"Id:Name"` if Name is non-null/non-whitespace, else `"Id"` |
| `id` | Numeric ID only |
| `name` | Name only (empty string if Name is null) |

### 3.3 Exception Format Specifiers

| Specifier | Renders |
|---|---|
| (default) | `Exception.ToString()` (full stack trace) |
| `message` | `Exception.Message` only |
| `type` | `Exception.GetType().FullName` only |

### 3.4 Thread Format Specifiers

| Specifier | Renders |
|---|---|
| `id` (default) | `Thread.ManagedThreadId` (formatted as decimal) |
| `name` | `Thread.Name` (empty if null) |

### 3.5 Scope / Properties Format Specifiers

| Specifier | Meaning |
|---|---|
| `separator=X` | Use `X` as the separator between entries (default: `, `) |

Each entry is rendered as `name=value`. The key-value separator is always `=`.

---

## 4. Source Generator Usage

### 4.1 Attribute Placement — On a `partial record` Class

The primary usage places `[LogFormatter]` on a `partial record` class that inherits from `LogFormatter`:

```csharp
using XenoAtom.Logging;
using XenoAtom.Logging.Formatters;

[LogFormatter("{Timestamp:yyyy-MM-dd HH:mm:ss.fffffff} {Level,-5} {LoggerName}{? [{EventId}]?} {Text}{? | {Exception}?}")]
public sealed partial record StandardLogFormatter : LogFormatter;
```

The generator produces:
1. A static `Instance` property returning a singleton with default config.
2. The `TryFormat` override with optimized, template-specific code.
3. `init`-only properties for runtime-configurable aspects (see §5).

Usage:

```csharp
// Default:
var writer = new FileLogWriter(options) { Formatter = StandardLogFormatter.Instance };

// Customized via record `with`:
var writer = new FileLogWriter(options)
{
    Formatter = StandardLogFormatter.Instance with { LevelFormat = LogLevelFormat.Long }
};
```

### 4.2 Attribute Placement — On a Static Partial Property

For quick one-off formatters without defining a named type, `[LogFormatter]` can be placed on a `static partial` property in a `partial class`:

```csharp
public static partial class MyFormatters
{
    [LogFormatter("{Timestamp:HH:mm:ss} {Level} {Text}")]
    public static partial LogFormatter Compact { get; }
}
```

The generator creates a private nested sealed record class and returns a singleton instance. The property type is `LogFormatter` (base class), so `with {}` customization is not available — this form is for simple, non-configurable formatters only.

### 4.3 Validation Rules

The generator validates the template at compile time and reports diagnostics:

| Code | Severity | Condition |
|---|---|---|
| `XLF0001` | Error | Unknown field name (not in the §3 field table) |
| `XLF0002` | Error | Malformed template (unmatched braces, empty field name, nested conditionals) |
| `XLF0003` | Error | Invalid format specifier for a field (e.g., `{Level:xyz}`) |
| `XLF0004` | Error | `[LogFormatter]` on a non-`partial` type, or class not inheriting `LogFormatter` |
| `XLF0005` | Error | `[LogFormatter]` on a property that is not `static partial` in a `partial class` |
| `XLF0006` | Warning | Conditional section `{?...?}` contains no nullable/emptyable fields (always emitted) |

### 4.4 Generated Code Structure

For the record form:

```csharp
[LogFormatter("{Timestamp:yyyy-MM-dd HH:mm:ss.fffffff} {Level,-5} {LoggerName} {Text}{? | {Exception}?}")]
public sealed partial record StandardLogFormatter : LogFormatter
{
    // --- Generated ---

    /// <summary>Gets a shared instance with default configuration.</summary>
    public static StandardLogFormatter Instance { get; } = new();

    /// <summary>Gets or sets the display format for the log level field.</summary>
    public LogLevelFormat LevelFormat { get; init; } = LogLevelFormat.Tri;

    /// <summary>Gets or sets the format string for the timestamp field.</summary>
    public string TimestampFormat { get; init; } = "yyyy-MM-dd HH:mm:ss.fffffff";

    /// <inheritdoc />
    public override bool TryFormat(
        in LogMessage logMessage,
        Span<char> destination,
        out int charsWritten,
        ref LogMessageFormatSegments segments)
    {
        charsWritten = 0;
        var pos = 0;

        // {Timestamp:yyyy-MM-dd HH:mm:ss.fffffff}
        var seg0 = pos;
        if (!logMessage.Timestamp.TryFormat(destination[pos..], out var n, TimestampFormat))
            return false;
        pos += n;
        segments.Add(seg0, pos, LogMessageFormatSegmentKind.Timestamp);

        // " "
        if ((uint)pos >= (uint)destination.Length) return false;
        destination[pos++] = ' ';

        // {Level,-5}
        var seg1 = pos;
        var levelText = LevelFormat switch
        {
            LogLevelFormat.Short => logMessage.Level.ToShortString(),
            LogLevelFormat.Long  => logMessage.Level.ToLongString(),
            LogLevelFormat.Tri   => logMessage.Level.ToTriString(),
            LogLevelFormat.Char  => logMessage.Level.ToShortString().AsSpan(0, 1).ToString(),
            _ => logMessage.Level.ToShortString(),
        };
        if (!TryAppend(levelText.AsSpan(), destination, ref pos)) return false;
        // Left-align padding to width 5
        while (pos - seg1 < 5)
        {
            if ((uint)pos >= (uint)destination.Length) return false;
            destination[pos++] = ' ';
        }
        segments.Add(seg1, pos, LogMessageFormatSegmentKind.Level);

        // " "
        if ((uint)pos >= (uint)destination.Length) return false;
        destination[pos++] = ' ';

        // {LoggerName}
        var seg2 = pos;
        if (!TryAppend(logMessage.Logger.Name.AsSpan(), destination, ref pos)) return false;
        segments.Add(seg2, pos, LogMessageFormatSegmentKind.LoggerName);

        // " "
        if ((uint)pos >= (uint)destination.Length) return false;
        destination[pos++] = ' ';

        // {Text}
        var seg3 = pos;
        if (!TryAppend(logMessage.Text, destination, ref pos)) return false;
        segments.Add(seg3, pos, LogMessageFormatSegmentKind.Text);

        // {? | {Exception}?} — conditional
        if (logMessage.Exception is not null)
        {
            if (!TryAppend(" | ", destination, ref pos)) return false;
            var seg4 = pos;
            if (!TryAppend(logMessage.Exception.ToString().AsSpan(), destination, ref pos)) return false;
            segments.Add(seg4, pos, LogMessageFormatSegmentKind.Exception);
        }

        charsWritten = pos;
        return true;
    }

    // Shared helpers (emitted once per containing assembly, or inlined)
    private static bool TryAppend(ReadOnlySpan<char> text, Span<char> destination, ref int pos) { ... }
    private static bool TryAppend(char c, Span<char> destination, ref int pos) { ... }
}
```

Key codegen patterns:
- **Literal sequences** are merged into a single `TryAppend` call where possible (e.g., `" | "` is one call, not three).
- **Alignment** is generated as an inline loop or `span.Fill(' ')` after the field value.
- **Format specifiers** from the template become the default value of the corresponding `init` property. The generated `TryFormat` reads the property, allowing runtime override.
- **Conditional sections** are wrapped in an `if` block that checks the field's emptiness condition (see §2.4).
- **Segment registration** calls `segments.Add(start, pos, kind)` immediately after each field. If segments are disabled (the common case for non-terminal writers), the calls are no-ops by design of `LogMessageFormatSegments`.

---

## 5. Runtime Configuration via `init` Properties

When `[LogFormatter]` is placed on a `partial record`, the generator emits `init`-only properties for aspects that benefit from runtime configuration. The template's format specifiers provide the **compile-time defaults**; the properties allow overriding them at runtime without changing the template.

### 5.1 Generated Properties

| Property | Type | Default From | Controls |
|---|---|---|---|
| `LevelFormat` | `LogLevelFormat` | Template specifier (e.g., `:short`) or `Tri` | How `{Level}` is rendered |
| `TimestampFormat` | `string` | Template specifier or `"yyyy-MM-dd HH:mm:ss.fffffff"` | Format string passed to `DateTime.TryFormat` |

Only properties relevant to the fields used in the template are generated. If the template has no `{Level}`, no `LevelFormat` property is emitted.

### 5.2 `LogLevelFormat` Enum

```csharp
namespace XenoAtom.Logging;

/// <summary>
/// Specifies how a <see cref="LogLevel"/> value is displayed in formatted output.
/// </summary>
public enum LogLevelFormat
{
    /// <summary>Uppercase short name: TRACE, DEBUG, INFO, WARN, ERROR, FATAL.</summary>
    Short,

    /// <summary>Full mixed-case name: Trace, Debug, Information, Warning, Error, Fatal.</summary>
    Long,

    /// <summary>Three-letter abbreviation: TRC, DBG, INF, WRN, ERR, FTL.</summary>
    Tri,

    /// <summary>Single uppercase character: T, D, I, W, E, F.</summary>
    Char,
}
```

### 5.3 Customization via `with {}`

Because the generated type is a `record`, users create customized instances immutably:

```csharp
// Default config:
var fmt = StandardLogFormatter.Instance;

// Override level display:
var fmt2 = fmt with { LevelFormat = LogLevelFormat.Long };

// Override timestamp format:
var fmt3 = fmt with { TimestampFormat = "HH:mm:ss.fff" };

// Both:
var fmt4 = fmt with { LevelFormat = LogLevelFormat.Tri, TimestampFormat = "HH:mm:ss" };
```

Each `with {}` produces a new formatter instance. Since formatters are expected to be long-lived (set once on a writer), this one-time allocation is acceptable.

---

## 6. Predefined Formatters

The library ships a set of ready-to-use formatters, each defined as a `partial record` with `[LogFormatter]`:

```csharp
namespace XenoAtom.Logging.Formatters;

/// <summary>
/// Standard single-line text format.
/// <c>2026-02-09 12:34:56.1234567 INFO  MyApp.Service [42:Connect] Hello world | System.Exception: ...</c>
/// </summary>
[LogFormatter("{Timestamp:yyyy-MM-dd HH:mm:ss.fffffff} {Level,-5} {LoggerName}{? [{EventId}]?} {Text}{? | {Exception}?}")]
public sealed partial record StandardLogFormatter : LogFormatter;

/// <summary>
/// Compact format — timestamp (time only), level, message.
/// <c>12:34:56 INF Hello world</c>
/// </summary>
[LogFormatter("{Timestamp:HH:mm:ss} {Level} {Text}")]
public sealed partial record CompactLogFormatter : LogFormatter;

/// <summary>
/// Detailed format including thread ID and sequence number.
/// <c>2026-02-09 12:34:56.1234567 INFO  [14] #000042 MyApp.Service [42:Connect] Hello world | System.Exception: ...</c>
/// </summary>
[LogFormatter("{Timestamp:yyyy-MM-dd HH:mm:ss.fffffff} {Level,-5} [{Thread}] #{SequenceId:D6} {LoggerName}{? [{EventId}]?} {Text}{? | {Exception}?}")]
public sealed partial record DetailedLogFormatter : LogFormatter;
```

`StandardLogFormatter` replaces `DefaultLogFormatter` as the default formatter for `StreamLogWriter` and `FileLogWriter`.

### 6.1 Migration from `DefaultLogFormatter`

| Step | Action |
|---|---|
| 1 | Mark `DefaultLogFormatter` as `[Obsolete("Use StandardLogFormatter instead.")]` |
| 2 | Change `StreamLogWriter` and `FileLogWriterOptions` defaults from `DefaultLogFormatter.Instance` to `StandardLogFormatter.Instance` |
| 3 | Remove `DefaultLogFormatter` in the next major version |

---

## 7. Integration with Writers

### 7.1 Formatter Property

`StreamLogWriter` and `FileLogWriterOptions` already have a `Formatter` property. No structural change is needed — users assign any `LogFormatter` subclass:

```csharp
var writer = new StreamLogWriter(stream)
{
    Formatter = StandardLogFormatter.Instance with { LevelFormat = LogLevelFormat.Long }
};
```

### 7.2 Segment-Aware Writers

The generated `TryFormat` always calls `segments.Add(...)`. When segments are disabled (the default for file/stream writers), these calls are no-ops. When enabled (terminal writer), the segments enable per-field styling at zero additional cost.

This means the terminal writer gains automatic segment support from any `[LogFormatter]`-generated formatter, with no extra work.

---

## 8. Design Rationale

### 8.1 Why Source Generation Over Runtime Parsing?

| Concern | Runtime Parsing | Source Generation |
|---|---|---|
| **Startup cost** | Parse template string, allocate segment descriptors | Zero — code is compiled |
| **Render cost** | Indirect dispatch through delegate/virtual per segment | Direct inline code, trivially inlineable |
| **Validation** | Runtime exceptions on first use | Compile-time diagnostics |
| **Allocations** | Potential intermediate allocations in generic dispatch | Guaranteed zero-alloc (can be verified by analyzer) |
| **Debuggability** | Opaque runtime behavior | Generated code visible in IDE, steppable |

### 8.2 Why `partial record` Instead of `partial class`?

- `record` gives `with {}` for free — immutable copies with changed properties.
- `record` generates `Equals`, `GetHashCode`, `ToString` — useful for diagnostics.
- `record` can inherit from a non-record `abstract class` (`LogFormatter`) in C# 10+.
- The one-time allocation of `with {}` is acceptable since formatters are long-lived objects configured once at startup.

### 8.3 Why `{?...?}` for Conditionals Instead of `{#if}`?

- `{#if ...}` requires an expression evaluator, which conflicts with the "no expression language" non-goal.
- `{?...?}` is implicit — the condition is "are all fields in this section non-empty?" — which covers 95% of real-world cases (optional EventId, optional Exception, optional Scope).
- The syntax is visually lightweight and doesn't break the flow of reading the template.
- It's easy to parse in the generator (no recursive descent needed beyond matching `{?` with `?}`).

### 8.4 Why Not Merge LogFormatterConfig Into LogWriter?

The user considered putting configuration on `LogWriter` and passing it to formatters. Instead, the configuration lives *on the formatter itself* as `init` properties:

- **Cohesion** — timestamp format and level format are formatter concerns, not writer concerns.
- **Discoverability** — IntelliSense on the formatter type shows available options; a generic config bag does not.
- **Type safety** — each generated formatter has exactly the properties relevant to its template. A `CompactLogFormatter` that omits `{EventId}` doesn't expose an EventId-related property.
- **Immutability** — `init` + `record with {}` gives clean, thread-safe configuration. No mutable state that could race with the consumer thread.

---

## 9. Comparison with Other Libraries

| Aspect | XenoAtom (proposed) | Serilog | NLog | ZeroLog |
|---|---|---|---|---|
| **Template syntax** | `{Field,align:fmt}` | `{Property,align:fmt}` | `${renderer:opt=val}` | `%name` / `%{name:fmt}` |
| **Parsing** | Compile-time (source gen) | Once at config time | Once at config time | Once at construction |
| **Conditionals** | `{?...?}` sections | ExpressionTemplate add-on | `${when}`, `${onexception}` | None (code-level) |
| **Level formatting** | `short`/`long`/`tri`/`char` + runtime override | `:u3`/`:w3` | Named options | Custom per-level strings |
| **Runtime config** | `init` properties + `with {}` | Separate `IFormatProvider` | XML/code configuration | `Formatter` subclass |
| **Allocation** | Zero (Span-based codegen) | Moderate (StringWriter) | Moderate (StringBuilder) | Near-zero (Span-based) |
| **Validation** | Compile-time diagnostics | Runtime parse errors | Runtime parse errors | Runtime parse errors |
| **Extensibility** | User-defined `[LogFormatter]` records | `ITextFormatter` interface | Custom layout renderers | Subclass `Formatter` |

The proposal uniquely combines **compile-time validation + zero-alloc rendering + runtime configurability via records** — no other .NET logging library offers all three.

---

## 10. Implementation Plan

### Phase 1: Core Infrastructure

1. Add `LogLevelFormat` enum to `XenoAtom.Logging`.
2. Add `[LogFormatterAttribute]` to `XenoAtom.Logging` (similar pattern to existing `[LogMethodAttribute]`).
3. Implement template parser in the generator (`LogFormatterGenerator`): tokenize → validate → emit.
4. Implement codegen for `partial record` placement: emit `TryFormat`, `Instance`, `init` properties.
5. Add diagnostics `XLF0001`–`XLF0006`.

### Phase 2: Predefined Formatters & Migration

6. Add `StandardLogFormatter`, `CompactLogFormatter`, `DetailedLogFormatter` as `[LogFormatter]`-decorated records.
7. Change `StreamLogWriter` and `FileLogWriterOptions` defaults to `StandardLogFormatter.Instance`.
8. Mark `DefaultLogFormatter` as `[Obsolete]`.
9. Update `TerminalLogWriter` to work with any segment-emitting formatter.

### Phase 3: Static Property Placement

10. Implement codegen for `static partial` property placement (nested sealed record, singleton return).
11. Add diagnostic `XLF0005` for invalid property usage.

### Phase 4: Tests & Docs

12. Generator tests: all diagnostics, all field types, all format specifiers, conditional sections, alignment, edge cases.
13. Runtime tests: verify generated formatters produce identical output to `DefaultLogFormatter` for the standard template.
14. Update `doc/source-generator.md` with formatter documentation.
15. Update `doc/readme.md` and `samples/` with formatter examples.

---

## 11. Open Questions

1. **Scope/Properties rendering** — The current proposal renders `{Scope}` and `{Properties}` as flat concatenated strings. Should we support accessing individual scope/property values by name in the template (e.g., `{Properties.UserId}`)? This would add significant generator complexity.

2. **Newline handling in exceptions** — `Exception.ToString()` contains embedded newlines. Should the formatter have an option to indent continuation lines for alignment? This is a common pain point in multi-line log output.

3. **User-defined field resolvers** — Should we support an extensibility point where users register custom fields (e.g., `{MachineName}`, `{ProcessId}`) via a delegate or interface? This trades simplicity for power. The current design intentionally keeps the field set closed.

4. **Template composition** — Should a formatter template be composable from sub-templates? For example, defining a `{Header}` sub-template and reusing it. This is likely over-engineering for the initial release.

5. **Additional `init` properties** — Beyond `LevelFormat` and `TimestampFormat`, are there other field-level presentation options that warrant runtime configurability? Candidates: EventId format, Exception format, Scope/Properties separators, Thread display. Each adds an `init` property and a branch in the generated code.
