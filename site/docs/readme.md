---
title: "User Guide"
---

# User Guide

![XenoAtom.Logging demo screenshot](../img/screenshot.png){.terminal}

Welcome to the XenoAtom.Logging documentation.

This section is organized as a practical map: start quickly, then go deeper by topic based on your use case.

## Start here

- [Getting Started](getting-started.md): install packages, configure logging, and run your first sync/async setup.
- [Migration from Microsoft.Extensions.Logging](microsoft-extensions-logging.md): API mapping and rollout guidance.
- [Package Consumption Guide](packages.md): package layout and when to use each package.

## Core runtime topics

- [Filtering and Routing](filtering.md): logger hierarchy, writer fan-out, and filters.
- [File and JSON Writers](file-writer.md): rolling files, retention, JSONL output, and failure handling.
- [Shutdown Semantics](shutdown.md): flushing and lifecycle behavior at app exit.
- [Thread Safety](thread-safety.md): what is safe concurrently and what must be configured carefully.

## Formatting and generated APIs

- [Log Formatters](log-formatter.md): built-in formatters, template syntax, and custom formatter generation.
- [Source-generated Logging](source-generator.md): `[LogMethod]` and diagnostics.

## Terminal and UI output

- [Terminal Integration](terminal.md): `TerminalLogWriter`, `TerminalLogControlWriter`, markup, and styling.
- [Terminal Visual Examples](terminal-visuals.md): rendered examples and styling walkthrough.

## Performance and deployment

- [Benchmarks](benchmarks.md): benchmark setup, scenarios, and interpretation guidance.
- [Native AOT and Trimming](aot.md): AOT/trimming-oriented behavior and constraints.

## Recommended reading order

1. [Getting Started](getting-started.md)
2. [Filtering and Routing](filtering.md)
3. [File and JSON Writers](file-writer.md)
4. [Log Formatters](log-formatter.md)
5. [Thread Safety](thread-safety.md)
6. [Shutdown Semantics](shutdown.md)
