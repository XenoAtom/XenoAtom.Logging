// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Tests;

[TestClass]
public class LogLevelExtensionsTests
{
    [TestMethod]
    public void ToCharString_ReturnsExpectedAbbreviations()
    {
        Assert.AreEqual("T", LogLevel.Trace.ToCharString());
        Assert.AreEqual("D", LogLevel.Debug.ToCharString());
        Assert.AreEqual("I", LogLevel.Info.ToCharString());
        Assert.AreEqual("W", LogLevel.Warn.ToCharString());
        Assert.AreEqual("E", LogLevel.Error.ToCharString());
        Assert.AreEqual("F", LogLevel.Fatal.ToCharString());
    }

    [TestMethod]
    public void ToCharString_ThrowsForUnsupportedLevels()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => LogLevel.All.ToCharString());
        Assert.Throws<ArgumentOutOfRangeException>(() => LogLevel.None.ToCharString());
    }
}
