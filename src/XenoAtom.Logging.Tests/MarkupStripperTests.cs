// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using XenoAtom.Logging.Helpers;

namespace XenoAtom.Logging.Tests;

[TestClass]
public sealed class MarkupStripperTests
{
    [TestMethod]
    public void Strip_RemovesMarkupTags()
    {
        const string input = "[green]ready[/] [bold]ok[/]";
        Span<char> destination = stackalloc char[input.Length];

        var written = MarkupStripper.Strip(input, destination);
        var text = new string(destination[..written]);

        Assert.AreEqual("ready ok", text);
    }

    [TestMethod]
    public void Strip_HandlesEscapedBrackets()
    {
        const string input = "literal [[green]] value";
        Span<char> destination = stackalloc char[input.Length];

        var written = MarkupStripper.Strip(input, destination);
        var text = new string(destination[..written]);

        Assert.AreEqual("literal [green] value", text);
    }

    [TestMethod]
    public void Strip_PreservesUnclosedBracket()
    {
        const string input = "value [not-closed";
        Span<char> destination = stackalloc char[input.Length];

        var written = MarkupStripper.Strip(input, destination);
        var text = new string(destination[..written]);

        Assert.AreEqual("value [not-closed", text);
    }
}
