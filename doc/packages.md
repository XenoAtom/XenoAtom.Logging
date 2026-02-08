# Package Consumption Guide

`XenoAtom.Logging` is split into focused packages.

## Core runtime

Install:

```sh
dotnet add package XenoAtom.Logging
```

Includes:

- `LogManager`, `Logger`, `LogProperties`, scopes
- Sync/async processors
- Built-in formatters
- Stream/file/json writers

## Terminal sink

Install:

```sh
dotnet add package XenoAtom.Logging.Terminal
```

Includes:

- `TerminalLogWriter`
- Markup logging extensions (`InfoMarkup`, `ErrorMarkup`, etc.)
- Rich segment styling integration with `XenoAtom.Terminal`

## Source generators

Install (as analyzer dependency):

```sh
dotnet add package XenoAtom.Logging.Generators
```

In SDK-style projects, ensure it is referenced as an analyzer:

```xml
<ItemGroup>
  <PackageReference Include="XenoAtom.Logging.Generators" Version="*"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false"
                    PrivateAssets="all" />
</ItemGroup>
```

This enables:

- `[LogMethod]` generation
- `[LogFormatter]` generation

## Typical combinations

- Server/service: `XenoAtom.Logging` + `XenoAtom.Logging.Generators`
- CLI/console app: add `XenoAtom.Logging.Terminal`
- JSON ingestion pipeline: `XenoAtom.Logging` with `JsonFileLogWriter`

## Verify package setup

After restore/build, generated members should appear in IDE for:

- `[LogMethod]` partial methods
- `[LogFormatter]` partial records/properties

If not, verify analyzer wiring in your project file.

See also:

- `doc/source-generator.md`
- `doc/log-formatter.md`
- `doc/terminal.md`
- `doc/aot.md`
