# Native AOT and Trimming

`XenoAtom.Logging` is designed to work in Native AOT and trimming scenarios.

## Package settings

- `XenoAtom.Logging` is published as AOT-compatible and trimmable.
- `XenoAtom.Logging.Terminal` is also marked AOT-compatible and trimmable.
- `XenoAtom.Logging.Generators` is a development-time analyzer package.

## Recommended publish command

```sh
dotnet publish -c Release -r win-x64 -p:PublishAot=true -p:PublishTrimmed=true
```

Replace `win-x64` with your runtime identifier.

## Guidance

- Prefer source-generated APIs (`[LogMethod]`, `[LogFormatter]`) for predictable codegen.
- Avoid reflection-based custom sinks on the hot path.
- Keep custom formatters span-based (`TryFormat`) to avoid trimming surprises around serializer/runtime helpers.
- If your sink depends on external libraries, validate their trimming/AOT compatibility separately.

## Validation checklist

- Build and publish with `PublishAot=true`.
- Run startup/shutdown log paths.
- Run representative `Info`, `Warn`, `Error` logging with properties/scopes.
- Validate file/JSON/terminal sink output in published binaries.

