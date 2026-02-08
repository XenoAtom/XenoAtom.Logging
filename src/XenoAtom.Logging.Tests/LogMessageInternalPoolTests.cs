// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Reflection;

namespace XenoAtom.Logging.Tests;

[TestClass]
public class LogMessageInternalPoolTests
{
    [TestMethod]
    public void DoubleReturn_DoesNotCorruptPool()
    {
        var assembly = typeof(LogManager).Assembly;
        var poolType = assembly.GetType("XenoAtom.Logging.LogMessageInternalPool", throwOnError: true)!;
        var messageType = assembly.GetType("XenoAtom.Logging.LogMessageInternal", throwOnError: true)!;

        var pool = Activator.CreateInstance(poolType, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, binder: null, args: [1, 4096], culture: null);
        Assert.IsNotNull(pool);

        var returnMethod = poolType.GetMethod("Return", BindingFlags.Instance | BindingFlags.Public)!;
        var tryRentMethod = poolType.GetMethod("TryRent", BindingFlags.Instance | BindingFlags.Public)!;
        var message = Activator.CreateInstance(messageType, nonPublic: true);
        Assert.IsNotNull(message);

        returnMethod.Invoke(pool, [message]);
        returnMethod.Invoke(pool, [message]);

        var rentTask = Task.Run(() => tryRentMethod.Invoke(pool, null));
        Assert.IsTrue(rentTask.Wait(TimeSpan.FromSeconds(1)), "TryRent should not spin after a double return.");
        Assert.IsNotNull(rentTask.Result);
    }
}
