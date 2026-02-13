---
title: "Benchmarks"
---

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
> Do not treat these results as an absolute ranking. The goal is to verify that `XenoAtom.Logging` stays in the right performance tier, remains zero-allocation on targeted paths, and tracks progress over time. Libraries make different architectural trade-offs, so interpret end-to-end costs within these benchmark constraints.

### Sync suite (`LoggingSyncBenchmarks`)

```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.7623/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 7950X 4.50GHz, 1 CPU, 32 logical and 16 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.2 (10.0.2, 10.0.225.61305), X64 RyuJIT x86-64-v4


```

{.table}
| Method                                             | Categories                   | Mean        | Error     | StdDev    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|--------------------------------------------------- |----------------------------- |------------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
| XenoAtom_Disabled                                  | Disabled                     |   1.6509 ns | 0.0351 ns | 0.0328 ns |  1.00 |    0.03 |      - |      - |         - |          NA |
| MicrosoftExtensions_Disabled                       | Disabled                     |  25.1582 ns | 0.1270 ns | 0.1188 ns | 15.24 |    0.30 | 0.0091 |      - |     152 B |          NA |
| ZLogger_Disabled                                   | Disabled                     |   3.0154 ns | 0.0152 ns | 0.0142 ns |  1.83 |    0.04 |      - |      - |         - |          NA |
| ZeroLog_Disabled                                   | Disabled                     |   0.3532 ns | 0.0051 ns | 0.0048 ns |  0.21 |    0.00 |      - |      - |         - |          NA |
| Serilog_Disabled                                   | Disabled                     |  10.8909 ns | 0.0894 ns | 0.0836 ns |  6.60 |    0.13 | 0.0091 |      - |     152 B |          NA |
|                                                    |                              |             |           |           |       |         |        |        |           |             |
| XenoAtom_Enabled_Exception                         | EnabledException             |  46.9235 ns | 0.2496 ns | 0.2335 ns |  1.00 |    0.01 |      - |      - |         - |          NA |
| MicrosoftExtensions_Enabled_Exception              | EnabledException             |  52.0513 ns | 0.1408 ns | 0.1099 ns |  1.11 |    0.01 | 0.0067 |      - |     112 B |          NA |
| ZLogger_Enabled_Exception                          | EnabledException             | 139.3819 ns | 0.9624 ns | 0.9002 ns |  2.97 |    0.02 | 0.0114 |      - |     192 B |          NA |
| ZeroLog_Enabled_Exception                          | EnabledException             |  54.9794 ns | 0.1938 ns | 0.1812 ns |  1.17 |    0.01 |      - |      - |         - |          NA |
| Serilog_Enabled_Exception                          | EnabledException             | 131.6097 ns | 0.7872 ns | 0.6574 ns |  2.80 |    0.02 | 0.0281 |      - |     472 B |          NA |
|                                                    |                              |             |           |           |       |         |        |        |           |             |
| XenoAtom_Enabled_Generated_Exception               | EnabledGeneratedException    |  46.2118 ns | 0.2475 ns | 0.2315 ns |  1.00 |    0.01 |      - |      - |         - |          NA |
| MicrosoftExtensions_Enabled_Generated_Exception    | EnabledGeneratedException    |  34.4092 ns | 0.6413 ns | 0.5999 ns |  0.74 |    0.01 | 0.0057 |      - |      96 B |          NA |
|                                                    |                              |             |           |           |       |         |        |        |           |             |
| XenoAtom_Enabled_Generated_Simple                  | EnabledGeneratedSimple       |  48.3564 ns | 0.6064 ns | 0.5064 ns |  1.00 |    0.01 |      - |      - |         - |          NA |
| MicrosoftExtensions_Enabled_Generated_Simple       | EnabledGeneratedSimple       |  35.0225 ns | 0.5339 ns | 0.4994 ns |  0.72 |    0.01 | 0.0048 |      - |      80 B |          NA |
|                                                    |                              |             |           |           |       |         |        |        |           |             |
| XenoAtom_Enabled_Simple_Interpolated               | EnabledSimpleInterpolated    |  45.3146 ns | 0.2983 ns | 0.2491 ns |  1.00 |    0.01 |      - |      - |         - |          NA |
| MicrosoftExtensions_Enabled_Simple_Interpolated    | EnabledSimpleInterpolated    |  30.0527 ns | 0.4310 ns | 0.3599 ns |  0.66 |    0.01 | 0.0048 |      - |      80 B |          NA |
| ZLogger_Enabled_Simple_Interpolated                | EnabledSimpleInterpolated    | 110.0859 ns | 0.9901 ns | 0.8777 ns |  2.43 |    0.02 | 0.0048 |      - |      80 B |          NA |
| ZeroLog_Enabled_Simple_Interpolated                | EnabledSimpleInterpolated    |  50.1072 ns | 0.9032 ns | 0.8006 ns |  1.11 |    0.02 |      - |      - |         - |          NA |
| Serilog_Enabled_Simple_Interpolated                | EnabledSimpleInterpolated    | 178.3583 ns | 1.4699 ns | 1.3749 ns |  3.94 |    0.04 | 0.0329 | 0.0098 |     552 B |          NA |
|                                                    |                              |             |           |           |       |         |        |        |           |             |
| XenoAtom_Enabled_Simple_NonInterpolated            | EnabledSimpleNonInterpolated |  37.8912 ns | 0.1701 ns | 0.1591 ns |  1.00 |    0.01 |      - |      - |         - |          NA |
| MicrosoftExtensions_Enabled_Simple_NonInterpolated | EnabledSimpleNonInterpolated |  13.7325 ns | 0.0289 ns | 0.0242 ns |  0.36 |    0.00 |      - |      - |         - |          NA |
| ZLogger_Enabled_Simple_NonInterpolated             | EnabledSimpleNonInterpolated |  82.1796 ns | 0.4524 ns | 0.4010 ns |  2.17 |    0.01 | 0.0072 |      - |     120 B |          NA |
| ZeroLog_Enabled_Simple_NonInterpolated             | EnabledSimpleNonInterpolated |  43.0691 ns | 0.2382 ns | 0.2228 ns |  1.14 |    0.01 |      - |      - |         - |          NA |
| Serilog_Enabled_Simple_NonInterpolated             | EnabledSimpleNonInterpolated |  84.0832 ns | 0.3738 ns | 0.3496 ns |  2.22 |    0.01 | 0.0138 |      - |     232 B |          NA |
|                                                    |                              |             |           |           |       |         |        |        |           |             |
| XenoAtom_Enabled_Structured                        | EnabledStructured            | 101.0461 ns | 0.9045 ns | 0.8018 ns |  1.00 |    0.01 |      - |      - |         - |          NA |
| MicrosoftExtensions_Enabled_Structured             | EnabledStructured            | 163.9980 ns | 0.6515 ns | 0.5776 ns |  1.62 |    0.01 | 0.0162 |      - |     272 B |          NA |
| ZLogger_Enabled_Structured                         | EnabledStructured            | 196.4248 ns | 1.0512 ns | 0.9833 ns |  1.94 |    0.02 | 0.0072 |      - |     120 B |          NA |
| ZeroLog_Enabled_Structured                         | EnabledStructured            | 115.4769 ns | 0.4402 ns | 0.4118 ns |  1.14 |    0.01 |      - |      - |         - |          NA |
| Serilog_Enabled_Structured                         | EnabledStructured            | 282.4018 ns | 1.5298 ns | 1.4309 ns |  2.79 |    0.03 | 0.0558 |      - |     936 B |          NA |

