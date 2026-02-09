# Terminal Visuals & Generalized Attachments — Specification

**Date:** 2026-02-09  
**Status:** Draft  
**Scope:** Core library (`XenoAtom.Logging`) + terminal sink (`XenoAtom.Logging.Terminal`)

---

## 1. Motivation

### 1.1 Visuals in Log Messages

Currently, log messages are plain text (possibly with ANSI markup). The `XenoAtom.Terminal.UI` package provides rich visual controls (`Table`, `Markup`, `TextBlock`, etc.) that can be rendered to a terminal with a single call:

```csharp
Terminal.Write(table); // TerminalExtensions.Write(Visual)
```

We want to attach such visuals to log messages so that a terminal-aware writer renders both the formatted log line **and** the visual:

```csharp
var table = new Table()
    .Headers("Task", "Status")
    .AddRow("Download", "Running")
    .AddRow("Render", "OK");

log.Info(table, "This is an info message");
```

Expected terminal output:

```
2026-02-09 12:00:00.0000000 Info  MyApp This is an info message
┌──────────┬─────────┐
│ Task     │ Status  │
├──────────┼─────────┤
│ Download │ Running │
│ Render   │ OK      │
└──────────┴─────────┘
```

Writers that do not understand a particular attachment type simply ignore it.

### 1.2 Generalizing `Exception` as an Attachment

Today, `Exception` is a **first-class field** on `LogMessageInternal`:

- Threaded as a constructor parameter through the entire interpolated string handler chain.
- 16 overloads per log level in `Logger.gen.cs` (8 string, 8 interpolated) — half of which accept `Exception?`.
- Rendered by formatters (e.g., `StandardLogFormatter` appends `" | {Exception}"` conditionally).

From a design perspective, `Exception` is just one kind of **object that can be attached to a log message**. Tables, charts, progress bars, and other visuals are other kinds. Rather than adding another first-class field for each new attachment type, we generalize the concept.

### 1.3 Markup as a Core Concept

Currently, "is this message markup?" is signaled by injecting a hidden property `"__xenoatom.logging.terminal.markup" = "True"` into `LogProperties`. This has several problems:

- **Leaks to non-terminal writers**: `JsonLogFormatter` must filter out all `__xenoatom.logging.*` properties.
- **Impractical**: Every writer/formatter that processes properties must know about this convention.
- **Inconsistent**: Markup is a message-level attribute, not a key-value property.

The markup flag should be a first-class concept on the internal log message, not encoded in properties.

---

## 2. Design Overview

### 2.1 Two Distinct Changes

| Change | Layer | Impact |
|--------|-------|--------|
| **A. Generalized attachments** | Core (`LogMessageInternal`) | Replace `Exception?` field with a generalized `object?` attachment slot |
| **B. Markup flag** | Core (`LogMessageInternal`) | Add a `bool IsMarkup` field to the internal message |

Both changes are internal to the core library. The public `LogMessage` API is extended to expose them.

### 2.2 Attachment Model

A log message can have **zero or one** attachment. The attachment is an `object?` stored on `LogMessageInternal`.

- `Exception` is simply one type of attachment.
- `Visual` (from `XenoAtom.Terminal.UI`) is another.
- Future types are supported without changing the core.

A writer inspects the attachment type and handles it accordingly:

| Writer | `Exception` | `Visual` | Unknown |
|--------|-------------|----------|---------|
| `TerminalLogWriter` | Render via formatter (as today) | Call `Terminal.Write(visual)` | Ignore |
| `StreamLogWriter` | Render via formatter (as today) | Ignore (or `ToString()`) | Ignore |
| `FileLogWriter` | Render via formatter (as today) | Ignore (or `ToString()`) | Ignore |
| `JsonLogFormatter` | Emit `"exception"` field | Ignore (or emit `"attachment"` with `ToString()`) | Ignore |

---

## 3. Core Library Changes (`XenoAtom.Logging`)

### 3.1 `LogMessageInternal` — New Fields

```csharp
// Existing field (to be replaced):
// public Exception? Exception;

// New fields:
public object? Attachment;    // Generalized attachment (Exception, Visual, etc.)
public bool IsMarkup;         // Whether the Text payload contains markup
```

