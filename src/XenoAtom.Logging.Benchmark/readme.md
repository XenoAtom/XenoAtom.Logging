# XenoAtom.Logging.Benchmark

BenchmarkDotNet project comparing:

- `XenoAtom.Logging`
- `Microsoft.Extensions.Logging`
- `ZLogger`
- `ZeroLog`
- `Serilog`

Benchmark set includes:

- `LoggingSyncBenchmarks` (disabled + enabled sync-like scenarios)
- `LoggingAsyncBenchmarks` (enabled scenarios for libraries with native async pipelines)

Each class uses an explicit BenchmarkDotNet `SimpleJob` profile for stable runs.

Run:

```sh
dotnet run --project src/XenoAtom.Logging.Benchmark/XenoAtom.Logging.Benchmark.csproj -c Release
```

Run with preset suites:

```sh
dotnet run --project src/XenoAtom.Logging.Benchmark/XenoAtom.Logging.Benchmark.csproj -c Release -- --suite comparison
dotnet run --project src/XenoAtom.Logging.Benchmark/XenoAtom.Logging.Benchmark.csproj -c Release -- --suite async
```

List available suites and aliases:

```sh
dotnet run --project src/XenoAtom.Logging.Benchmark/XenoAtom.Logging.Benchmark.csproj -c Release -- --list-suites
```

See `site/docs/benchmarks.md` for scenario details and fairness notes.
