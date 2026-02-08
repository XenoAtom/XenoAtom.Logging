// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Tests;

[TestClass]
public class LogPropertiesEncodingTests
{
    [TestMethod]
    public void LogProperties_EnumeratesMixedEntriesInOrder()
    {
        using var properties = new LogProperties
        {
            ("Int", 42),
            ("Flag", true),
            ("Name", "Ada")
        };
        properties.Add("Span", "value".AsSpan());
        properties.Add("unnamed-value");

        var list = Read(properties);
        Assert.AreEqual(5, list.Count);
        Assert.AreEqual(("Int", "42"), list[0]);
        Assert.AreEqual(("Flag", "True"), list[1]);
        Assert.AreEqual(("Name", "Ada"), list[2]);
        Assert.AreEqual(("Span", "value"), list[3]);
        Assert.AreEqual((string.Empty, "unnamed-value"), list[4]);
    }

    [TestMethod]
    public void LogProperties_AddRange_CopiesPayload()
    {
        using var left = new LogProperties { ("A", 1) };
        using var right = new LogProperties { ("B", 2), ("C", "three") };

        left.AddRange(right);
        right.Reset();

        var leftEntries = Read(left);
        Assert.AreEqual(3, leftEntries.Count);
        Assert.AreEqual(("A", "1"), leftEntries[0]);
        Assert.AreEqual(("B", "2"), leftEntries[1]);
        Assert.AreEqual(("C", "three"), leftEntries[2]);
        Assert.AreEqual(0, right.Count);
    }

    [TestMethod]
    public void LogProperties_Reset_ClearsAllValues()
    {
        using var properties = new LogProperties { ("A", 1), ("B", "x") };
        Assert.AreEqual(2, properties.Count);

        properties.Reset();

        Assert.AreEqual(0, properties.Count);
        var entries = Read(properties);
        Assert.AreEqual(0, entries.Count);
    }

    [TestMethod]
    public void LogProperties_GrowsWhenLargeEntriesAreAdded()
    {
        using var properties = new LogProperties();
        var largeText = new string('x', 8_192);
        properties.Add("Large", largeText);
        properties.Add("Tail", "ok");

        var entries = Read(properties);
        Assert.AreEqual(2, entries.Count);
        Assert.AreEqual(("Large", largeText), entries[0]);
        Assert.AreEqual(("Tail", "ok"), entries[1]);
    }

    [TestMethod]
    public void LogProperties_CanBeReusedAfterDispose()
    {
        var properties = new LogProperties { ("A", 1) };
        properties.Dispose();

        properties.Add("B", "two");
        var entries = Read(properties);
        Assert.AreEqual(1, entries.Count);
        Assert.AreEqual(("B", "two"), entries[0]);

        properties.Dispose();
    }

    private static List<(string Name, string Value)> Read(LogProperties properties)
    {
        var list = new List<(string Name, string Value)>();
        foreach (var property in properties)
        {
            list.Add((property.Name.ToString(), property.Value.ToString()));
        }

        return list;
    }
}