- `Exception` is no longer a separate field. An `Exception` is stored in `Attachment`.
- `IsMarkup` replaces the `__xenoatom.logging.terminal.markup` property convention.

### 3.2 `LogMessageInternal.Initialize()` — Signature Change

```csharp
// Current:
public void Initialize(Logger logger, LogLevel level, DateTimeOffset timestamp,
    LogEventId eventId, Thread thread, LogScopeSnapshot scope,
    LogPropertiesSnapshot? properties, Exception? exception,
    IFormatProvider? formatProvider)

// Proposed:
public void Initialize(Logger logger, LogLevel level, DateTimeOffset timestamp,
    LogEventId eventId, Thread thread, LogScopeSnapshot scope,
    LogPropertiesSnapshot? properties, object? attachment,
    bool isMarkup, IFormatProvider? formatProvider)
```

### 3.3 `LogMessageInternal.Reset()`

```csharp
public void Reset()
{
    // ... existing reset code ...
    Attachment = null;   // was: Exception = null;
    IsMarkup = false;
}
```

### 3.4 `LogMessage` — Public API Update

```csharp
public readonly ref struct LogMessage
{
    // Existing (replaced):
    // public Exception? Exception => _internalMessage.Exception;

    // New:
    /// <summary>
    /// Gets the attachment associated with this log message, or <see langword="null"/> if none.
    /// </summary>
    /// <remarks>
    /// Common attachment types include <see cref="System.Exception"/> and terminal visuals.
    /// Writers should handle known attachment types and ignore unknown ones.
    /// </remarks>
    public object? Attachment => _internalMessage.Attachment;

    /// <summary>
    /// Gets the exception associated with this log message, or <see langword="null"/> if no exception
    /// is attached.
    /// </summary>
    /// <remarks>
    /// This is a convenience property equivalent to <c>Attachment as Exception</c>.
    /// </remarks>
    public Exception? Exception => _internalMessage.Attachment as Exception;

    /// <summary>
    /// Gets a value indicating whether the <see cref="Text"/> payload contains markup tags
    /// that should be interpreted by markup-aware writers.
    /// </summary>
    public bool IsMarkup => _internalMessage.IsMarkup;
}
```

### 3.5 `InterpolatedLogMessageInternal` — Constructor Chain

All existing `Exception?` constructor overloads are replaced with `object? attachment`. The `Exception?`-typed parameter is removed entirely.

```csharp
// Replaces all Exception? constructors:
internal InterpolatedLogMessageInternal(Logger logger, LogLevel level,
    object? attachment, bool isMarkup, string msg)
    : this(logger, level, default, default, attachment, isMarkup, msg) { }

internal InterpolatedLogMessageInternal(Logger logger, LogLevel level,
    LogEventId eventId, object? attachment, bool isMarkup, string msg)
    : this(logger, level, eventId, default, attachment, isMarkup, msg) { }

// Full constructor:
internal InterpolatedLogMessageInternal(Logger logger, LogLevel level,
    LogEventId eventId, LogPropertiesSnapshot? properties,
    object? attachment, bool isMarkup, string msg) { ... }
```

The private initializer stores the attachment:

```csharp
private static void InitializeMessage(LogMessageInternal message,
    Logger logger, LogLevel level, LogEventId eventId,
    LogPropertiesSnapshot? properties, object? attachment,
    bool isMarkup, int capacity)
{
    // ... existing code ...
    message.Initialize(logger, level, timestamp, eventId, thread,
        scope, properties, attachment, isMarkup, formatProvider);
}
```

### 3.6 `Logger.gen.tt` / `Logger.gen.cs` — Unified Overloads

The existing `Exception?`-typed overloads are **removed** and replaced with a unified `object? attachment` parameter. The attachment is always placed **before** the message — this is required for interpolated overloads (so the handler can receive the attachment via its constructor) and is applied uniformly to string overloads for consistency.

For each log level, the full set of overloads is:

