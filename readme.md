# XenoAtom.Logging [![ci](https://github.com/XenoAtom/XenoAtom.Logging/actions/workflows/ci.yml/badge.svg)](https://github.com/XenoAtom/XenoAtom.Logging/actions/workflows/ci.yml) [![NuGet](https://img.shields.io/nuget/v/XenoAtom.Logging.svg)](https://www.nuget.org/packages/XenoAtom.Logging/)

<img align="right" width="160px" height="160px" src="https://raw.githubusercontent.com/XenoAtom/XenoAtom.Logging/main/img/XenoAtom.Logging.png">

`XenoAtom.Logging` is a high-performance structured logging runtime for .NET with an allocation-aware hot path and sync/async processing modes.

## Requirements

- .NET 10 SDK/runtime (`net10.0`)

## Installation

```sh
dotnet add package XenoAtom.Logging
dotnet add package XenoAtom.Logging.Terminal
```

## Features

- Interpolated-string handler based logging API (`Trace`, `Debug`, `Info`, `Warn`, `Error`, `Fatal`)
- Sync and async message processors with configurable overflow mode
- Runtime diagnostics via `LogManager.GetDiagnostics()`
- Scoped and per-message properties (`BeginScope`, `LogProperties`)
- Formatter + segment model for rich sinks
- Production file sinks (`FileLogWriter`, `JsonFileLogWriter`) with rolling and retention
- Dependency-minimal core package (`XenoAtom.Logging`)
- Terminal sink package (`XenoAtom.Logging.Terminal`) powered by `XenoAtom.Terminal`

## Package layout

- `XenoAtom.Logging`: core runtime, formatters, stream/file/json writers
- `XenoAtom.Logging` also ships the source generator/analyzers in-package (`analyzers/dotnet/cs`)
- `XenoAtom.Logging.Terminal`: terminal/ANSI sink using `XenoAtom.Terminal` (core library does not use `System.Console`)

## Quick start

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

LogManager.Initialize(config);

var logger = LogManager.GetLogger("Sample");
logger.Info($"Hello {42}");

LogManager.Shutdown();
```

With `XenoAtom.Logging.Terminal`, you can emit markup-aware messages:

```csharp
logger.InfoMarkup("[green]ready[/]");
logger.ErrorMarkup($"[red]failed[/] id={requestId}");
```

`TerminalLogWriter` also exposes `Styles` for per-segment and per-level rich style customization.

## Thread safety

- `LogManager` and `Logger` are safe for concurrent logging.
- Configure `LogManagerConfig`, `LoggerConfig.Writers`, and writer filter collections from a single thread, then call `ApplyChanges()` when done.
- `LogProperties` is a mutable value type; avoid copying populated instances and dispose only the owner instance.

See [`doc/thread-safety.md`](doc/thread-safety.md) for detailed guidance.

## Documentation

- User guide: [`doc/readme.md`](doc/readme.md)
- Changelog: [`CHANGELOG.md`](CHANGELOG.md)
- Package consumption: [`doc/packages.md`](doc/packages.md)
- Native AOT and trimming: [`doc/aot.md`](doc/aot.md)
- Filtering and routing: [`doc/filtering.md`](doc/filtering.md)
- Shutdown semantics: [`doc/shutdown.md`](doc/shutdown.md)
- File and JSON writers: [`doc/file-writer.md`](doc/file-writer.md)
- Benchmarks: [`doc/benchmarks.md`](doc/benchmarks.md)
- Source-generated logging: [`doc/source-generator.md`](doc/source-generator.md)
- Terminal sink: [`doc/terminal.md`](doc/terminal.md)
- Terminal visual examples: [`doc/terminal-visuals.md`](doc/terminal-visuals.md)
- Thread safety: [`doc/thread-safety.md`](doc/thread-safety.md)
- Samples: [`samples/readme.md`](samples/readme.md)

## License

This software is released under the [BSD-2-Clause license](https://opensource.org/licenses/BSD-2-Clause).

## Author

Alexandre Mutel aka [xoofx](https://xoofx.github.io).
