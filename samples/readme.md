# Samples

## HelloLogging

`HelloLogging` demonstrates:

- `XenoAtom.Logging.Terminal` rich segment styling
- markup logging APIs (`InfoMarkup`, `WarnMarkup`, `ErrorMarkup`, `FatalMarkup`)
- visual attachments with `XenoAtom.Terminal.UI` (`Table` attached to log messages)
- scoped and per-message `LogProperties`
- `LogEventId` and exception logging

Run it from the repository root:

```sh
dotnet run --project samples/HelloLogging/HelloLogging.csproj -c Release
```

## FileJsonLogging

`FileJsonLogging` demonstrates:

- text logging with `FileLogWriter`
- JSON lines logging with `JsonFileLogWriter`
- rolling and retention options
- structured properties, scopes, event IDs, and exception logging

Run it from the repository root:

```sh
dotnet run --project samples/FileJsonLogging/FileJsonLogging.csproj -c Release
```
