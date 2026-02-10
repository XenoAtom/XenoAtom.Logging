# Shutdown Semantics

`LogManager.Shutdown()` is synchronous and performs a coordinated stop:

1. Stop accepting new processing through the active manager instance.
2. Dispose the active message processor.
3. Flush and dispose all configured writers once.
4. Reset logger state so existing logger references can be safely reused after a later `Initialize(...)`.

## Sync processor (`LogMessageSyncProcessor`)

- Calls are already processed inline.
- `Shutdown()` mainly flushes/disposes writers.

## Async processor (`LogMessageAsyncProcessor`)

- The background consumer drains queued messages before exiting.
- Writers are flushed as part of normal drain behavior.
- On shutdown, the processor waits for the background thread with a bounded timeout.
- If the background thread does not stop before timeout, a timeout error is reported through `LogManagerConfig.AsyncErrorHandler` (when configured) and counted in `LogManager.GetDiagnostics().ErrorCount`.

If a writer blocks indefinitely, shutdown returns after the timeout rather than hanging forever.

## In-flight messages

- Messages already enqueued before shutdown are processed when possible.
- Messages attempted after shutdown are not processed by an active manager.
- Existing logger references are reset and reconfigured on the next `Initialize(...)`.

## Recommended usage

For service/hosted apps:

```csharp
try
{
    RunApplication();
}
finally
{
    LogManager.Shutdown();
}
```

For tests:

- Call `LogManager.Shutdown()` in both setup and cleanup hooks to isolate state between tests.

See also:

- `doc/readme.md`
- `doc/thread-safety.md`
- `doc/filtering.md`