```csharp
// String-based (8 overloads per level):
public static void Info(this Logger logger, string msg)
public static void Info(this Logger logger, object? attachment, string msg)
public static void Info(this Logger logger, LogEventId eventId, string msg)
public static void Info(this Logger logger, LogEventId eventId, object? attachment, string msg)
public static void Info(this Logger logger, LogProperties properties, string msg)
public static void Info(this Logger logger, LogProperties properties, object? attachment, string msg)
public static void Info(this Logger logger, LogEventId eventId, LogProperties properties, string msg)
public static void Info(this Logger logger, LogEventId eventId, LogProperties properties, object? attachment, string msg)

// Interpolated (8 overloads per level):
public static void Info(this Logger logger,
    [InterpolatedStringHandlerArgument("logger")] ref InfoInterpolatedLogMessage message)
public static void Info(this Logger logger, object? attachment,
    [InterpolatedStringHandlerArgument("logger")] ref InfoInterpolatedLogMessage message)
public static void Info(this Logger logger, LogEventId eventId,
    [InterpolatedStringHandlerArgument("logger")] ref InfoInterpolatedLogMessage message)
public static void Info(this Logger logger, LogEventId eventId, object? attachment,
    [InterpolatedStringHandlerArgument("logger")] ref InfoInterpolatedLogMessage message)
// ... etc. with LogProperties combinations
```

Usage is uniform for all attachment types:

```csharp
// Exception — attachment-first, same as any other attachment:
log.Error(exception, "Failed to connect");
log.Error(exception, $"Failed to connect to {host}");

// Visual:
log.Info(table, "Status report");
log.Info(table, $"Status: {status}");

// No attachment:
log.Info("Simple message");
```

**Design note:** Interpolated string handlers require non-handler arguments to appear first so they can be forwarded to the handler's constructor. Placing the attachment before the message for string overloads too ensures a single consistent calling convention across the entire API.

### 3.7 Formatter Impact

#### 3.7.1 `StandardLogFormatter` (Source-Generated)

The `{Exception}` template field currently reads `logMessage.Exception`. It will continue to work because `LogMessage.Exception` is preserved as a computed property. No change needed.

#### 3.7.2 `JsonLogFormatter`

Currently emits `"exception": "..."` when `logMessage.Exception is not null`. This continues to work via the computed `Exception` property.

```csharp
if (logMessage.Exception is not null)
{
    // emit "exception" field as before
}
```

#### 3.7.3 Internal Property Filtering

`JsonLogFormatter.IsInternalPropertyName()` is **removed**. The `__xenoatom.logging.terminal.markup` property convention is deleted entirely — markup is now signaled via `IsMarkup`.

### 3.8 Remove Markup from Properties

After the `IsMarkup` field is added:

1. `LoggerMarkupExtensions.CreateMarkupProperties()` should set `IsMarkup = true` on the message directly instead of adding a property.
2. Since `LoggerMarkupExtensions` lives in `XenoAtom.Logging.Terminal` and doesn't have direct access to `LogMessageInternal`, the mechanism must go through the existing logging API.

**Approach**: Add mark overloads or a way to signal markup at the `Logger` extension level. Two options:

**Option A — Dedicated internal API**: Add an internal `Logger.LogMarkup(...)` method or overload that sets `IsMarkup = true`:

```csharp
// In Logger (core library):
internal void LogMarkup(in InterpolatedLogMessageInternal message)
{
    // Same as Log(), but message was created with isMarkup: true
    // The InterpolatedLogMessageInternal already carries the isMarkup flag
}
```

**Option B — Pass isMarkup through InterpolatedLogMessageInternal**: The `InterpolatedLogMessageInternal` already carries all initialization parameters. Add a `bool isMarkup` parameter to the constructor chain:

```csharp
// In LoggerMarkupExtensions:
private static void LogMarkupCore(Logger logger, LogLevel level,
    LogProperties properties, ReadOnlySpan<char> markupMessage)
{
    // Create InterpolatedLogMessageInternal with isMarkup: true
    var msg = new InterpolatedLogMessageInternal(logger, level,
        default, properties, null, isMarkup: true, markupMessage);
    logger.Log(in msg);
}
```

**Preferred: Option B** — It keeps the change minimal and follows the existing pattern where all message attributes flow through the constructor.