### Async suite (`LoggingAsyncBenchmarks`)

{.table}
| Method                            | Categories             | Mean     | Error   | StdDev  | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------------------------------- |----------------------- |---------:|--------:|--------:|------:|--------:|-------:|----------:|------------:|
| XenoAtom_Async_Enabled_Exception  | AsyncEnabledException  | 132.9 ns | 2.64 ns | 2.47 ns |  1.00 |    0.03 |      - |         - |          NA |
| ZLogger_Async_Enabled_Exception   | AsyncEnabledException  | 210.7 ns | 1.04 ns | 0.92 ns |  1.59 |    0.03 | 0.0114 |     192 B |          NA |
| ZeroLog_Async_Enabled_Exception   | AsyncEnabledException  | 123.5 ns | 1.53 ns | 1.43 ns |  0.93 |    0.02 |      - |         - |          NA |
|                                   |                        |          |         |         |       |         |        |           |             |
| XenoAtom_Async_Enabled_Structured | AsyncEnabledStructured | 183.5 ns | 1.16 ns | 1.09 ns |  1.00 |    0.01 |      - |         - |          NA |
| ZLogger_Async_Enabled_Structured  | AsyncEnabledStructured | 233.8 ns | 0.51 ns | 0.45 ns |  1.27 |    0.01 | 0.0072 |     120 B |          NA |
| ZeroLog_Async_Enabled_Structured  | AsyncEnabledStructured | 160.2 ns | 1.02 ns | 0.96 ns |  0.87 |    0.01 |      - |         - |          NA |


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
- `EnabledSimpleNonInterpolated`
- `EnabledSimpleInterpolated`
- `EnabledStructured`
- `EnabledGeneratedSimple`
- `EnabledException`
- `EnabledGeneratedException`
- `AsyncEnabledStructured`
- `AsyncEnabledException`

Preset suite names:

- `comparison` (aliases: `sync`)
- `async`

Each category measures the same logical logging operation for each participating library.

Benchmark classes use explicit `SimpleJob` settings to keep runs stable and avoid unbounded auto-scaling per iteration.

## Fairness strategy

To keep comparisons fair:

- No filesystem or console I/O is performed.
- Each library is configured with an in-memory/no-op sink.
- Enabled benchmarks still force each sink to consume the rendered message payload, so we measure logging + message materialization, not just level checks.
- Disabled benchmarks use the same category names and thresholds across all libraries (`Bench.Enabled` and `Bench.Disabled`).
- Async categories include only libraries with native asynchronous infrastructure and route payload lengths through an asynchronous consumer queue to compare call-site overhead under sink pressure.
- Async consumer queues are bounded and drop on overflow to prevent unbounded memory growth from benchmark harness backpressure.

## Notes

- Different libraries use different architectures (sync vs async pipelines, template handling, interpolation handlers, etc.); interpret results as *end-to-end call cost under equivalent constraints*, not as a universal ranking.
- Run benchmarks on an idle machine and repeat multiple times before drawing conclusions.
