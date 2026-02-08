# Benchmarks

`XenoAtom.Logging.Benchmark` uses BenchmarkDotNet to compare `XenoAtom.Logging` with:

- `Microsoft.Extensions.Logging`
- `ZLogger`
- `ZeroLog`
- `Serilog`

## Project location

- Benchmark project: `src/XenoAtom.Logging.Benchmark/`
- Benchmark classes:
  - `src/XenoAtom.Logging.Benchmark/LoggingSyncBenchmarks.cs`
  - `src/XenoAtom.Logging.Benchmark/LoggingAsyncBenchmarks.cs`

## Run

From repository root:

```sh
dotnet run --project src/XenoAtom.Logging.Benchmark/XenoAtom.Logging.Benchmark.csproj -c Release
```

Run only one scenario category:

```sh
dotnet run --project src/XenoAtom.Logging.Benchmark/XenoAtom.Logging.Benchmark.csproj -c Release -- --anyCategories EnabledStructured
```

Run one benchmark method:

```sh
dotnet run --project src/XenoAtom.Logging.Benchmark/XenoAtom.Logging.Benchmark.csproj -c Release -- --filter *XenoAtom_Enabled_Structured*
```

Run only async-consumer scenarios:

```sh
dotnet run --project src/XenoAtom.Logging.Benchmark/XenoAtom.Logging.Benchmark.csproj -c Release -- --anyCategories AsyncEnabledStructured AsyncEnabledException
```

Run preset suites (implemented in `src/XenoAtom.Logging.Benchmark/Program.cs`):

```sh
dotnet run --project src/XenoAtom.Logging.Benchmark/XenoAtom.Logging.Benchmark.csproj -c Release -- --suite comparison
dotnet run --project src/XenoAtom.Logging.Benchmark/XenoAtom.Logging.Benchmark.csproj -c Release -- --suite async
```

List available suites/aliases:

```sh
dotnet run --project src/XenoAtom.Logging.Benchmark/XenoAtom.Logging.Benchmark.csproj -c Release -- --list-suites
```

## Scenarios

Benchmarks are grouped by category:

- `Disabled`
- `EnabledSimple`
- `EnabledStructured`
- `EnabledException`
- `AsyncEnabledStructured`
- `AsyncEnabledException`

Preset suite names:

- `comparison` (aliases: `sync`)
- `async`

Each category measures the same logical logging operation for each participating library.

Benchmark classes use explicit `SimpleJob` settings to keep runs stable and avoid unbounded auto-scaling of operations per iteration.

## Fairness strategy

To keep comparisons fair:

- No filesystem or console I/O is performed.
- Each library is configured with an in-memory/no-op sink.
- Enabled benchmarks still force each sink to consume the rendered message payload, so we measure logging + message materialization, not just level checks.
- Disabled benchmarks use the same category names and thresholds across all libraries (`Bench.Enabled` and `Bench.Disabled`).
- Async categories include only libraries with native asynchronous infrastructure and route payload lengths through an asynchronous consumer queue to compare call-site overhead under asynchronous sink pressure.
- Async consumer queues are bounded and drop on overflow to prevent unbounded memory growth from benchmark harness backpressure.

## Notes

- Different libraries have different internal architectures (sync vs async pipelines, template handling, interpolation handlers, etc.); results should be interpreted as *end-to-end call cost under equivalent benchmark constraints*, not as an absolute universal ranking.
- Run benchmarks on an idle machine and repeat multiple times before drawing conclusions.

## Latest snapshot (2026-02-09)

- Sync suite (`LoggingSyncBenchmarks`): `XenoAtom.Logging` is in the top tier and generally comparable to `ZeroLog` while staying allocation-free in hot paths.
- Async suite (`LoggingAsyncBenchmarks`): `XenoAtom.Logging` remains competitive but still trails `ZeroLog` in some categories; ongoing work targets queue and consumer-path overhead reductions.
- Allocation profile: enabled sync paths are near-zero allocation for `XenoAtom.Logging`; async paths should stay bounded and are tracked with dedicated regression tests.
