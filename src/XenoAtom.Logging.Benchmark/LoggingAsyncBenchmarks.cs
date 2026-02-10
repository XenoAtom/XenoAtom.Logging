using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Microsoft.Extensions.Logging;
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
public class LoggingAsyncBenchmarks
{
    private const string EnabledLoggerName = "Bench.Async.Enabled";
    private const int SharedAsyncInFlightCapacity = 8_192;

    private readonly int _orderId = 1357;
    private readonly int _userId = 42;
    private readonly double _elapsedMs = 12.345;
    private readonly bool _success = true;
    private readonly Exception _exception = new InvalidOperationException("Synthetic benchmark exception");

    private AsyncIntConsumer _consumer = null!;

    private XenoAtom.Logging.Logger _xenoEnabled = null!;

    private ILoggerFactory _zloggerFactory = null!;
    private Microsoft.Extensions.Logging.ILogger _zloggerEnabled = null!;

    private IDisposable? _zeroLogLifetime;
    private ZeroLog.Log _zeroLogEnabled = null!;

    [GlobalSetup]
    public void Setup()
    {
        _consumer = new AsyncIntConsumer(SharedAsyncInFlightCapacity);
        SetupXenoAtomLogging();
        SetupZLogger();
        SetupZeroLog();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _zloggerFactory.Dispose();
        _zeroLogLifetime?.Dispose();
        _zeroLogLifetime = null;
        ZeroLogManager.Shutdown();
        XenoLogManager.Shutdown();
        _consumer.Dispose();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("AsyncEnabledStructured")]
    public void XenoAtom_Async_Enabled_Structured()
    {
        _xenoEnabled.Info($"Order {_orderId} User {_userId} Success {_success} Elapsed {_elapsedMs}");
    }

    [Benchmark]
    [BenchmarkCategory("AsyncEnabledStructured")]
    public void ZLogger_Async_Enabled_Structured()
    {
        _zloggerEnabled.ZLogInformation($"Order {_orderId} User {_userId} Success {_success} Elapsed {_elapsedMs}");
    }

    [Benchmark]
    [BenchmarkCategory("AsyncEnabledStructured")]
    public void ZeroLog_Async_Enabled_Structured()
    {
        _zeroLogEnabled.Info($"Order {_orderId} User {_userId} Success {_success} Elapsed {_elapsedMs}");
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("AsyncEnabledException")]
    public void XenoAtom_Async_Enabled_Exception()
    {
        _xenoEnabled.Error(_exception, $"Order {_orderId} failed");
    }

    [Benchmark]
    [BenchmarkCategory("AsyncEnabledException")]
    public void ZLogger_Async_Enabled_Exception()
    {
        _zloggerEnabled.ZLogError(_exception, $"Order {_orderId} failed");
    }

    [Benchmark]
    [BenchmarkCategory("AsyncEnabledException")]
    public void ZeroLog_Async_Enabled_Exception()
    {
        _zeroLogEnabled.Error($"Order {_orderId} failed", _exception);
    }


    private void SetupXenoAtomLogging()
    {
        XenoLogManager.Shutdown();

        var config = new XenoAtom.Logging.LogManagerConfig
        {
            AsyncLogMessageQueueCapacity = SharedAsyncInFlightCapacity,
            RootLogger =
            {
                MinimumLevel = XenoLogLevel.Warn,
                OverflowMode = XenoAtom.Logging.LoggerOverflowMode.Block,
                Writers =
                {
                    new AsyncCountingXenoWriter(_consumer)
                }
            }
        };
        config.Loggers.Add(EnabledLoggerName, XenoLogLevel.Info);

        XenoLogManager.InitializeForAsync(config);
        _xenoEnabled = XenoLogManager.GetLogger(EnabledLoggerName);
    }

    private void SetupZLogger()
    {
        _zloggerFactory = LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(MelLogLevel.Information);
            builder.AddZLoggerInMemory(options =>
            {
                options.MessageReceived += message =>
                {
                    _consumer.Enqueue(message.Length);
                };
            });
        });

        _zloggerEnabled = _zloggerFactory.CreateLogger(EnabledLoggerName);
    }

    private void SetupZeroLog()
    {
        ZeroLogManager.Shutdown();

        var config = new ZeroLogConfiguration
        {
            AppendingStrategy = AppendingStrategy.Asynchronous,
            LogMessagePoolSize = SharedAsyncInFlightCapacity,
            RootLogger =
            {
                Level = ZeroLog.LogLevel.Warn,
                LogMessagePoolExhaustionStrategy = LogMessagePoolExhaustionStrategy.WaitUntilAvailable,
                Appenders =
                {
                    new AppenderConfiguration(new AsyncCountingZeroLogAppender(_consumer))
                }
            },
            Loggers =
            {
                { EnabledLoggerName, ZeroLog.LogLevel.Info }
            }
        };

        _zeroLogLifetime = ZeroLogManager.Initialize(config);
        _zeroLogEnabled = ZeroLogManager.GetLogger(EnabledLoggerName);
    }

    private sealed class AsyncCountingXenoWriter : LogWriter
    {
        private readonly AsyncIntConsumer _consumer;

        public AsyncCountingXenoWriter(AsyncIntConsumer consumer)
        {
            _consumer = consumer;
        }

        protected override void Log(in XenoAtom.Logging.LogMessage logMessage)
        {
            _consumer.Enqueue(logMessage.Text.Length);
        }
    }

    private sealed class AsyncCountingZeroLogAppender : Appender
    {
        private readonly AsyncIntConsumer _consumer;

        public AsyncCountingZeroLogAppender(AsyncIntConsumer consumer)
        {
            _consumer = consumer;
        }

        public override void WriteMessage(ZeroLog.Formatting.LoggedMessage message)
        {
            _consumer.Enqueue(message.Message.Length);
        }
    }
}
