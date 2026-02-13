# XenoAtom.Logging [![ci](https://github.com/XenoAtom/XenoAtom.Logging/actions/workflows/ci.yml/badge.svg)](https://github.com/XenoAtom/XenoAtom.Logging/actions/workflows/ci.yml) [![NuGet](https://img.shields.io/nuget/v/XenoAtom.Logging.svg)](https://www.nuget.org/packages/XenoAtom.Logging/)

<img align="right" width="256px" height="256px" src="https://raw.githubusercontent.com/XenoAtom/XenoAtom.Logging/main/img/XenoAtom.Logging.png">

XenoAtom.Logging is a high-performance structured logging runtime for .NET, designed for **zero allocations on the hot path** and predictable throughput in both sync and async modes.
It includes a high-efficiency interpolated logging API, structured properties/scopes, source-generated formatters, and production-grade file/JSON sinks.

> [!NOTE]
> This library is pre-1.0. The API is largely stable but may still see breaking changes.

## Requirements (.NET 10 / C# 14)

XenoAtom.Logging targets `net10.0` and requires the .NET 10 SDK (C# 14).

## Installation

```sh
dotnet add package XenoAtom.Logging
dotnet add package XenoAtom.Logging.Terminal
```

## ✨ Features

- **Performance-first (zero allocations on the hot path)**:
  - Allocation-aware interpolated-string handlers (`Trace`/`Debug`/`Info`/`Warn`/`Error`/`Fatal`)
  - Formatting into `Span<char>` with pooled buffers and optional segment metadata
- **Sync by default, async when you need it**:
  - Default mode is synchronous
  - Optional asynchronous mode with bounded queue and overflow policy (`Drop`, `DropAndNotify`, `Block`, `Allocate`)
- **Structured logging**:
  - Per-message properties (`LogProperties`)
  - Scopes (`BeginScope`) captured as snapshots
- **Template-driven text formatting (source generation)**:
  - `LogFormatter` base class with shared settings (`LevelFormat`, `TimestampFormat`)
  - Generated template formatters (e.g. `StandardLogFormatter`) and segment kinds for rich sinks
- **Terminal integration without `System.Console`**:
  - `XenoAtom.Logging.Terminal` uses `XenoAtom.Terminal` for markup-aware output
  - `TerminalLogControlWriter` targets `XenoAtom.Terminal.UI.Controls.LogControl` for fullscreen/log-viewer apps
  - **Visual attachments**: log calls can attach `XenoAtom.Terminal.UI.Visual` (tables, layouts, rich widgets)
  - Terminal docs: https://xenoatom.github.io/terminal
- **Production file and JSON sinks**:
  - Rolling + retention (`FileLogWriter`, `JsonFileLogWriter`)
  - Failure policies and durability options
- **Operational support**:
  - Async error callback via `LogManagerConfig.AsyncErrorHandler`
  - Runtime diagnostics via `LogManager.GetDiagnostics()` (`DroppedMessages`, `ErrorCount`)
  - NativeAOT and trimming oriented (`IsAotCompatible`, `IsTrimmable`)

![Screenshot](https://raw.githubusercontent.com/XenoAtom/XenoAtom.Logging/main/img/screenshot.png)

And the integration with LogControl:

![Integration with LogControl](https://raw.githubusercontent.com/XenoAtom/XenoAtom.Logging/main/img/xenoatom-logcontrol.gif)

> [!NOTE]
> XenoAtom.Logging does not aim to be compatible with `Microsoft.Extensions.Logging` today. A bridge may be added later, but the runtime is designed to stand on its own.

## Package layout

- `XenoAtom.Logging`: core runtime, formatters, stream/file/JSON writers
- `XenoAtom.Logging` also ships the generators/analyzers in-package (`analyzers/dotnet/cs`)
- `XenoAtom.Logging.Terminal`: terminal sink using `XenoAtom.Terminal` and `XenoAtom.Terminal.UI`

## 🚀 Quick start

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
            new FileLogWriter(
                new FileLogWriterOptions("logs/app.log")
                {
                    FileSizeLimitBytes = 10 * 1024 * 1024,
                    RollingInterval = FileRollingInterval.Daily,
                    RetainedFileCountLimit = 7
                })
        }
    }
};

LogManager.Initialize(config); // sync processor by default

var logger = LogManager.GetLogger("Sample");
logger.Info($"Hello {42}");

LogManager.Shutdown();
```

Enable async processing:

```csharp
config.AsyncErrorHandler = exception =>
{
    Console.Error.WriteLine($"[logging async error] {exception}");
};

LogManager.InitializeForAsync(config);
```

## 🖥️ Terminal markup and visuals

Markup payload logging (terminal sink):

```csharp
logger.InfoMarkup("[green]ready[/]");
logger.ErrorMarkup($"[bold red]failed[/] id={requestId}");
```

Visual attachments via `XenoAtom.Terminal.UI` (rendered under the log line by `TerminalLogWriter`):

```csharp
using XenoAtom.Terminal.UI;
using XenoAtom.Terminal.UI.Controls;

var table = new Table()
    .Headers("Step", "Status", "Duration")
    .AddRow("Initialize", "OK", "00:00.045")
    .AddRow("ProcessRequest", "FAILED", "00:00.003");

logger.Info(table, "Run summary");
logger.InfoMarkup(table, "[bold]Run summary (styled)[/]");
```

`TerminalLogWriter` and `TerminalLogControlWriter` both expose `Styles` and `SegmentStyleResolver` for per-segment and per-level styling.

## Thread safety

- `LogManager` and `Logger` are safe for concurrent logging.
- Configure `LogManagerConfig`, `LoggerConfig.Writers`, and writer filter collections from a single thread, then call `ApplyChanges()` when done.
- `LogProperties` is a mutable value type; avoid copying populated instances and dispose only the owner instance.
- In sync mode, writer exceptions propagate to callers; in async mode, use `AsyncErrorHandler` + diagnostics to observe failures.

See [`site/docs/thread-safety.md`](site/docs/thread-safety.md) for detailed guidance.

## 📖 Documentation

- User guide: [`site/docs/readme.md`](site/docs/readme.md)
- Template-based log formatters: [`site/docs/log-formatter.md`](site/docs/log-formatter.md)
- Terminal sink and visuals: [`site/docs/terminal.md`](site/docs/terminal.md), [`site/docs/terminal-visuals.md`](site/docs/terminal-visuals.md)
- File and JSON writers: [`site/docs/file-writer.md`](site/docs/file-writer.md)
- Filtering and routing: [`site/docs/filtering.md`](site/docs/filtering.md)
- Shutdown and flushing: [`site/docs/shutdown.md`](site/docs/shutdown.md)
- Native AOT and trimming: [`site/docs/aot.md`](site/docs/aot.md)
- Package metadata and consumption notes: [`site/docs/packages.md`](site/docs/packages.md)
- Benchmarks: [`site/docs/benchmarks.md`](site/docs/benchmarks.md)
- Source-generated logging (`[LogMethod]`): [`site/docs/source-generator.md`](site/docs/source-generator.md)
- Samples: [`samples/readme.md`](samples/readme.md)
  - Includes `HelloLogControl` (fullscreen `LogControl` + background logging demo)

## 🪪 License

This software is released under the [BSD-2-Clause license](https://opensource.org/licenses/BSD-2-Clause).

## 🤗 Author

Alexandre Mutel aka [xoofx](https://xoofx.github.io).
