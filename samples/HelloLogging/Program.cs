using XenoAtom.Logging;
using XenoAtom.Logging.Writers;
using XenoAtom.Terminal;

var terminalWriter = new TerminalLogWriter(Terminal.Instance)
{
    EnableRichFormatting = true,
    EnableMarkupMessages = true,
};

terminalWriter.Styles.ResetToDefaults();
terminalWriter.Styles.SetStyle(LogMessageFormatSegmentKind.Timestamp, "dim");
terminalWriter.Styles.SetStyle(LogMessageFormatSegmentKind.LoggerName, "bold cyan");
terminalWriter.Styles.SetLevelStyle(LogLevel.Info, "bold green");
terminalWriter.Styles.SetLevelStyle(LogLevel.Warn, "bold yellow");
terminalWriter.Styles.SetLevelStyle(LogLevel.Error, "bold white on red");
terminalWriter.Styles.SetLevelStyle(LogLevel.Fatal, "bold white on red");

var config = new LogManagerConfig
{
    RootLogger =
    {
        MinimumLevel = LogLevel.Trace,
        Writers =
        {
            terminalWriter
        }
    }
};

LogManager.Initialize(config);
var logger = LogManager.GetLogger("Samples.HelloLogging");

logger.InfoMarkup("[bold green]HelloLogging[/] [gray]starting terminal demo[/]");

using (var startupProperties = new LogProperties
{
    ("Environment", "Development"),
    ("Instance", 1),
})
{
    logger.Info(startupProperties, $"Booting service");
}

using (var requestScope = new LogProperties
{
    ("RequestId", 4242),
    ("User", "alexa"),
})
using (logger.BeginScope(requestScope))
{
    logger.Debug($"Resolving dependencies for request {4242}");
    logger.Info(new LogEventId(1001, "RequestStart"), $"Request started");

    using var queryProperties = new LogProperties();
    queryProperties.Add("sql", "SELECT * FROM customers WHERE active = 1");
    logger.Trace(queryProperties, $"Preparing query");
}

try
{
    throw new InvalidOperationException("Failed to connect to upstream service");
}
catch (Exception exception)
{
    logger.Error(exception, "Processing failed");
    logger.ErrorMarkup($"[bold red]Failure[/] correlationId={Guid.NewGuid():N}");
}

logger.WarnMarkup("[yellow]Retry scheduled in 5 seconds[/]");
logger.FatalMarkup("[bold white on red]Demo completed[/]");

LogManager.Shutdown();
