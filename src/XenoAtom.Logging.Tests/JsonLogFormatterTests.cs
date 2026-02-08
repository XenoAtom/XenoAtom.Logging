// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Text.Json;
using XenoAtom.Logging.Formatters;
using XenoAtom.Logging.Helpers;

namespace XenoAtom.Logging.Tests;

[TestClass]
public class JsonLogFormatterTests
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
    public void JsonLogFormatter_EmitsExpectedPayload()
    {
        var writer = new JsonCaptureWriter(new JsonLogFormatter(includeProperties: true, includeScopes: true));
        var config = CreateConfig(writer);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Json.Formatter");

        var scopeProperties = new LogProperties { ("RequestId", 123) };
        var messageProperties = new LogProperties { ("User", "Ada"), ("Input", "\"quoted\"\nline") };
        using (logger.BeginScope(scopeProperties))
        {
            logger.Error(new LogEventId(10, "Failure"), new InvalidOperationException("boom"), "Hello \"json\"", messageProperties);
        }

        Assert.AreEqual(1, writer.Messages.Count);

        using var document = JsonDocument.Parse(writer.Messages[0]);
        var root = document.RootElement;

        Assert.AreEqual("Tests.Json.Formatter", root.GetProperty("logger").GetString());
        Assert.AreEqual("Error", root.GetProperty("level").GetString());
        Assert.AreEqual("Hello \"json\"", root.GetProperty("message").GetString());
        Assert.AreEqual(10, root.GetProperty("eventId").GetProperty("id").GetInt32());
        Assert.AreEqual("Failure", root.GetProperty("eventId").GetProperty("name").GetString());
        Assert.IsTrue(root.GetProperty("exception").GetString()!.Contains("InvalidOperationException", StringComparison.Ordinal));

        var properties = root.GetProperty("properties");
        Assert.AreEqual(2, properties.GetArrayLength());
        Assert.AreEqual("User", properties[0].GetProperty("name").GetString());
        Assert.AreEqual("Ada", properties[0].GetProperty("value").GetString());
        Assert.AreEqual("Input", properties[1].GetProperty("name").GetString());
        Assert.AreEqual("\"quoted\"\nline", properties[1].GetProperty("value").GetString());

        var scopes = root.GetProperty("scopes");
        Assert.AreEqual(1, scopes.GetArrayLength());
        Assert.AreEqual(1, scopes[0].GetArrayLength());
        Assert.AreEqual("RequestId", scopes[0][0].GetProperty("name").GetString());
        Assert.AreEqual("123", scopes[0][0].GetProperty("value").GetString());
    }

    [TestMethod]
    public void JsonLogFormatter_CanDisablePropertiesAndScopes()
    {
        var writer = new JsonCaptureWriter(new JsonLogFormatter(includeProperties: false, includeScopes: false));
        var config = CreateConfig(writer);
        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Json.Minimal");
        var properties = new LogProperties { ("UserId", 42) };

        using (logger.BeginScope(new LogProperties { ("RequestId", 99) }))
        {
            logger.Info("minimal", properties);
        }

        Assert.AreEqual(1, writer.Messages.Count);
        using var document = JsonDocument.Parse(writer.Messages[0]);
        var root = document.RootElement;

        Assert.IsFalse(root.TryGetProperty("properties", out _));
        Assert.IsFalse(root.TryGetProperty("scopes", out _));
    }

    [TestMethod]
    public void JsonLogFormatter_DefaultSchema_CanUseSnakeCaseFields()
    {
        var writer = new JsonCaptureWriter(new JsonLogFormatter(new JsonLogFormatterOptions
        {
            FieldNamingPolicy = JsonLogFieldNamingPolicy.SnakeCase
        }));
        var config = CreateConfig(writer);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Json.SnakeCase");
        logger.Info(new LogEventId(99, "SnakeEvent"), "snake-case");

        Assert.AreEqual(1, writer.Messages.Count);

        using var document = JsonDocument.Parse(writer.Messages[0]);
        var root = document.RootElement;

        Assert.IsTrue(root.TryGetProperty("thread_id", out var threadIdElement));
        Assert.IsFalse(root.TryGetProperty("threadId", out _));
        Assert.IsTrue(threadIdElement.GetInt32() > 0);

        var eventElement = root.GetProperty("event_id");
        Assert.AreEqual(99, eventElement.GetProperty("id").GetInt32());
        Assert.AreEqual("SnakeEvent", eventElement.GetProperty("name").GetString());
    }

    [TestMethod]
    public void JsonLogFormatter_EcsSchema_UsesFlattenedFieldNames()
    {
        var writer = new JsonCaptureWriter(new JsonLogFormatter(new JsonLogFormatterOptions
        {
            SchemaProfile = JsonLogSchemaProfile.ElasticCommonSchema
        }));
        var config = CreateConfig(writer);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Json.Ecs");
        logger.Error(new LogEventId(7, "UserEvent"), new InvalidOperationException("boom"), "ecs-message", new LogProperties { ("User", "Ada") });

        Assert.AreEqual(1, writer.Messages.Count);
        using var document = JsonDocument.Parse(writer.Messages[0]);
        var root = document.RootElement;

        Assert.AreEqual("Error", root.GetProperty("log.level").GetString());
        Assert.AreEqual("Tests.Json.Ecs", root.GetProperty("log.logger").GetString());
        Assert.AreEqual(7, root.GetProperty("event.code").GetInt32());
        Assert.AreEqual("UserEvent", root.GetProperty("event.action").GetString());
        Assert.IsTrue(root.GetProperty("error.stack_trace").GetString()!.Contains("InvalidOperationException", StringComparison.Ordinal));
        Assert.AreEqual(1, root.GetProperty("labels").GetArrayLength());
        Assert.IsTrue(root.TryGetProperty("@timestamp", out _));
        Assert.IsTrue(root.TryGetProperty("process.thread.id", out _));
    }

    [TestMethod]
    public void JsonLogFormatter_EcsSchema_IgnoresFieldNamingPolicy()
    {
        var writer = new JsonCaptureWriter(new JsonLogFormatter(new JsonLogFormatterOptions
        {
            SchemaProfile = JsonLogSchemaProfile.ElasticCommonSchema,
            FieldNamingPolicy = JsonLogFieldNamingPolicy.SnakeCase
        }));
        var config = CreateConfig(writer);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Json.Ecs.Naming");
        logger.Info("ecs-naming");

        Assert.AreEqual(1, writer.Messages.Count);
        using var document = JsonDocument.Parse(writer.Messages[0]);
        var root = document.RootElement;

        Assert.IsTrue(root.TryGetProperty("log.level", out _));
        Assert.IsFalse(root.TryGetProperty("log_level", out _));
        Assert.IsTrue(root.TryGetProperty("process.thread.id", out _));
        Assert.IsFalse(root.TryGetProperty("process_thread_id", out _));
    }

    [TestMethod]
    public void JsonLogFormatter_CanDisableOptionalFields()
    {
        var writer = new JsonCaptureWriter(new JsonLogFormatter(new JsonLogFormatterOptions
        {
            IncludeThreadId = false,
            IncludeEventId = false,
            IncludeException = false
        }));
        var config = CreateConfig(writer);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Json.Toggle");
        logger.Error(new LogEventId(11, "Ignored"), new InvalidOperationException("boom"), "toggle");

        Assert.AreEqual(1, writer.Messages.Count);
        using var document = JsonDocument.Parse(writer.Messages[0]);
        var root = document.RootElement;

        Assert.IsFalse(root.TryGetProperty("threadId", out _));
        Assert.IsFalse(root.TryGetProperty("eventId", out _));
        Assert.IsFalse(root.TryGetProperty("exception", out _));
    }

    [TestMethod]
    public void JsonLogFormatter_DoesNotEmitInternalTransportProperties()
    {
        var writer = new JsonCaptureWriter(new JsonLogFormatter(includeProperties: true, includeScopes: false));
        var config = CreateConfig(writer);

        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Json.InternalProperties");
        logger.InfoMarkup("[green]ok[/]");

        Assert.AreEqual(1, writer.Messages.Count);
        using var document = JsonDocument.Parse(writer.Messages[0]);
        var root = document.RootElement;

        Assert.IsTrue(root.TryGetProperty("properties", out var properties));
        Assert.AreEqual(0, properties.GetArrayLength());
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

    private sealed class JsonCaptureWriter : LogWriter
    {
        private readonly JsonLogFormatter _formatter;

        public JsonCaptureWriter(JsonLogFormatter formatter)
        {
            _formatter = formatter;
        }

        public List<string> Messages { get; } = [];

        protected override void Log(in LogMessage logMessage)
        {
            using var formatterBuffer = new LogFormatterBuffer();
            var segments = new LogMessageFormatSegments(false);
            try
            {
                var text = formatterBuffer.Format(logMessage, _formatter, ref segments);
                Messages.Add(text.ToString());
            }
            finally
            {
                segments.Dispose();
            }
        }
    }
}
