using System.Globalization;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog.Events;
using ZeroLog.Appenders;
using ZeroLog.Configuration;
using ZLogger;

using MelLogLevel = Microsoft.Extensions.Logging.LogLevel;
using XenoLogLevel = XenoAtom.Logging.LogLevel;
using XenoLogManager = XenoAtom.Logging.LogManager;
using ZeroLogManager = ZeroLog.LogManager;

namespace XenoAtom.Logging.Benchmark;

internal static class FormatValidation
{
    public static int Run(TextWriter output, TextWriter error)
    {
        var orderId = 1357;
        var userId = 42;
        var elapsedMs = 12.345;
        var success = true;
        var exception = new InvalidOperationException("Synthetic benchmark exception");

        var expectedException = $"Order {orderId} failed";
        var expectedStructured = $"Order {orderId} User {userId} Success {success} Elapsed {elapsedMs.ToString(CultureInfo.InvariantCulture)}";

        var exceptionResults = new Dictionary<string, string>(StringComparer.Ordinal);
        var structuredResults = new Dictionary<string, string>(StringComparer.Ordinal);

        try
        {
            exceptionResults["XenoAtom"] = CaptureXenoAtomException(orderId, exception);
            structuredResults["XenoAtom"] = CaptureXenoAtomStructured(orderId, userId, success, elapsedMs);

            exceptionResults["MicrosoftExtensions"] = CaptureMicrosoftExtensionsException(orderId, exception);
            structuredResults["MicrosoftExtensions"] = CaptureMicrosoftExtensionsStructured(orderId, userId, success, elapsedMs);

            exceptionResults["ZLogger"] = CaptureZLoggerException(orderId, exception);
            structuredResults["ZLogger"] = CaptureZLoggerStructured(orderId, userId, success, elapsedMs);

            exceptionResults["ZeroLog"] = CaptureZeroLogException(orderId, exception);
            structuredResults["ZeroLog"] = CaptureZeroLogStructured(orderId, userId, success, elapsedMs);

            exceptionResults["Serilog"] = CaptureSerilogException(orderId, exception);
            structuredResults["Serilog"] = CaptureSerilogStructured(orderId, userId, success, elapsedMs);
        }
        catch (Exception ex)
        {
            error.WriteLine("Format validation failed with an unexpected exception:");
            error.WriteLine(ex);
            return 2;
        }

        var ok = true;
        ok &= ValidateSuite(output, error, "Exception", expectedException, exceptionResults);
        ok &= ValidateSuite(output, error, "Structured", expectedStructured, structuredResults);

        return ok ? 0 : 1;
    }

    private static bool ValidateSuite(TextWriter output, TextWriter error, string suiteName, string expected, Dictionary<string, string> results)
    {
        output.WriteLine($"[{suiteName}] Expected: {expected}");
        foreach (var (name, value) in results.OrderBy(pair => pair.Key, StringComparer.Ordinal))
        {
            output.WriteLine($"[{suiteName}] {name}: {value}");
        }

        var mismatches = results.Where(pair => !string.Equals(pair.Value, expected, StringComparison.Ordinal)).ToArray();
        if (mismatches.Length == 0)
        {
            output.WriteLine($"[{suiteName}] OK");
            output.WriteLine();
            return true;
        }

        error.WriteLine($"[{suiteName}] MISMATCH");
        foreach (var mismatch in mismatches)
        {
            error.WriteLine($"[{suiteName}] {mismatch.Key} != expected");
        }

        error.WriteLine();
        return false;
    }

    private static string CaptureXenoAtomException(int orderId, Exception exception)
    {
        var writer = new XenoAtomCaptureWriter(requiresText: true);
        var config = new XenoAtom.Logging.LogManagerConfig
        {
            RootLogger =
            {
                MinimumLevel = XenoLogLevel.Trace,
                OverflowMode = XenoAtom.Logging.LoggerOverflowMode.Block,
                Writers =
                {
                    writer
                }
            }
        };

        XenoLogManager.Shutdown();
        XenoLogManager.Initialize(config);
        var logger = XenoLogManager.GetLogger("Format.XenoAtom");
        logger.Error(exception, $"Order {orderId} failed");
        XenoLogManager.Shutdown();

        return writer.LastMessage ?? string.Empty;
    }

