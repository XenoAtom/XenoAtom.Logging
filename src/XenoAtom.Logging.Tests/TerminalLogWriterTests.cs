// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Diagnostics;
using System.Threading;
using XenoAtom.Logging.Writers;
using XenoAtom.Terminal.Backends;
using XenoAtom.Terminal.UI;
using XenoAtom.Terminal.UI.Controls;

namespace XenoAtom.Logging.Tests;

[TestClass]
public class TerminalLogWriterTests
{
    [TestInitialize]
    public void Initialize()
    {
        LogManager.Shutdown();
        global::XenoAtom.Terminal.Terminal.Close();
    }

    [TestCleanup]
    public void Cleanup()
    {
        LogManager.Shutdown();
        global::XenoAtom.Terminal.Terminal.Close();
    }

    [TestMethod]
    public void TerminalLogWriter_WritesToTerminalBackend()
    {
        var backend = new InMemoryTerminalBackend();
        using (global::XenoAtom.Terminal.Terminal.Open(backend, force: true))
        {
            var config = new LogManagerConfig
            {
                RootLogger =
                {
                    MinimumLevel = LogLevel.Trace,
                    Writers =
                    {
                        new TerminalLogWriter(global::XenoAtom.Terminal.Terminal.Instance)
                    }
                }
            };

            LogManager.Initialize<LogMessageSyncProcessor>(config);
            var logger = LogManager.GetLogger("Tests.Terminal");
            logger.Info("terminal message");
        }

        var output = backend.GetOutText();
        Assert.IsTrue(output.Contains("terminal message", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains(Environment.NewLine, StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("\x1b[", StringComparison.Ordinal));
    }

    [TestMethod]
    public void TerminalLogWriter_InfoMarkup_RendersMarkupPayload()
    {
        var backend = new InMemoryTerminalBackend();
        using (global::XenoAtom.Terminal.Terminal.Open(backend, force: true))
        {
            var config = new LogManagerConfig
            {
                RootLogger =
                {
                    MinimumLevel = LogLevel.Trace,
                    Writers =
                    {
                        new TerminalLogWriter(global::XenoAtom.Terminal.Terminal.Instance)
                    }
                }
            };

            LogManager.Initialize<LogMessageSyncProcessor>(config);
            var logger = LogManager.GetLogger("Tests.TerminalMarkup");
            logger.InfoMarkup("[red]danger[/] plain");
        }

        var output = backend.GetOutText();
        Assert.IsTrue(output.Contains("danger", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("plain", StringComparison.Ordinal));
        Assert.IsFalse(output.Contains("[red]", StringComparison.Ordinal));
        Assert.IsFalse(output.Contains("[/]", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("\x1b[", StringComparison.Ordinal));
    }

    [TestMethod]
    public void TerminalLogWriter_InfoMarkup_InterpolatedValuesAreEscaped()
    {
        var backend = new InMemoryTerminalBackend();
        using (global::XenoAtom.Terminal.Terminal.Open(backend, force: true))
        {
            var config = new LogManagerConfig
            {
                RootLogger =
                {
                    MinimumLevel = LogLevel.Trace,
                    Writers =
                    {
                        new TerminalLogWriter(global::XenoAtom.Terminal.Terminal.Instance)
                    }
                }
            };

            LogManager.Initialize<LogMessageSyncProcessor>(config);
            var logger = LogManager.GetLogger("Tests.TerminalMarkupEscape");
            var userInput = "[red]INJECT[/]";
            logger.InfoMarkup($"User: {userInput}");
        }

        var output = backend.GetOutText();
        Assert.IsTrue(output.Contains("User: [red]INJECT[/]", StringComparison.Ordinal));
    }

    [TestMethod]
    public void TerminalLogWriter_MarkupMethods_WorkAcrossAllLevels()
    {
        var backend = new InMemoryTerminalBackend();
        using (global::XenoAtom.Terminal.Terminal.Open(backend, force: true))
        {
            var config = new LogManagerConfig
            {
                RootLogger =
                {
                    MinimumLevel = LogLevel.Trace,
                    Writers =
                    {
                        new TerminalLogWriter(global::XenoAtom.Terminal.Terminal.Instance)
                    }
                }
            };

            LogManager.Initialize<LogMessageSyncProcessor>(config);
            var logger = LogManager.GetLogger("Tests.TerminalMarkup.AllLevels");
            logger.TraceMarkup("[cyan]trace[/]");
            logger.DebugMarkup("[blue]debug[/]");
            logger.InfoMarkup("[green]info[/]");
            logger.WarnMarkup("[yellow]warn[/]");
            logger.ErrorMarkup("[red]error[/]");
            logger.FatalMarkup("[white on red]fatal[/]");
        }

        var output = backend.GetOutText();
        Assert.IsTrue(output.Contains("trace", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("debug", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("info", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("warn", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("error", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("fatal", StringComparison.Ordinal));
        Assert.IsFalse(output.Contains("[cyan]", StringComparison.Ordinal));
        Assert.IsFalse(output.Contains("[blue]", StringComparison.Ordinal));
        Assert.IsFalse(output.Contains("[green]", StringComparison.Ordinal));
        Assert.IsFalse(output.Contains("[yellow]", StringComparison.Ordinal));
        Assert.IsFalse(output.Contains("[red]", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("\x1b[", StringComparison.Ordinal));
    }

    [TestMethod]
    public void TerminalLogWriter_MarkupInterpolatedMethods_EscapeValuesAcrossAllLevels()
    {
        var backend = new InMemoryTerminalBackend();
        using (global::XenoAtom.Terminal.Terminal.Open(backend, force: true))
        {
            var config = new LogManagerConfig
            {
                RootLogger =
                {
                    MinimumLevel = LogLevel.Trace,
                    Writers =
                    {
                        new TerminalLogWriter(global::XenoAtom.Terminal.Terminal.Instance)
                    }
                }
            };

            LogManager.Initialize<LogMessageSyncProcessor>(config);
            var logger = LogManager.GetLogger("Tests.TerminalMarkup.InterpolatedAllLevels");
            var userInput = "[red]INJECT[/]";

            logger.TraceMarkup($"trace={userInput}");
            logger.DebugMarkup($"debug={userInput}");
            logger.InfoMarkup($"info={userInput}");
            logger.WarnMarkup($"warn={userInput}");
            logger.ErrorMarkup($"error={userInput}");
            logger.FatalMarkup($"fatal={userInput}");
        }

        var output = backend.GetOutText();
        Assert.IsTrue(output.Contains("trace=[red]INJECT[/]", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("debug=[red]INJECT[/]", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("info=[red]INJECT[/]", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("warn=[red]INJECT[/]", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("error=[red]INJECT[/]", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("fatal=[red]INJECT[/]", StringComparison.Ordinal));
    }

    [TestMethod]
    public void TerminalLogWriter_RendersVisualAttachment()
    {
        var backend = new InMemoryTerminalBackend();
        using (global::XenoAtom.Terminal.Terminal.Open(backend, force: true))
        {
            var config = new LogManagerConfig
            {
                RootLogger =
                {
                    MinimumLevel = LogLevel.Trace,
                    Writers =
                    {
                        new TerminalLogWriter(global::XenoAtom.Terminal.Terminal.Instance)
                    }
                }
            };

            LogManager.Initialize<LogMessageSyncProcessor>(config);
            var logger = LogManager.GetLogger("Tests.TerminalVisual");
            var table = new Table();
            table.Headers("Task", "State").AddRow("Build", "OK");

            logger.Info(table, "build summary");
        }

        var output = backend.GetOutText();
        Assert.IsTrue(output.Contains("build summary", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("Task", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("Build", StringComparison.Ordinal));
    }

    [TestMethod]
    public void TerminalLogWriter_RendersVisualAttachmentWithMarkupMessage()
    {
        var backend = new InMemoryTerminalBackend();
        using (global::XenoAtom.Terminal.Terminal.Open(backend, force: true))
        {
            var config = new LogManagerConfig
            {
                RootLogger =
                {
                    MinimumLevel = LogLevel.Trace,
                    Writers =
                    {
                        new TerminalLogWriter(global::XenoAtom.Terminal.Terminal.Instance)
                    }
                }
            };

            LogManager.Initialize<LogMessageSyncProcessor>(config);
            var logger = LogManager.GetLogger("Tests.TerminalVisual.Markup");
            var table = new Table();
            table.Headers("Task", "State").AddRow("Deploy", "Done");

            logger.InfoMarkup(table, "[green]deployment complete[/]");
        }

        var output = backend.GetOutText();
        Assert.IsTrue(output.Contains("deployment complete", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("Deploy", StringComparison.Ordinal));
        Assert.IsFalse(output.Contains("[green]", StringComparison.Ordinal));
    }

    [TestMethod]
    public void TerminalLogWriter_DisabledMarkupMessages_WritesLiteralMarkup()
    {
        var backend = new InMemoryTerminalBackend();
        using (global::XenoAtom.Terminal.Terminal.Open(backend, force: true))
        {
            var writer = new TerminalLogWriter(global::XenoAtom.Terminal.Terminal.Instance)
            {
                EnableMarkupMessages = false
            };

            var config = new LogManagerConfig
            {
                RootLogger =
                {
                    MinimumLevel = LogLevel.Trace,
                    Writers =
                    {
                        writer
                    }
                }
            };

            LogManager.Initialize<LogMessageSyncProcessor>(config);
            var logger = LogManager.GetLogger("Tests.TerminalMarkupDisabled");
            logger.InfoMarkup("[red]danger[/]");
        }

        var output = backend.GetOutText();
        Assert.IsTrue(output.Contains("[red]danger[/]", StringComparison.Ordinal));
    }

    [TestMethod]
    public void TerminalLogWriter_StyleConfiguration_ClearRemovesAllAnsiStyling()
    {
        var backend = new InMemoryTerminalBackend();
        using (global::XenoAtom.Terminal.Terminal.Open(backend, force: true))
        {
            var writer = new TerminalLogWriter(global::XenoAtom.Terminal.Terminal.Instance);
            writer.Styles.Clear();

            var config = new LogManagerConfig
            {
                RootLogger =
                {
                    MinimumLevel = LogLevel.Trace,
                    Writers =
                    {
                        writer
                    }
                }
            };

            LogManager.Initialize<LogMessageSyncProcessor>(config);
            var logger = LogManager.GetLogger("Tests.TerminalStyleClear");
            logger.Info("unstyled output");
        }

        var output = backend.GetOutText();
        Assert.IsTrue(output.Contains("unstyled output", StringComparison.Ordinal));
        Assert.IsFalse(output.Contains("\x1b[", StringComparison.Ordinal));
    }

    [TestMethod]
    public void TerminalLogWriter_StyleConfiguration_CanOverrideLevelStyle()
    {
        var backend = new InMemoryTerminalBackend();
        using (global::XenoAtom.Terminal.Terminal.Open(backend, force: true))
        {
            var writer = new TerminalLogWriter(global::XenoAtom.Terminal.Terminal.Instance);
            writer.Styles.Clear();
            writer.Styles.SetLevelStyle(LogLevel.Info, "underline");

            var config = new LogManagerConfig
            {
                RootLogger =
                {
                    MinimumLevel = LogLevel.Trace,
                    Writers =
                    {
                        writer
                    }
                }
            };

            LogManager.Initialize<LogMessageSyncProcessor>(config);
            var logger = LogManager.GetLogger("Tests.TerminalStyleLevel");
            logger.Info("styled output");
        }

        var output = backend.GetOutText();
        Assert.IsTrue(output.Contains("styled output", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("\x1b[4m", StringComparison.Ordinal));
    }

    [TestMethod]
    public void TerminalLogWriter_SegmentStyleResolver_FallsBackToStyleConfiguration()
    {
        var backend = new InMemoryTerminalBackend();
        using (global::XenoAtom.Terminal.Terminal.Open(backend, force: true))
        {
            var writer = new TerminalLogWriter(global::XenoAtom.Terminal.Terminal.Instance);
            writer.Styles.Clear();
            writer.Styles.SetLevelStyle(LogLevel.Info, "underline");
            writer.SegmentStyleResolver = static (_, _) => null;

            var config = new LogManagerConfig
            {
                RootLogger =
                {
                    MinimumLevel = LogLevel.Trace,
                    Writers =
                    {
                        writer
                    }
                }
            };

            LogManager.Initialize<LogMessageSyncProcessor>(config);
            var logger = LogManager.GetLogger("Tests.TerminalStyleFallback");
            logger.Info("fallback output");
        }

        var output = backend.GetOutText();
        Assert.IsTrue(output.Contains("fallback output", StringComparison.Ordinal));
        Assert.IsTrue(output.Contains("\x1b[4m", StringComparison.Ordinal));
    }

    [TestMethod]
    public void TerminalLogWriter_InfoMarkup_DoesNotMutateInputProperties()
    {
        var backend = new InMemoryTerminalBackend();
        using (global::XenoAtom.Terminal.Terminal.Open(backend, force: true))
        {
            var config = new LogManagerConfig
            {
                RootLogger =
                {
                    MinimumLevel = LogLevel.Trace,
                    Writers =
                    {
                        new TerminalLogWriter(global::XenoAtom.Terminal.Terminal.Instance)
                    }
                }
            };

            LogManager.Initialize<LogMessageSyncProcessor>(config);
            var logger = LogManager.GetLogger("Tests.TerminalMarkupProperties");
            var properties = new LogProperties { ("RequestId", 42) };

            logger.InfoMarkup(properties, "[green]ok[/]");

            Assert.AreEqual(1, properties.Count);
            var list = new List<(string Name, string Value)>();
            foreach (var property in properties)
            {
                list.Add((property.Name.ToString(), property.Value.ToString()));
            }

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("RequestId", list[0].Name);
            Assert.AreEqual("42", list[0].Value);
        }
    }

    [TestMethod]
    public void TerminalLogControlWriter_InfoMarkup_WritesIntoLogControl()
    {
        var logControl = new LogControl();
        var writer = new TerminalLogControlWriter(logControl);
        var config = new LogManagerConfig
        {
            RootLogger =
            {
                MinimumLevel = LogLevel.Trace,
                Writers =
                {
                    writer
                }
            }
        };

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Terminal.LogControl");
        logger.InfoMarkup("[green]danger[/] plain");
        logger.Info("second line");
        LogManager.Shutdown();

        Assert.AreEqual(2, logControl.Count);
        logControl.Search("danger");
        Assert.AreEqual(1, logControl.MatchCount);
        logControl.Search("[green]");
        Assert.AreEqual(0, logControl.MatchCount);
    }

    [TestMethod]
    public void TerminalLogControlWriter_BackgroundThread_MarshalsToUiThread()
    {
        var backend = new InMemoryTerminalBackend();
        using (global::XenoAtom.Terminal.Terminal.Open(backend, force: true))
        {
            var logControl = new LogControl();
            var writer = new TerminalLogControlWriter(logControl);
            var config = new LogManagerConfig
            {
                RootLogger =
                {
                    MinimumLevel = LogLevel.Trace,
                    Writers =
                    {
                        writer
                    }
                }
            };

            LogManager.Initialize<LogMessageSyncProcessor>(config);
            var logger = LogManager.GetLogger("Tests.Terminal.LogControl.Background");
            using var started = new ManualResetEventSlim(false);
            var worker = Task.Run(() =>
            {
                started.Wait();
                logger.Info("from worker thread");
            });

            var stopwatch = Stopwatch.StartNew();
            global::XenoAtom.Terminal.Terminal.Instance.Run(logControl, () =>
            {
                started.Set();
                if (logControl.Count > 0)
                {
                    return TerminalLoopResult.Stop;
                }

                return stopwatch.Elapsed >= TimeSpan.FromSeconds(2) ? TerminalLoopResult.Stop : TerminalLoopResult.Continue;
            });

            worker.Wait(TimeSpan.FromSeconds(2));
            LogManager.Shutdown();

            Assert.IsTrue(worker.IsCompletedSuccessfully);
            Assert.IsTrue(logControl.Count > 0);
            logControl.Search("from worker thread");
            Assert.IsTrue(logControl.MatchCount > 0);
        }
    }
}
