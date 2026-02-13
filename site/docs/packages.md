---
title: "Package Consumption Guide"
---

# Package Consumption Guide

`XenoAtom.Logging` is split into focused packages.

## Core runtime

Install:

```sh
dotnet add package XenoAtom.Logging
```

Includes:

- `LogManager`, `Logger`, `LogProperties`, scopes
- Sync/async processors
- Built-in formatters
- Stream/file/json writers
- Embedded source generators/analyzers (`[LogMethod]`, `[LogFormatter]`)

## Terminal sink

Install:

```sh
dotnet add package XenoAtom.Logging.Terminal
```

Includes:

- `TerminalLogWriter`
- Markup logging extensions (`InfoMarkup`, `ErrorMarkup`, etc.)
- Rich segment styling integration with `XenoAtom.Terminal`

## Typical combinations

- Server/service: `XenoAtom.Logging`
- CLI/console app: add `XenoAtom.Logging.Terminal`
- JSON ingestion pipeline: `XenoAtom.Logging` with `JsonFileLogWriter`

## Verify package setup

After restore/build, generated members should appear in IDE for:

- `[LogMethod]` partial methods
- `[LogFormatter]` partial records/properties

If not, verify analyzer wiring in your project file.
If your project disables transitive analyzers, re-enable analyzers for `XenoAtom.Logging` package references.

See also:

- [Source-generated Logging](source-generator.md)
- [Log Formatters](log-formatter.md)
- [Terminal Integration](terminal.md)
- [Native AOT and Trimming](aot.md)
