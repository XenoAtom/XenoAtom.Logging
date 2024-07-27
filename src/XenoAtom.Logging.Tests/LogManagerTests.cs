// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;
using XenoAtom.Logging;
using XenoAtom.Logging.Writers;

namespace XenoAtom.Logging.Tests;

[TestClass]
public class LogManagerTests
{
    public TestContext TestContext { get; set; }

    [TestMethod]
    public unsafe void TestSimple()
    {
        var config = new LogManagerConfig()
        {
            RootLogger =
            {
                MinimumLevel = LogLevel.Info,
                Writers =
                {
                    new ConsoleLogWriter()
                }
            },
            Loggers =
            {
                { "hello", LogLevel.Error },
                { "hello", LogLevel.Info, [new ConsoleLogWriter()]}
            }
        };

        LogManager.Initialize(config);


        var helloConfig = config.GetLoggerConfig("hello");
        helloConfig.MinimumLevel = LogLevel.Error;
        helloConfig.Writers.Add(new ConsoleLogWriter());

        Logger log = LogManager.GetLogger("hello");

        int x = 0;
        var test = "World";
        var utf8Span = "hello world"u8;

        using (log.BeginScope(["Hello", 126, 12.0f]))
        {
            log.Info($"Hel{x}l {true} {true,6} {LogLevel.Info}o {test} another {utf8Span}");
        }

        var array = GC.AllocateArray<object>(1024, true);
        var ptrArray = *(object**)(&array);
        ref var str = ref array[0];
        var ptr = (nint)(void*)Unsafe.AsPointer(ref str);
        TestContext.WriteLine($"Base 0x{(nint)ptrArray:X16}");
        TestContext.WriteLine($"0x{ptr:X16}");

        GC.Collect();
        GC.Collect();
        GC.Collect();

        var ptr2 = (nint)(void*)Unsafe.AsPointer(ref str);
        TestContext.WriteLine($"0x{ptr2:X16}");
    }
}