---

## 4. Terminal Library Changes (`XenoAtom.Logging.Terminal`)

### 4.1 `TerminalLogWriter` — Visual Rendering

When `Attachment` is a `Visual`, render it after the log line:

```csharp
protected override void Log(in LogMessage logMessage)
{
    // ... existing formatting and segment rendering ...

    // After writing the formatted log line:
    if (logMessage.Attachment is Visual visual)
    {
        Terminal.Write(visual);  // Uses TerminalExtensions.Write(Visual)
    }
}
```

`TerminalVisualWriter.Write(TerminalInstance, Visual)` handles measure → arrange → render → write, so the integration is a single call.

### 4.2 `TerminalLogWriter` — Markup Detection

Replace the property-based markup detection:

```csharp
// Current (remove):
var hasMarkupMessage = EnableMarkupMessages && ContainsMarkupPayload(logMessage.Properties);

// New:
var hasMarkupMessage = EnableMarkupMessages && logMessage.IsMarkup;
```

The `ContainsMarkupPayload()` helper and the `MarkupPropertyName`/`MarkupTrueValue` constants are removed from `TerminalLogWriter`.

### 4.3 `LoggerMarkupExtensions` — Simplification

The markup extensions no longer need to inject a hidden property:

```csharp
// Current:
private static LogProperties CreateMarkupProperties()
{
    var properties = new LogProperties();
    properties.Add(MarkupPropertyName, true);
    return properties;
}

// New: no property creation needed.
// Instead, LogMarkupCore creates a message with isMarkup: true.
```

This eliminates:
- The `LogProperties` allocation for the markup tag.
- The merge logic in `CreateMarkupProperties(LogProperties)`.
- The `IsInternalPropertyName()` filtering in `JsonLogFormatter`.

### 4.4 `LoggerMarkupExtensions` — Attachment Support

Add overloads that accept both markup and an attachment (e.g., a visual):

```csharp
public static void InfoMarkup(this Logger logger, object? attachment, string markupMessage)
    => logger.LogMarkup(LogLevel.Info, attachment, markupMessage);

// Example usage:
log.InfoMarkup(summaryTable, "[green]Build complete[/]");
```

---

## 5. Markup Stripping for Non-Terminal Writers

### 5.1 Problem

When a message is marked as markup (`IsMarkup = true`), non-terminal writers (file, stream, JSON) receive raw markup text like `[green]ok[/]`. They need to strip markup tags to produce plain text (`ok`).

### 5.2 Approach: Core-Library `MarkupStripper`

The core library provides a fast, zero-dependency markup stripper. The markup syntax is simple (`[tag]`, `[/]`, `[[` escape, `]]` escape), so a hand-written parser is straightforward and avoids adding `XenoAtom.Ansi` as a core dependency.

```csharp
/// <summary>
/// Strips markup tags from text, producing plain text output.
/// </summary>
/// <remarks>
/// Handles: [tag] → removed, [/] → removed, [[ → [, ]] → ].
/// </remarks>
internal static class MarkupStripper
{
    /// <summary>
    /// Strips markup tags from <paramref name="markup"/> and writes the plain text
    /// into <paramref name="destination"/>.
    /// </summary>
    /// <returns>The number of characters written.</returns>
    public static int Strip(ReadOnlySpan<char> markup, Span<char> destination)
    {
        // Hand-written parser:
        // 1. Scan for '['
        // 2. If next char is '[', emit literal '[' and skip both
        // 3. Otherwise, find matching ']' and skip the entire tag
        // 4. If ']' followed by ']', emit literal ']' and skip both
        // 5. Copy all other characters verbatim
    }

    /// <summary>
    /// Returns the maximum possible output length for a given markup input.
    /// </summary>
    /// <remarks>
    /// The stripped output is always ≤ the input length, so the input length
    /// is a safe upper bound.
    /// </remarks>
    public static int GetMaxOutputLength(int markupLength) => markupLength;
}
```

Key properties:
- **Zero allocation**: operates on `Span<char>`, no `string` creation in the hot path.
- **Output ≤ input**: stripping only removes characters, so the input length is always a safe destination size.
- **No dependencies**: pure character parsing, no `XenoAtom.Ansi` reference.

