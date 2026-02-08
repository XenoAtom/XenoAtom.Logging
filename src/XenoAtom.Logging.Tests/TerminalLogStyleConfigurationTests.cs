// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using XenoAtom.Logging.Writers;

namespace XenoAtom.Logging.Tests;

[TestClass]
public class TerminalLogStyleConfigurationTests
{
    [TestMethod]
    public void TerminalLogStyleConfiguration_DefaultStyles_AreApplied()
    {
        var styles = new TerminalLogStyleConfiguration();

        Assert.AreEqual("gray", styles.GetStyle(LogMessageFormatSegmentKind.Timestamp));
        Assert.AreEqual("blue", styles.GetStyle(LogMessageFormatSegmentKind.LoggerName));
        Assert.AreEqual("magenta", styles.GetStyle(LogMessageFormatSegmentKind.EventId));
        Assert.AreEqual("bold red", styles.GetStyle(LogMessageFormatSegmentKind.Exception));
        Assert.AreEqual("dim", styles.GetLevelStyle(LogLevel.Trace));
        Assert.AreEqual("cyan", styles.GetLevelStyle(LogLevel.Debug));
        Assert.AreEqual("green", styles.GetLevelStyle(LogLevel.Info));
        Assert.AreEqual("bold yellow", styles.GetLevelStyle(LogLevel.Warn));
        Assert.AreEqual("bold red", styles.GetLevelStyle(LogLevel.Error));
        Assert.AreEqual("bold white on red", styles.GetLevelStyle(LogLevel.Fatal));
    }

    [TestMethod]
    public void TerminalLogStyleConfiguration_Clear_RemovesStyles()
    {
        var styles = new TerminalLogStyleConfiguration();
        styles.Clear();

        Assert.IsNull(styles.GetStyle(LogMessageFormatSegmentKind.Timestamp));
        Assert.IsNull(styles.GetStyle(LogMessageFormatSegmentKind.LoggerName));
        Assert.IsNull(styles.GetLevelStyle(LogLevel.Info));
        Assert.IsNull(styles.GetLevelStyle(LogLevel.Error));
    }

    [TestMethod]
    public void TerminalLogStyleConfiguration_ResetToDefaults_RestoresStyles()
    {
        var styles = new TerminalLogStyleConfiguration();
        styles.Clear();
        styles.SetStyle(LogMessageFormatSegmentKind.Timestamp, "red");
        styles.SetLevelStyle(LogLevel.Info, "underline");

        styles.ResetToDefaults();

        Assert.AreEqual("gray", styles.GetStyle(LogMessageFormatSegmentKind.Timestamp));
        Assert.AreEqual("green", styles.GetLevelStyle(LogLevel.Info));
    }

    [TestMethod]
    public void TerminalLogStyleConfiguration_SetLevelStyle_RejectsUnsupportedLevel()
    {
        var styles = new TerminalLogStyleConfiguration();

        Assert.Throws<ArgumentOutOfRangeException>(() => styles.SetLevelStyle(LogLevel.All, "red"));
        Assert.Throws<ArgumentOutOfRangeException>(() => styles.SetLevelStyle(LogLevel.None, "red"));
    }
}
