# Filtering and Routing

`XenoAtom.Logging` supports two complementary routing mechanisms:

- Logger hierarchy configuration (`LogManagerConfig.RootLogger` + `LogManagerConfig.Loggers`)
- Per-writer filters (`AcceptFilters` / `RejectFilters`)

Use hierarchy first for coarse routing and filters for precise predicates.

## 1. Hierarchy routing

Every logger inherits from `RootLogger`, then applies matching named logger configs in order of specificity.

```csharp
using XenoAtom.Logging;
using XenoAtom.Logging.Writers;

var fileWriter = new FileLogWriter("logs/app.log");
var jsonWriter = new JsonFileLogWriter("logs/app.jsonl");

var config = new LogManagerConfig
{
    RootLogger =
    {
        MinimumLevel = LogLevel.Info,
        Writers = { fileWriter }
    }
};

// "App.Http" and children become Debug and fan out to both writers.
config.Loggers.Add("App.Http", LogLevel.Debug, [fileWriter, jsonWriter], includeParents: true);
```

## 2. includeParents behavior

`includeParents` controls whether parent/root writers are inherited.

- `true`: append writers from parent and current config.
- `false`: replace inherited writers with current config writers.

```csharp
config.Loggers.Add("App.Audit", LogLevel.Info, [jsonWriter], includeParents: false);
```

In this example, `App.Audit.*` writes only to `jsonWriter`.

## 3. Writer filters

Each `LogWriter` has:

- `RejectFilters`: applied first, any match rejects immediately.
- `AcceptFilters`: if non-empty, at least one filter must match.

```csharp
fileWriter.RejectFilters.Add(static m =>
    m.Logger.Name.StartsWith("App.Noisy", StringComparison.Ordinal));

fileWriter.AcceptFilters.Add(static m =>
    m.Level >= LogLevel.Warn);
```

## 4. Applying runtime updates safely

Configuration collections are mutable. Update them from one thread, then call:

```csharp
config.ApplyChanges();
```

Do not mutate writer filter collections concurrently with high logging traffic.

## 5. Common patterns

- Route errors from all categories to a durable file writer.
- Route high-volume categories to JSON only.
- Keep terminal output concise with `MinimumLevel = LogLevel.Info`.
- Apply reject filters to suppress known noisy categories.

See also:

- `doc/thread-safety.md`
- `doc/file-writer.md`
- `doc/readme.md`