### 5.3 Integration

Writers that do not support markup call the stripper when `IsMarkup` is set:

```csharp
// In StreamLogWriter / FileLogWriter:
protected override void Log(in LogMessage logMessage)
{
    var text = formatterBuffer.Format(logMessage, Formatter, ref segments);

    if (logMessage.IsMarkup)
    {
        // Strip in-place into a rented buffer
        var buffer = ArrayPool<char>.Shared.Rent(text.Length);
        var written = MarkupStripper.Strip(text, buffer);
        text = buffer.AsSpan(0, written);
        // ... write text, then return buffer ...
    }

    // ... write text ...
}
```

This is **not opt-in per writer** — all non-markup-aware writers strip by default. The `TerminalLogWriter` does not strip because it passes markup through to the terminal.

---

## 6. API Summary

### 6.1 Public API Surface

```csharp
// LogMessage (core):
public readonly ref struct LogMessage
{
    public object? Attachment { get; }         // replaces Exception
    public Exception? Exception { get; }       // convenience (Attachment as Exception)
    public bool IsMarkup { get; }              // replaces property-based signaling
}

// Logger extension overloads (generated, per level × 8 string + 8 interpolated):
public static void Info(this Logger logger, string msg)
public static void Info(this Logger logger, object? attachment, string msg)
public static void Info(this Logger logger, LogEventId eventId, string msg)
public static void Info(this Logger logger, LogEventId eventId, object? attachment, string msg)
public static void Info(this Logger logger, LogProperties properties, string msg)
public static void Info(this Logger logger, LogProperties properties, object? attachment, string msg)
public static void Info(this Logger logger, LogEventId eventId, LogProperties properties, string msg)
public static void Info(this Logger logger, LogEventId eventId, LogProperties properties, object? attachment, string msg)
// ... same pattern for interpolated, and for all levels

// LoggerMarkupExtensions (Terminal):
public static void InfoMarkup(this Logger logger, string markupMessage)
public static void InfoMarkup(this Logger logger, object? attachment, string markupMessage)
// ... etc. for all levels
```

### 6.2 Internal Changes

```csharp
// LogMessageInternal:
internal object? Attachment;     // replaces Exception?
internal bool IsMarkup;          // replaces property-based signaling

// InterpolatedLogMessageInternal:
// All constructors take (object? attachment, bool isMarkup) instead of Exception?
```

---

## 7. Usage Examples

### 7.1 Logging with a Visual Attachment

```csharp
var log = LogManager.GetLogger("MyApp");

var table = new Table()
    .Headers("Task", "Status")
    .AddRow("Download", "Running")
    .AddRow("Render", "OK");

log.Info(table, "Build status report");
```

**Terminal output** (with `TerminalLogWriter`):

```
2026-02-09 12:00:00.0000000 Info  MyApp Build status report
┌──────────┬─────────┐
│ Task     │ Status  │
├──────────┼─────────┤
│ Download │ Running │
│ Render   │ OK      │
└──────────┴─────────┘
```

**File output** (with `FileLogWriter`): Attachment is ignored; only the text line is written.

**JSON output** (with `JsonLogFormatter`): Attachment is ignored (or optionally rendered as a `"attachment"` string field).

### 7.2 Logging with Markup

```csharp
log.InfoMarkup("[green]Build succeeded[/] in [bold]2.3s[/]");
```

**Terminal output**: Rendered with ANSI colors.  
**File output**: `Build succeeded in 2.3s` (stripped).  
**JSON output**: `"message": "Build succeeded in 2.3s"` (stripped).

### 7.3 Markup + Visual Attachment

```csharp
log.InfoMarkup(statusTable, "[bold]Deployment complete[/]");
```

**Terminal output**: Styled log line + rendered table below.

### 7.4 Exception

```csharp
try { ... }
catch (Exception ex)
{
    log.Error(ex, "Operation failed");
    log.Error(ex, $"Operation {name} failed");
}
```

### 7.5 Interpolated + Attachment

