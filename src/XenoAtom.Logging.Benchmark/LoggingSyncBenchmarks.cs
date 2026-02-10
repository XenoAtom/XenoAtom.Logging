using System.Globalization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog.Events;
using XenoAtom.Logging.Writers;
using ZeroLog.Appenders;
using ZeroLog.Configuration;
using ZLogger;

using MelLogLevel = Microsoft.Extensions.Logging.LogLevel;
using XenoLogLevel = XenoAtom.Logging.LogLevel;
using XenoLogManager = XenoAtom.Logging.LogManager;
using ZeroLogManager = ZeroLog.LogManager;

namespace XenoAtom.Logging.Benchmark;

[MemoryDiagnoser]
[CategoriesColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class LoggingSyncBenchmarks
{
    private const string EnabledLoggerName = "Bench.Enabled";
    private const string DisabledLoggerName = "Bench.Disabled";

    private readonly int _orderId = 1357;
    private readonly int _userId = 42;
    private readonly double _elapsedMs = 12.345;
    private readonly bool _success = true;
    private readonly Exception _exception = new InvalidOperationException("Synthetic benchmark exception");
    private readonly int _priorityItemId = 21;

    private readonly LengthCounter _counter = new();

    private XenoAtom.Logging.Logger _xenoEnabled = null!;
    private XenoAtom.Logging.Logger _xenoDisabled = null!;

    private ILoggerFactory _melFactory = null!;
    private Microsoft.Extensions.Logging.ILogger _melEnabled = null!;
    private Microsoft.Extensions.Logging.ILogger _melDisabled = null!;

    private ILoggerFactory _zloggerFactory = null!;
    private Microsoft.Extensions.Logging.ILogger _zloggerEnabled = null!;
    private Microsoft.Extensions.Logging.ILogger _zloggerDisabled = null!;

    private IDisposable? _zeroLogLifetime;
    private ZeroLog.Log _zeroLogEnabled = null!;
    private ZeroLog.Log _zeroLogDisabled = null!;

    private Serilog.ILogger _serilogEnabled = null!;
    private Serilog.ILogger _serilogDisabled = null!;

    [GlobalSetup]
    public void Setup()
    {
        SetupXenoAtomLogging();
        SetupMicrosoftExtensionsLogging();
        SetupZLogger();
        SetupZeroLog();
        SetupSerilog();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _melFactory.Dispose();
        _zloggerFactory.Dispose();
        _zeroLogLifetime?.Dispose();
        _zeroLogLifetime = null;
        ZeroLogManager.Shutdown();
        XenoLogManager.Shutdown();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Disabled")]
    public void XenoAtom_Disabled()
    {
        _xenoDisabled.Info($"Order {_orderId} User {_userId} Success {_success} Elapsed {_elapsedMs}");
    }

    [Benchmark]
    [BenchmarkCategory("Disabled")]
    public void MicrosoftExtensions_Disabled()
    {
        _melDisabled.LogInformation("Order {OrderId} User {UserId} Success {Success} Elapsed {ElapsedMs}", _orderId, _userId, _success, _elapsedMs);
    }

    [Benchmark]
    [BenchmarkCategory("Disabled")]
    public void ZLogger_Disabled()
    {
        _zloggerDisabled.ZLogInformation($"Order {_orderId} User {_userId} Success {_success} Elapsed {_elapsedMs}");
    }

    [Benchmark]
    [BenchmarkCategory("Disabled")]
    public void ZeroLog_Disabled()
    {
        _zeroLogDisabled.Info($"Order {_orderId} User {_userId} Success {_success} Elapsed {_elapsedMs}");
    }

    [Benchmark]
    [BenchmarkCategory("Disabled")]
    public void Serilog_Disabled()
    {
        _serilogDisabled.Information("Order {OrderId} User {UserId} Success {Success} Elapsed {ElapsedMs}", _orderId, _userId, _success, _elapsedMs);
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("EnabledSimpleNonInterpolated")]
    public void XenoAtom_Enabled_Simple_NonInterpolated()
    {
        _xenoEnabled.Info("Benchmark simple message");
    }

    [Benchmark]
    [BenchmarkCategory("EnabledSimpleNonInterpolated")]
    public void MicrosoftExtensions_Enabled_Simple_NonInterpolated()
    {
        _melEnabled.LogInformation("Benchmark simple message");
    }

    [Benchmark]
    [BenchmarkCategory("EnabledSimpleNonInterpolated")]
    public void ZLogger_Enabled_Simple_NonInterpolated()
    {
        _zloggerEnabled.LogInformation("Benchmark simple message");
    }

    [Benchmark]
    [BenchmarkCategory("EnabledSimpleNonInterpolated")]
    public void ZeroLog_Enabled_Simple_NonInterpolated()
    {
        _zeroLogEnabled.Info("Benchmark simple message");
    }

    [Benchmark]
    [BenchmarkCategory("EnabledSimpleNonInterpolated")]
    public void Serilog_Enabled_Simple_NonInterpolated()
    {
        _serilogEnabled.Information("Benchmark simple message");
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("EnabledSimpleInterpolated")]
    public void XenoAtom_Enabled_Simple_Interpolated()
    {
        _xenoEnabled.Info($"Benchmark simple item {_orderId}");
    }

    [Benchmark]
    [BenchmarkCategory("EnabledSimpleInterpolated")]
    public void MicrosoftExtensions_Enabled_Simple_Interpolated()
    {
        _melEnabled.LogInformation($"Benchmark simple item {_orderId}");
    }

    [Benchmark]
    [BenchmarkCategory("EnabledSimpleInterpolated")]
    public void ZLogger_Enabled_Simple_Interpolated()
    {
        _zloggerEnabled.ZLogInformation($"Benchmark simple item {_orderId}");
    }

    [Benchmark]
    [BenchmarkCategory("EnabledSimpleInterpolated")]
    public void ZeroLog_Enabled_Simple_Interpolated()
    {
        _zeroLogEnabled.Info($"Benchmark simple item {_orderId}");
    }

    [Benchmark]
    [BenchmarkCategory("EnabledSimpleInterpolated")]
    public void Serilog_Enabled_Simple_Interpolated()
    {
        _serilogEnabled.Information($"Benchmark simple item {_orderId}");
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("EnabledStructured")]
    public void XenoAtom_Enabled_Structured()
    {
        _xenoEnabled.Info($"Order {_orderId} User {_userId} Success {_success} Elapsed {_elapsedMs}");
    }

    [Benchmark]
    [BenchmarkCategory("EnabledStructured")]
    public void MicrosoftExtensions_Enabled_Structured()
    {
        _melEnabled.LogInformation("Order {OrderId} User {UserId} Success {Success} Elapsed {ElapsedMs}", _orderId, _userId, _success, _elapsedMs);
    }

    [Benchmark]
    [BenchmarkCategory("EnabledStructured")]
    public void ZLogger_Enabled_Structured()
    {
        _zloggerEnabled.ZLogInformation($"Order {_orderId} User {_userId} Success {_success} Elapsed {_elapsedMs}");
    }

    [Benchmark]
    [BenchmarkCategory("EnabledStructured")]
    public void ZeroLog_Enabled_Structured()
    {
        _zeroLogEnabled.Info($"Order {_orderId} User {_userId} Success {_success} Elapsed {_elapsedMs}");
    }

    [Benchmark]
    [BenchmarkCategory("EnabledStructured")]
    public void Serilog_Enabled_Structured()
    {
        _serilogEnabled.Information("Order {OrderId} User {UserId} Success {Success} Elapsed {ElapsedMs}", _orderId, _userId, _success, _elapsedMs);
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("EnabledGeneratedSimple")]
    public void XenoAtom_Enabled_Generated_Simple()
    {
        XenoGeneratedSyncLog.PriorityItemProcessed(_xenoEnabled, _priorityItemId);
    }

    [Benchmark]
    [BenchmarkCategory("EnabledGeneratedSimple")]
    public void MicrosoftExtensions_Enabled_Generated_Simple()
    {
        MelGeneratedSyncLog.PriorityItemProcessed(_melEnabled, _priorityItemId);
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("EnabledException")]
    public void XenoAtom_Enabled_Exception()
    {
        _xenoEnabled.Error(_exception, $"Order {_orderId} failed");
    }

    [Benchmark]
    [BenchmarkCategory("EnabledException")]
    public void MicrosoftExtensions_Enabled_Exception()
    {
        _melEnabled.LogError(_exception, "Order {OrderId} failed", _orderId);
    }

    [Benchmark]
    [BenchmarkCategory("EnabledException")]
    public void ZLogger_Enabled_Exception()
    {
        _zloggerEnabled.ZLogError(_exception, $"Order {_orderId} failed");
    }

    [Benchmark]
    [BenchmarkCategory("EnabledException")]
    public void ZeroLog_Enabled_Exception()
    {
        _zeroLogEnabled.Error($"Order {_orderId} failed", _exception);
    }

    [Benchmark]
    [BenchmarkCategory("EnabledException")]
    public void Serilog_Enabled_Exception()
    {
        _serilogEnabled.Error(_exception, "Order {OrderId} failed", _orderId);
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("EnabledGeneratedException")]
    public void XenoAtom_Enabled_Generated_Exception()
    {
        XenoGeneratedSyncLog.PriorityItemFailed(_xenoEnabled, _exception, _priorityItemId);
    }

    [Benchmark]
    [BenchmarkCategory("EnabledGeneratedException")]
    public void MicrosoftExtensions_Enabled_Generated_Exception()
    {
        MelGeneratedSyncLog.PriorityItemFailed(_melEnabled, _exception, _priorityItemId);
    }

    private void SetupXenoAtomLogging()
    {
        XenoLogManager.Shutdown();

        var config = new XenoAtom.Logging.LogManagerConfig
        {
            RootLogger =
            {
                MinimumLevel = XenoLogLevel.Warn,
                Writers =
                {
                    new CountingXenoWriter(_counter)
                }
            }
        };
        config.Loggers.Add(EnabledLoggerName, XenoLogLevel.Info);

        XenoLogManager.Initialize(config);
        _xenoEnabled = XenoLogManager.GetLogger(EnabledLoggerName);
        _xenoDisabled = XenoLogManager.GetLogger(DisabledLoggerName);
    }

    private void SetupMicrosoftExtensionsLogging()
    {
        _melFactory = LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(MelLogLevel.Warning);
            builder.AddFilter((category, level) => category == EnabledLoggerName ? level >= MelLogLevel.Information : level >= MelLogLevel.Warning);
            builder.AddProvider(new FormattingNoopLoggerProvider(_counter));
        });

        _melEnabled = _melFactory.CreateLogger(EnabledLoggerName);
        _melDisabled = _melFactory.CreateLogger(DisabledLoggerName);
    }

    private void SetupZLogger()
    {
        _zloggerFactory = LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(MelLogLevel.Warning);
            builder.AddFilter((category, level) => category == EnabledLoggerName ? level >= MelLogLevel.Information : level >= MelLogLevel.Warning);
            builder.AddZLoggerInMemory(options =>
            {
                options.MessageReceived += message =>
                {
                    var span = message.AsSpan();
                    var newLineIndex = span.IndexOf('\n');
                    if (newLineIndex >= 0)
                    {
                        var end = newLineIndex;
                        if (end > 0 && span[end - 1] == '\r')
                        {
                            end--;
                        }
                        _counter.Add(end);
                        return;
                    }

                    _counter.Add(message.Length);
                };
            });
        });

        _zloggerEnabled = _zloggerFactory.CreateLogger(EnabledLoggerName);
        _zloggerDisabled = _zloggerFactory.CreateLogger(DisabledLoggerName);
    }

    private void SetupZeroLog()
    {
        ZeroLogManager.Shutdown();

        var config = new ZeroLogConfiguration
        {
            AppendingStrategy = AppendingStrategy.Synchronous,
            RootLogger =
            {
                Level = ZeroLog.LogLevel.Warn,
                LogMessagePoolExhaustionStrategy = LogMessagePoolExhaustionStrategy.WaitUntilAvailable,
                Appenders =
                {
                    new AppenderConfiguration(new CountingZeroLogAppender(_counter))
                }
            },
            Loggers =
            {
                { EnabledLoggerName, ZeroLog.LogLevel.Info }
            }
        };

        _zeroLogLifetime = ZeroLogManager.Initialize(config);
        _zeroLogEnabled = ZeroLogManager.GetLogger(EnabledLoggerName);
        _zeroLogDisabled = ZeroLogManager.GetLogger(DisabledLoggerName);
    }

    private void SetupSerilog()
    {
        var sink = new FormattingSerilogSink(_counter);

        _serilogEnabled = new Serilog.LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Sink(sink)
            .CreateLogger();

        _serilogDisabled = new Serilog.LoggerConfiguration()
            .MinimumLevel.Warning()
            .WriteTo.Sink(sink)
            .CreateLogger();
    }

    private sealed class LengthCounter
    {
        private long _value;

        public void Add(int value)
        {
            Interlocked.Add(ref _value, value);
        }

    }

    private sealed class CountingXenoWriter : LogWriter
    {
        private readonly LengthCounter _counter;

        public CountingXenoWriter(LengthCounter counter)
        {
            _counter = counter;
        }

        protected override void Log(LogMessage logMessage)
        {
            _counter.Add(logMessage.Text.Length);
        }
    }

    private sealed class FormattingNoopLoggerProvider : ILoggerProvider
    {
        private readonly LengthCounter _counter;

        public FormattingNoopLoggerProvider(LengthCounter counter)
        {
            _counter = counter;
        }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
        {
            return new FormattingNoopLogger(_counter);
        }

        public void Dispose()
        {
        }

        private sealed class FormattingNoopLogger : Microsoft.Extensions.Logging.ILogger
        {
            private readonly LengthCounter _counter;

            public FormattingNoopLogger(LengthCounter counter)
            {
                _counter = counter;
            }

            public IDisposable BeginScope<TState>(TState state) where TState : notnull
            {
                return NullScope.Instance;
            }

            public bool IsEnabled(MelLogLevel logLevel) => true;

            public void Log<TState>(MelLogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                _counter.Add(formatter(state, exception).Length);
            }
        }
    }

    private sealed class FormattingSerilogSink : ILogEventSink
    {
        private readonly LengthCounter _counter;

        public FormattingSerilogSink(LengthCounter counter)
        {
            _counter = counter;
        }

        public void Emit(LogEvent logEvent)
        {
            _counter.Add(logEvent.RenderMessage(CultureInfo.InvariantCulture).Length);
        }
    }

    private sealed class CountingZeroLogAppender : Appender
    {
        private readonly LengthCounter _counter;

        public CountingZeroLogAppender(LengthCounter counter)
        {
            _counter = counter;
        }

        public override void WriteMessage(ZeroLog.Formatting.LoggedMessage message)
        {
            _counter.Add(message.Message.Length);
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

internal static partial class XenoGeneratedSyncLog
{
    [global::XenoAtom.Logging.LogMethod(global::XenoAtom.Logging.LogLevel.Info, "Processing priority item: {itemId}", EventId = 1001, EventName = "PriorityItemProcessed")]
    public static partial void PriorityItemProcessed(global::XenoAtom.Logging.Logger logger, int itemId);

    [global::XenoAtom.Logging.LogMethod(global::XenoAtom.Logging.LogLevel.Error, "Processing priority item failed: {itemId}", EventId = 1002, EventName = "PriorityItemFailed")]
    public static partial void PriorityItemFailed(global::XenoAtom.Logging.Logger logger, Exception exception, int itemId);
}

internal static partial class MelGeneratedSyncLog
{
    [LoggerMessage(EventId = 1001, Level = MelLogLevel.Information, Message = "Processing priority item: {ItemId}")]
    public static partial void PriorityItemProcessed(Microsoft.Extensions.Logging.ILogger logger, int itemId);

    [LoggerMessage(EventId = 1002, Level = MelLogLevel.Error, Message = "Processing priority item failed: {ItemId}")]
    public static partial void PriorityItemFailed(Microsoft.Extensions.Logging.ILogger logger, Exception exception, int itemId);
}
