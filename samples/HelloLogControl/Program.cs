using XenoAtom.Logging;
using XenoAtom.Logging.Writers;
using XenoAtom.Terminal;
using XenoAtom.Terminal.UI;
using XenoAtom.Terminal.UI.Controls;
using XenoAtom.Terminal.UI.Styling;

using var session = Terminal.Open();

var logControl = new LogControl
{
    MaxCapacity = 4000
}.WrapText(true);

var terminalWriter = new TerminalLogControlWriter(logControl)
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
var logger = LogManager.GetLogger("Samples.HelloLogControl");

var runBackgroundLogs = true;
var backgroundTask = Task.Run(async () =>
{
    var index = 0;
    while (Volatile.Read(ref runBackgroundLogs))
    {
        index++;
        if (index % 7 == 0)
        {
            logger.ErrorMarkup($"[bold red]Background failure[/] tick={index}");
        }
        else if (index % 5 == 0)
        {
            logger.WarnMarkup($"[yellow]Background warning[/] tick={index}");
        }
        else
        {
            logger.InfoMarkup($"[gray]Background tick[/] #{index}");
        }

        await Task.Delay(700).ConfigureAwait(false);
    }
});

var buttonInfo = new Button("Info")
    .Tone(ControlTone.Primary)
    .Click(() => logger.Info("Manual info log"));
var buttonWarn = new Button("Warn")
    .Tone(ControlTone.Warning)
    .Click(() => logger.Warn("Manual warning log"));
var buttonError = new Button("Error")
    .Tone(ControlTone.Error)
    .Click(() => logger.Error("Manual error log"));
var buttonMarkup = new Button("Markup")
    .Click(() => logger.InfoMarkup("[green]Manual markup[/] [gray](from button)[/]"));
var buttonClear = new Button("Clear")
    .Click(logControl.Clear);
var buttonStop = new Button("Stop")
    .Click(() => logControl.App?.Stop());

var root = new VStack(
        new Markup("[bold]HelloLogControl[/] [dim]- XenoAtom.Logging + XenoAtom.Terminal.UI LogControl sink[/]"),
        new TextBlock("Use the buttons below to generate logs. Background logs run on a worker thread and are marshaled to the UI thread by TerminalLogControlWriter."),
        new TextBlock("LogControl supports built-in search (Ctrl+F), copy, and tail-style viewing."),
        new HStack(buttonInfo, buttonWarn, buttonError, buttonMarkup, buttonClear, buttonStop).Spacing(1),
        new Group()
            .TopLeftText("Logs")
            .TopRightText("Ctrl+F Search")
            .Padding(1)
            .Content(logControl))
    .Spacing(1)
    .HorizontalAlignment(Align.Stretch)
    .VerticalAlignment(Align.Stretch);

logger.InfoMarkup("[bold green]HelloLogControl started[/]");
logger.Info("Press Ctrl+Q or click Stop to exit.");

Terminal.Run(root, () => TerminalLoopResult.Continue);

Volatile.Write(ref runBackgroundLogs, false);
await backgroundTask.ConfigureAwait(false);
LogManager.Shutdown();
