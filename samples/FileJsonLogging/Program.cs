using XenoAtom.Logging;
using XenoAtom.Logging.Formatters;
using XenoAtom.Logging.Writers;

var logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
Directory.CreateDirectory(logDirectory);

var textWriter = new FileLogWriter(
    new FileLogWriterOptions(Path.Combine(logDirectory, "app.log"))
    {
        RollingInterval = FileRollingInterval.Daily,
        RetainedFileCountLimit = 7,
        AutoFlush = true
    });

var jsonWriter = new JsonFileLogWriter(
    new FileLogWriterOptions(Path.Combine(logDirectory, "app.jsonl"))
    {
        RollingInterval = FileRollingInterval.Daily,
        RetainedFileCountLimit = 7,
        AutoFlush = true
    },
    new JsonLogFormatterOptions
    {
        FieldNamingPolicy = JsonLogFieldNamingPolicy.SnakeCase,
        IncludeScopes = true
    });

var config = new LogManagerConfig
{
    RootLogger =
    {
        MinimumLevel = LogLevel.Trace,
        Writers =
        {
            textWriter,
            jsonWriter
        }
    }
};

LogManager.Initialize(config);
var logger = LogManager.GetLogger("Samples.FileJsonLogging");

using (var startupProperties = new LogProperties
{
    ("Environment", "Development"),
    ("Instance", 1)
})
{
    logger.Info(new LogEventId(100, "Startup"), "Service started", startupProperties);
}

using (var requestScope = new LogProperties
{
    ("RequestId", 4242),
    ("UserId", 77)
})
using (logger.BeginScope(requestScope))
{
    logger.Debug("Loading account");
    logger.Info($"Fetched account {77} in {12.34}ms");

    using var queryProperties = new LogProperties
    {
        ("Sql", "SELECT * FROM account WHERE id = @id"),
        ("Rows", 1)
    };
    logger.Trace("Database query completed", queryProperties);
}

try
{
    throw new InvalidOperationException("Synthetic sample failure");
}
catch (Exception exception)
{
    using var errorProperties = new LogProperties
    {
        ("Operation", "Transfer"),
        ("Amount", 42.50m)
    };
    logger.Error(new LogEventId(5001, "TransferFailed"), exception, "Operation failed", errorProperties);
}

logger.Warn("Shutting down");
LogManager.Shutdown();
