// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Tests;

[TestClass]
public class LogScopeAndPropertiesTests
{
    [TestInitialize]
    public void Initialize()
    {
        LogManager.Shutdown();
    }

    [TestCleanup]
    public void Cleanup()
    {
        LogManager.Shutdown();
    }

    [TestMethod]
    public void MessageProperties_AreCaptured()
    {
        var writer = new ScopeAndPropertiesWriter();
        var config = CreateConfig(writer);
        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Properties");

        var properties = new LogProperties
        {
            ("UserId", 42),
            ("Name", "Ada"),
        };

        logger.Info("hello", properties);

        Assert.AreEqual(1, writer.Messages.Count);
        var message = writer.Messages[0];
        Assert.AreEqual(2, message.Properties.Length);
        Assert.AreEqual("UserId", message.Properties[0].Name);
        Assert.AreEqual("42", message.Properties[0].Value);
        Assert.AreEqual("Name", message.Properties[1].Name);
        Assert.AreEqual("Ada", message.Properties[1].Value);
    }

    [TestMethod]
    public void NestedScopes_AreCaptured()
    {
        var writer = new ScopeAndPropertiesWriter();
        var config = CreateConfig(writer);
        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Scope");

        var scopeA = new LogProperties { ("RequestId", 123) };
        var scopeB = new LogProperties { ("Operation", "Import") };

        using (logger.BeginScope(scopeA))
        using (logger.BeginScope(scopeB))
        {
            logger.Info("scoped");
        }

        Assert.AreEqual(1, writer.Messages.Count);
        var message = writer.Messages[0];
        Assert.AreEqual(2, message.Scopes.Length);
        Assert.AreEqual("RequestId", message.Scopes[0][0].Name);
        Assert.AreEqual("123", message.Scopes[0][0].Value);
        Assert.AreEqual("Operation", message.Scopes[1][0].Name);
        Assert.AreEqual("Import", message.Scopes[1][0].Value);
    }

    [TestMethod]
    public void PropertiesReader_Contains_MatchesEntry()
    {
        var writer = new ContainsWriter("UserId", "42");
        var config = CreateConfig(writer);
        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Properties.Contains");

        var properties = new LogProperties { ("UserId", 42), ("Name", "Ada") };
        logger.Info("hello", properties);

        Assert.AreEqual(1, writer.MatchCount);
    }

    [TestMethod]
    public void NonInterpolatedOverloads_WithProperties_CaptureForAllLevels()
    {
        var writer = new ScopeAndPropertiesWriter();
        var config = CreateConfig(writer);
        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Properties.AllLevels");

        logger.Trace("trace", new LogProperties { ("Level", "Trace") });
        logger.Debug("debug", new LogProperties { ("Level", "Debug") });
        logger.Info("info", new LogProperties { ("Level", "Info") });
        logger.Warn("warn", new LogProperties { ("Level", "Warn") });
        logger.Error("error", new LogProperties { ("Level", "Error") });
        logger.Fatal("fatal", new LogProperties { ("Level", "Fatal") });

        Assert.AreEqual(6, writer.Messages.Count);
        CollectionAssert.AreEqual(
            new[] { "Trace", "Debug", "Info", "Warn", "Error", "Fatal" },
            writer.Messages.Select(static x => x.Properties[0].Value).ToArray());
    }

    [TestMethod]
    public void OutOfOrderScopeDispose_DoesNotDropActiveScopeStack()
    {
        var writer = new ScopeAndPropertiesWriter();
        LogManager.Initialize<LogMessageSyncProcessor>(CreateConfig(writer));
        var logger = LogManager.GetLogger("Tests.Scope.OutOfOrder");

        var requestScope = logger.BeginScope(new LogProperties { ("RequestId", 123) });
        var deferredRequestScope = requestScope;
        var operationScope = logger.BeginScope(new LogProperties { ("Operation", "Import") });

        requestScope.Dispose(); // Out-of-order dispose must not clear active stack.
        logger.Info("before-pop");

        operationScope.Dispose();
        logger.Info("after-inner-pop");

        deferredRequestScope.Dispose();
        logger.Info("after-all-pop");

        Assert.AreEqual(3, writer.Messages.Count);
        Assert.AreEqual(2, writer.Messages[0].Scopes.Length);
        Assert.AreEqual(1, writer.Messages[1].Scopes.Length);
        Assert.AreEqual(0, writer.Messages[2].Scopes.Length);
    }

    private static LogManagerConfig CreateConfig(LogWriter writer)
    {
        return new LogManagerConfig
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
    }

    private sealed class ScopeAndPropertiesWriter : LogWriter
    {
        public List<CapturedMessage> Messages { get; } = [];

        protected override void Log(in LogMessage logMessage)
        {
            var properties = ReadProperties(logMessage.Properties);
            var scopes = ReadScopes(logMessage.Scope);
            Messages.Add(new CapturedMessage(logMessage.Text.ToString(), properties, scopes));
        }

        private static CapturedProperty[] ReadProperties(LogPropertiesReader reader)
        {
            var list = new List<CapturedProperty>(reader.Count);
            foreach (var property in reader)
            {
                list.Add(new CapturedProperty(property.Name.ToString(), property.Value.ToString()));
            }
            return list.ToArray();
        }

        private static CapturedProperty[][] ReadScopes(LogScope scope)
        {
            var list = new List<CapturedProperty[]>(scope.Count);
            for (var i = 0; i < scope.Count; i++)
            {
                list.Add(ReadProperties(scope[i]));
            }
            return list.ToArray();
        }
    }

    private sealed class ContainsWriter : LogWriter
    {
        private readonly string _name;
        private readonly string _value;

        public ContainsWriter(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public int MatchCount { get; private set; }

        protected override void Log(in LogMessage logMessage)
        {
            if (logMessage.Properties.Contains(_name, _value))
            {
                MatchCount++;
            }
        }
    }

    private readonly record struct CapturedProperty(string Name, string Value);

    private readonly record struct CapturedMessage(string Text, CapturedProperty[] Properties, CapturedProperty[][] Scopes);
}