    private static string CaptureXenoAtomStructured(int orderId, int userId, bool success, double elapsedMs)
    {
        var writer = new XenoAtomCaptureWriter(requiresText: true);
        var config = new XenoAtom.Logging.LogManagerConfig
        {
            RootLogger =
            {
                MinimumLevel = XenoLogLevel.Trace,
                OverflowMode = XenoAtom.Logging.LoggerOverflowMode.Block,
                Writers =
                {
                    writer
                }
            }
        };

        XenoLogManager.Shutdown();
        XenoLogManager.Initialize(config);
        var logger = XenoLogManager.GetLogger("Format.XenoAtom");
        logger.Info($"Order {orderId} User {userId} Success {success} Elapsed {elapsedMs}");
        XenoLogManager.Shutdown();

        return writer.LastMessage ?? string.Empty;
    }

    private static string CaptureMicrosoftExtensionsException(int orderId, Exception exception)
    {
        using var capture = new MelCaptureProvider();
        using var factory = LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(MelLogLevel.Information);
            builder.AddProvider(capture);
        });

        var logger = factory.CreateLogger("Format.MEL");
        logger.LogError(exception, "Order {OrderId} failed", orderId);
        return capture.GetOrThrow();
    }

    private static string CaptureMicrosoftExtensionsStructured(int orderId, int userId, bool success, double elapsedMs)
    {
        using var capture = new MelCaptureProvider();
        using var factory = LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(MelLogLevel.Information);
            builder.AddProvider(capture);
        });

        var logger = factory.CreateLogger("Format.MEL");
        logger.LogInformation("Order {OrderId} User {UserId} Success {Success} Elapsed {ElapsedMs}", orderId, userId, success, elapsedMs);
        return capture.GetOrThrow();
    }

    private static string CaptureZLoggerException(int orderId, Exception exception)
    {
        var gate = new ManualResetEventSlim(false);
        string? captured = null;

        using var factory = LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(MelLogLevel.Information);
            builder.AddZLoggerInMemory(options =>
            {
                options.MessageReceived += message =>
                {
                    captured = FirstLine(message);
                    gate.Set();
                };
            });
        });

        var logger = factory.CreateLogger("Format.ZLogger");
        logger.ZLogError(exception, $"Order {orderId} failed");
        WaitOrThrow(gate, "ZLogger");
        return captured ?? string.Empty;
    }

    private static string CaptureZLoggerStructured(int orderId, int userId, bool success, double elapsedMs)
    {
        var gate = new ManualResetEventSlim(false);
        string? captured = null;

        using var factory = LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(MelLogLevel.Information);
            builder.AddZLoggerInMemory(options =>
            {
                options.MessageReceived += message =>
                {
                    captured = FirstLine(message);
                    gate.Set();
                };
            });
        });

        var logger = factory.CreateLogger("Format.ZLogger");
        logger.ZLogInformation($"Order {orderId} User {userId} Success {success} Elapsed {elapsedMs}");
        WaitOrThrow(gate, "ZLogger");
        return captured ?? string.Empty;
    }

    private static string CaptureZeroLogException(int orderId, Exception exception)
    {
        var capture = new ZeroLogCaptureAppender();

        ZeroLogManager.Shutdown();
        var config = new ZeroLogConfiguration
        {
            AppendingStrategy = AppendingStrategy.Asynchronous,
            LogMessagePoolSize = 64,
            RootLogger =
            {
                Level = ZeroLog.LogLevel.Info,
                LogMessagePoolExhaustionStrategy = LogMessagePoolExhaustionStrategy.WaitUntilAvailable,
                Appenders =
                {
                    new AppenderConfiguration(capture)
                }
            }
        };

        using var lifetime = ZeroLogManager.Initialize(config);
        var logger = ZeroLogManager.GetLogger("Format.ZeroLog");
        logger.Error($"Order {orderId} failed", exception);
        var message = capture.GetOrThrow();
        ZeroLogManager.Shutdown();
        return message;
    }

    private static string CaptureZeroLogStructured(int orderId, int userId, bool success, double elapsedMs)
    {
        var capture = new ZeroLogCaptureAppender();

        ZeroLogManager.Shutdown();
        var config = new ZeroLogConfiguration
        {
            AppendingStrategy = AppendingStrategy.Asynchronous,
            LogMessagePoolSize = 64,
            RootLogger =
            {
                Level = ZeroLog.LogLevel.Info,
                LogMessagePoolExhaustionStrategy = LogMessagePoolExhaustionStrategy.WaitUntilAvailable,
                Appenders =
                {
                    new AppenderConfiguration(capture)
                }
            }
        };

        using var lifetime = ZeroLogManager.Initialize(config);
        var logger = ZeroLogManager.GetLogger("Format.ZeroLog");
        logger.Info($"Order {orderId} User {userId} Success {success} Elapsed {elapsedMs}");
        var message = capture.GetOrThrow();
        ZeroLogManager.Shutdown();
        return message;
    }

    private static string CaptureSerilogException(int orderId, Exception exception)
    {
        var capture = new SerilogCaptureSink();
        using var logger = new Serilog.LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Sink(capture)
            .CreateLogger();

        logger.Error(exception, "Order {OrderId} failed", orderId);
        return capture.GetOrThrow();
    }

    private static string CaptureSerilogStructured(int orderId, int userId, bool success, double elapsedMs)
    {
        var capture = new SerilogCaptureSink();
        using var logger = new Serilog.LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Sink(capture)
            .CreateLogger();

        logger.Information("Order {OrderId} User {UserId} Success {Success} Elapsed {ElapsedMs}", orderId, userId, success, elapsedMs);
        return capture.GetOrThrow();
    }

    private static void WaitOrThrow(ManualResetEventSlim gate, string source)
    {
        if (!gate.Wait(TimeSpan.FromSeconds(2)))
        {
            throw new InvalidOperationException($"Timed out waiting for message from {source}.");
        }
    }

    private static string FirstLine(string message)
    {
        var span = message.AsSpan();
        var newLineIndex = span.IndexOf('\n');
        if (newLineIndex < 0)
        {
            return message;
        }

        var end = newLineIndex;
        if (end > 0 && span[end - 1] == '\r')
        {
            end--;
        }

        return message[..end];
    }

    private sealed class XenoAtomCaptureWriter : XenoAtom.Logging.LogWriter
    {
        private readonly bool _requiresText;

        public XenoAtomCaptureWriter(bool requiresText)
        {
            _requiresText = requiresText;
        }

        public string? LastMessage { get; private set; }

        protected override void Log(LogMessage logMessage)
        {
            LastMessage = logMessage.Text.ToString();
        }
    }

    private sealed class MelCaptureProvider : ILoggerProvider
    {
        private readonly ManualResetEventSlim _gate = new(false);
        private string? _message;

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName) => new CaptureLogger(this);

        public void Dispose()
        {
            _gate.Dispose();
        }

        public string GetOrThrow()
        {
            if (!_gate.Wait(TimeSpan.FromSeconds(2)))
            {
                throw new InvalidOperationException("Timed out waiting for Microsoft.Extensions.Logging message.");
            }

            return _message ?? string.Empty;
        }

        private void Capture(string message)
        {
            _message = message;
            _gate.Set();
        }

        private sealed class CaptureLogger : Microsoft.Extensions.Logging.ILogger
        {
            private readonly MelCaptureProvider _owner;

            public CaptureLogger(MelCaptureProvider owner)
            {
                _owner = owner;
            }

            public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

            public bool IsEnabled(MelLogLevel logLevel) => true;

            public void Log<TState>(MelLogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                _owner.Capture(formatter(state, exception));
            }
        }
    }

    private sealed class ZeroLogCaptureAppender : Appender
    {
        private readonly ManualResetEventSlim _gate = new(false);
        private string? _message;

        public override void WriteMessage(ZeroLog.Formatting.LoggedMessage message)
        {
            _message = message.Message.ToString();
            _gate.Set();
        }

        public string GetOrThrow()
        {
            if (!_gate.Wait(TimeSpan.FromSeconds(2)))
            {
                throw new InvalidOperationException("Timed out waiting for ZeroLog message.");
            }

            return _message ?? string.Empty;
        }
    }

    private sealed class SerilogCaptureSink : ILogEventSink
    {
        private readonly ManualResetEventSlim _gate = new(false);
        private string? _message;

        public void Emit(LogEvent logEvent)
        {
            _message = logEvent.RenderMessage(CultureInfo.InvariantCulture);
            _gate.Set();
        }

        public string GetOrThrow()
        {
            if (!_gate.Wait(TimeSpan.FromSeconds(2)))
            {
                throw new InvalidOperationException("Timed out waiting for Serilog message.");
            }

            return _message ?? string.Empty;
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }
}
