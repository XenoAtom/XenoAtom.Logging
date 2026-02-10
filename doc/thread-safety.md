# Thread Safety

This document summarizes thread-safety guarantees and configuration rules for `XenoAtom.Logging`.

## Runtime usage

- `LogManager` static APIs are thread-safe.
- `Logger` instances are safe for concurrent log calls.
- `LogWriter` implementations may receive concurrent calls depending on processor mode and configuration.

## Error propagation by processor mode

- `LogMessageSyncProcessor`: writer exceptions propagate to the caller thread.
- `LogMessageAsyncProcessor`: writer/dispatch exceptions are handled on the background thread and do not propagate to caller threads.
- Configure `LogManagerConfig.AsyncErrorHandler` to observe async failures, and use `LogManager.GetDiagnostics()` (`ErrorCount`, `DroppedMessages`) for runtime visibility.

## Configuration mutation

- Treat `LogManagerConfig`, `LoggerConfig.Writers`, and writer filter collections as single-threaded configuration objects.
- Build or update configuration from one thread, then call `LogManagerConfig.ApplyChanges()` to refresh logger/writer snapshots.
- Avoid mutating configuration collections concurrently with heavy logging traffic.

## `LogProperties` ownership

- `LogProperties` is a mutable value type that rents pooled buffers.
- Avoid copying populated `LogProperties` values.
- Dispose only the original owner instance.
- If you need to merge properties, create a new instance and call `AddRange`.

## Writer-specific notes

- `StreamLogWriter` serializes stream writes internally and can optionally own/dispose its stream.
- `FileLogWriter` serializes file operations internally and is safe for concurrent logging.
- `TerminalLogWriter` writes through `XenoAtom.Terminal`; `TerminalLogStyleConfiguration` should be treated as startup-time configuration (not thread-safe for concurrent mutation/read).
