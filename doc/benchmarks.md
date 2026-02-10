# Benchmarks

`XenoAtom.Logging.Benchmark` uses BenchmarkDotNet to compare `XenoAtom.Logging` with:

- `Microsoft.Extensions.Logging` version `10.0.2`
- `ZLogger` version `2.5.10`
- `ZeroLog` version `2.6.0`
- `Serilog` version `4.3.0`

## Results

The following tables summarize benchmark results as of `2026-02-10`.

> [!WARNING]
> 
> These results should not be interpreted as an absolute ranking of libraries. The goal of these benchmarks was to make sure that `XenoAtom.Logging` is in the **right performance tier**, produces **zero allocations** and to track relative performance across versions as we optimize. Each library has different architectural trade-offs, so end-to-end call costs should be interpreted in the context of the specific benchmark scenarios and constraints.


### Sync suite (`LoggingSyncBenchmarks`)

```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7623/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 7950X 4.50GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v4


```

| Method                                 | Categories        | Mean        | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|--------------------------------------- |------------------ |------------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| XenoAtom_Disabled                      | Disabled          |   2.4268 ns | 0.0341 ns | 0.0319 ns |  1.00 |    0.02 |      - |         - |          NA |
| MicrosoftExtensions_Disabled           | Disabled          |  26.1401 ns | 0.4197 ns | 0.3926 ns | 10.77 |    0.21 | 0.0091 |     152 B |          NA |
| ZLogger_Disabled                       | Disabled          |   3.1866 ns | 0.0362 ns | 0.0321 ns |  1.31 |    0.02 |      - |         - |          NA |
| ZeroLog_Disabled                       | Disabled          |   0.3577 ns | 0.0041 ns | 0.0038 ns |  0.15 |    0.00 |      - |         - |          NA |
| Serilog_Disabled                       | Disabled          |  10.9688 ns | 0.0830 ns | 0.0776 ns |  4.52 |    0.06 | 0.0091 |     152 B |          NA |
|                                        |                   |             |           |           |       |         |        |           |             |
| XenoAtom_Enabled_Exception             | EnabledException  |  47.5075 ns | 0.1849 ns | 0.1730 ns |  1.00 |    0.00 |      - |         - |          NA |
| MicrosoftExtensions_Enabled_Exception  | EnabledException  |  53.4775 ns | 0.2848 ns | 0.2664 ns |  1.13 |    0.01 | 0.0067 |     112 B |          NA |
| ZLogger_Enabled_Exception              | EnabledException  | 147.8964 ns | 1.6686 ns | 1.4791 ns |  3.11 |    0.03 | 0.0114 |     192 B |          NA |
| ZeroLog_Enabled_Exception              | EnabledException  |  55.0767 ns | 0.2134 ns | 0.1892 ns |  1.16 |    0.01 |      - |         - |          NA |
| Serilog_Enabled_Exception              | EnabledException  | 134.0713 ns | 1.0655 ns | 0.9967 ns |  2.82 |    0.02 | 0.0281 |     472 B |          NA |
|                                        |                   |             |           |           |       |         |        |           |             |
| XenoAtom_Enabled_Simple                | EnabledSimple     |  38.1638 ns | 0.1734 ns | 0.1538 ns |  1.00 |    0.01 |      - |         - |          NA |
| MicrosoftExtensions_Enabled_Simple     | EnabledSimple     |  14.0463 ns | 0.1010 ns | 0.0944 ns |  0.37 |    0.00 |      - |         - |          NA |
| ZLogger_Enabled_Simple                 | EnabledSimple     |  96.2682 ns | 0.5662 ns | 0.5296 ns |  2.52 |    0.02 | 0.0043 |      72 B |          NA |
| ZeroLog_Enabled_Simple                 | EnabledSimple     |  42.8869 ns | 0.2245 ns | 0.1990 ns |  1.12 |    0.01 |      - |         - |          NA |
| Serilog_Enabled_Simple                 | EnabledSimple     |  86.2467 ns | 0.6199 ns | 0.5798 ns |  2.26 |    0.02 | 0.0138 |     232 B |          NA |
|                                        |                   |             |           |           |       |         |        |           |             |
| XenoAtom_Enabled_Structured            | EnabledStructured | 100.8397 ns | 0.5848 ns | 0.5470 ns |  1.00 |    0.01 |      - |         - |          NA |
| MicrosoftExtensions_Enabled_Structured | EnabledStructured | 165.9514 ns | 1.2676 ns | 1.1857 ns |  1.65 |    0.01 | 0.0162 |     272 B |          NA |
| ZLogger_Enabled_Structured             | EnabledStructured | 204.2272 ns | 0.8694 ns | 0.7707 ns |  2.03 |    0.01 | 0.0072 |     120 B |          NA |
| ZeroLog_Enabled_Structured             | EnabledStructured | 115.8277 ns | 0.5651 ns | 0.5009 ns |  1.15 |    0.01 |      - |         - |          NA |
| Serilog_Enabled_Structured             | EnabledStructured | 290.9576 ns | 2.4012 ns | 2.2461 ns |  2.89 |    0.03 | 0.0558 |     936 B |          NA |

### Async suite (`LoggingAsyncBenchmarks`)

| Method                            | Categories             | Mean     | Error   | StdDev  | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------------------------------- |----------------------- |---------:|--------:|--------:|------:|--------:|-------:|----------:|------------:|
| XenoAtom_Async_Enabled_Exception  | AsyncEnabledException  | 123.3 ns | 1.66 ns | 1.70 ns |  1.00 |    0.02 |      - |         - |          NA |
| ZLogger_Async_Enabled_Exception   | AsyncEnabledException  | 217.8 ns | 0.59 ns | 0.55 ns |  1.77 |    0.02 | 0.0114 |     192 B |          NA |
| ZeroLog_Async_Enabled_Exception   | AsyncEnabledException  | 123.9 ns | 0.76 ns | 0.71 ns |  1.01 |    0.01 |      - |         - |          NA |
|                                   |                        |          |         |         |       |         |        |           |             |
| XenoAtom_Async_Enabled_Structured | AsyncEnabledStructured | 167.8 ns | 1.28 ns | 1.20 ns |  1.00 |    0.01 |      - |         - |          NA |
| ZLogger_Async_Enabled_Structured  | AsyncEnabledStructured | 251.7 ns | 1.07 ns | 1.00 ns |  1.50 |    0.01 | 0.0072 |     120 B |          NA |
| ZeroLog_Async_Enabled_Structured  | AsyncEnabledStructured | 162.9 ns | 1.75 ns | 1.55 ns |  0.97 |    0.01 |      - |         - |          NA |


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
