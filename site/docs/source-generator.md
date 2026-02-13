---
title: "Source-generated Logging"
---

# Source-generated logging

`XenoAtom.Logging` includes source generators and analyzers embedded in the core package.

This generator covers two features:

- `[LogMethod]`: generates strongly-typed logging methods from message templates.
- `[LogFormatter]`: generates high-performance text formatters from declarative templates.

## Attribute-based API

Declare static partial methods with `[LogMethod]`:

```csharp
using XenoAtom.Logging;

public static partial class AppLogs
{
    [LogMethod(LogLevel.Info, "User {userId} connected from {ip}")]
    public static partial void UserConnected(Logger logger, int userId, string ip);

    [LogMethod(LogLevel.Error, "Request {requestId} failed", EventId = 42, EventName = "RequestFailed")]
    public static partial void RequestFailed(Logger logger, Exception exception, LogProperties properties, int requestId);
}
```

The generator emits method implementations that:

- perform a level check (`logger.IsEnabled(level)`)
- construct optional `LogEventId`
- call the matching generated `LoggerExtensions` level method
- emit interpolation code derived from the compile-time template

## Message template rules

- Placeholder syntax: `{name}`, `{name,alignment}`, `{name:format}`, `{name,alignment:format}`
- Escaped braces: write two opening braces (`{` then `{`) or two closing braces (`}` then `}`).
- Placeholder names must match method parameter names (case-insensitive)

## Analyzer diagnostics

- `XLG0100`: placeholder parameter type may allocate during generated logging.
  - Preferred types: `string`, `bool`, unmanaged `ISpanFormattable` values.

## Generator diagnostics

- `XLG0001`: invalid `[LogMethod]` signature
- `XLG0002`: unsupported log level
- `XLG0003`: invalid message template
- `XLG0004`: unknown template parameter

## Log formatters

To create custom text formatters, use `[LogFormatter]` on a `partial record` inheriting `LogFormatter`:

```csharp
using XenoAtom.Logging;

[LogFormatter("{Timestamp:HH:mm:ss} {Level,-5} {LoggerName} {Text}{? | {Exception}?}")]
public sealed partial record MyFormatter : LogFormatter;
```

See [Log Formatters](log-formatter.md) for usage, template quick reference, and generator setup guidance.