```csharp
log.Info(table, $"Processed {count} items in {elapsed.TotalSeconds:F1}s");
```

---

## 8. Implementation Plan

### Phase 1 — Core: Attachment + IsMarkup (Core Library)

1. Replace `Exception?` field with `object? Attachment` and add `bool IsMarkup` on `LogMessageInternal`.
2. Update `Initialize()` and `Reset()`.
3. Update `LogMessage` public API (`Attachment`, `Exception` computed, `IsMarkup`).
4. Replace all `Exception?` parameters with `(object? attachment, bool isMarkup)` in `InterpolatedLogMessageInternal` constructor chain.
5. Remove `JsonLogFormatter.IsInternalPropertyName()` and the `__xenoatom.logging.*` property filtering.

### Phase 2 — Overload Rewrite (Code Generation)

1. Update `Logger.gen.tt` to replace `Exception?` overloads with unified `object? attachment` overloads (attachment-first for both string and interpolated).
2. Regenerate `Logger.gen.cs`.
3. Update interpolated string handler types to accept `object? attachment`.

### Phase 3 — Markup Refactor (Terminal Library)

1. Rewrite `LoggerMarkupExtensions` to set `IsMarkup` directly instead of injecting properties.
2. Remove `MarkupPropertyName`, `CreateMarkupProperties()`, `ContainsMarkupPayload()`.
3. Update `TerminalLogWriter` to use `logMessage.IsMarkup`.

### Phase 4 — Visual Attachment (Terminal Library)

1. Add `Visual` detection in `TerminalLogWriter.Log()`.
2. Call `Terminal.Write(visual)` after the formatted log line.
3. Add attachment overloads to `LoggerMarkupExtensions`.

### Phase 5 — Markup Stripping (Core Library)

1. Implement `MarkupStripper.Strip(ReadOnlySpan<char>, Span<char>)` — fast hand-written parser, zero-alloc.
2. Integrate into `StreamLogWriter` and `FileLogWriter` for `IsMarkup` messages.
3. Integrate into `JsonLogFormatter` for the `"message"` field.

### Phase 6 — Tests & Documentation

1. Unit tests for `Attachment` lifecycle (set, read, reset, pool return).
2. Unit tests for `IsMarkup` flag propagation.
3. Unit tests for `MarkupStripper` (tags, escapes, nested, edge cases).
4. Integration test: `TerminalLogWriter` renders a `Visual` attachment.
5. Test exception-as-attachment: `log.Error("msg", exception)` round-trips.
6. Test markup stripping in file/stream/JSON writers.
7. Update `doc/terminal.md`, `doc/readme.md` with attachment/visual examples.

---

## 9. Open Questions

1. **Should `Attachment` be `object?` or a more specific base type?**  
   `object?` is the simplest and most flexible. A custom interface like `ILogAttachment` would allow type-safe dispatch but adds ceremony. Since the primary use cases are `Exception` (which we can't modify) and `Visual` (which we don't want to couple to the core), `object?` is pragmatic.

2. **Multiple attachments?**  
   For now, **one attachment per message** is sufficient. A future enhancement could allow a list, but the complexity is unjustified for the current use cases.

3. **Should attachment rendering be handled by the formatter or the writer?**  
   **Writer.** The formatter produces the text of the log line. The attachment is a supplementary visual that the writer renders after the formatted line. `Exception` rendering in the formatter is a special case: it's inlined into the text line because exceptions have a natural text representation. `Visual` rendering is inherently writer-specific (terminal vs file vs JSON).

4. **Should `IsMarkup` affect the log message text stored in the buffer, or only how writers interpret it?**  
   It is a **writer hint only**. The text buffer stores the raw markup text. Writers decide whether to strip or render with styles. This keeps the core simple and flexible.

5. **Should the `{Exception}` formatter template field be renamed to `{Attachment}`?**  
   No. `{Exception}` reads `logMessage.Exception` (computed as `Attachment as Exception`), which produces the exception text. A generic `{Attachment}` calling `ToString()` on arbitrary objects would produce unpredictable output. Keep `{Exception}` for exception formatting; visual attachments are rendered by the writer, not the formatter.
