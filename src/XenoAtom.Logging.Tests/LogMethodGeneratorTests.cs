// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using XenoAtom.Logging.Generators;

namespace XenoAtom.Logging.Tests;

[TestClass]
public class LogMethodGeneratorTests
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
    public void Generator_EmitsImplementation_ForAttributedMethod()
    {
        var compilation = CreateCompilation(
            """
            using XenoAtom.Logging;

            namespace Demo;

            public static partial class GeneratedLogs
            {
                [LogMethod(LogLevel.Info, "User {userId} connected")]
                public static partial void UserConnected(Logger logger, int userId);
            }
            """);

        var driver = RunGenerator(compilation, out var outputCompilation, out var diagnostics);
        AssertNoErrors(diagnostics, "Generator diagnostics");
        AssertCompilationSuccess(outputCompilation);

        var generated = GetGeneratedSource(driver);
        Assert.IsTrue(generated.Contains("global::XenoAtom.Logging.LoggerExtensions.Info(logger", StringComparison.Ordinal));
        Assert.IsTrue(generated.Contains("$\"User {userId} connected\"", StringComparison.Ordinal));
    }

    [TestMethod]
    public void Generator_EmitsEventIdExceptionAndPropertiesPath()
    {
        var compilation = CreateCompilation(
            """
            using System;
            using XenoAtom.Logging;

            namespace Demo;

            public static partial class GeneratedLogs
            {
                [LogMethod(LogLevel.Error, "Failure for {id}", EventId = 42, EventName = "FailureEvent")]
                public static partial void Failure(Logger logger, Exception exception, LogProperties properties, int id);
            }
            """);

        var driver = RunGenerator(compilation, out var outputCompilation, out var diagnostics);
        AssertNoErrors(diagnostics, "Generator diagnostics");
        AssertCompilationSuccess(outputCompilation);

        var generated = GetGeneratedSource(driver);
        Assert.IsTrue(generated.Contains("new global::XenoAtom.Logging.LogEventId(42, \"FailureEvent\")", StringComparison.Ordinal));
        Assert.IsTrue(generated.Contains("global::XenoAtom.Logging.LoggerExtensions.Error(logger", StringComparison.Ordinal));
        Assert.IsTrue(generated.Contains("exception, properties", StringComparison.Ordinal));
    }

    [TestMethod]
    public void Analyzer_ReportsAllocationRisk_ForObjectPlaceholder()
    {
        var compilation = CreateCompilation(
            """
            using XenoAtom.Logging;

            namespace Demo;

            public static partial class GeneratedLogs
            {
                [LogMethod(LogLevel.Info, "Payload {payload}")]
                private static partial void Payload(Logger logger, object payload);
            }
            """);

        var analyzer = new LogMethodAllocationAnalyzer();
        var diagnostics = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer)).GetAnalyzerDiagnosticsAsync().GetAwaiter().GetResult();
        Assert.IsTrue(diagnostics.Any(static d => d.Id == "XLG0100"));
    }

    [TestMethod]
    public void Generator_ReportsInvalidMethodSignatureDiagnostic()
    {
        var compilation = CreateCompilation(
            """
            using XenoAtom.Logging;

            namespace Demo;

            public partial class GeneratedLogs
            {
                [LogMethod(LogLevel.Info, "Message")]
                private partial void NotStatic(Logger logger);
            }
            """);

        _ = RunGenerator(compilation, out _, out var diagnostics);
        AssertHasDiagnostic(diagnostics, "XLG0001");
    }

    [TestMethod]
    public void Generator_ReportsInvalidLogLevelDiagnostic()
    {
        var compilation = CreateCompilation(
            """
            using XenoAtom.Logging;

            namespace Demo;

            public static partial class GeneratedLogs
            {
                [LogMethod((LogLevel)999, "Message")]
                private static partial void InvalidLevel(Logger logger);
            }
            """);

        _ = RunGenerator(compilation, out _, out var diagnostics);
        AssertHasDiagnostic(diagnostics, "XLG0002");
    }

    [TestMethod]
    public void Generator_ReportsTemplateParseDiagnostic()
    {
        var compilation = CreateCompilation(
            """
            using XenoAtom.Logging;

            namespace Demo;

            public static partial class GeneratedLogs
            {
                [LogMethod(LogLevel.Info, "User {userId")]
                private static partial void InvalidTemplate(Logger logger, int userId);
            }
            """);

        _ = RunGenerator(compilation, out _, out var diagnostics);
        AssertHasDiagnostic(diagnostics, "XLG0003");
    }

    [TestMethod]
    public void Generator_ReportsTemplateParameterNotFoundDiagnostic()
    {
        var compilation = CreateCompilation(
            """
            using XenoAtom.Logging;

            namespace Demo;

            public static partial class GeneratedLogs
            {
                [LogMethod(LogLevel.Info, "User {missing}")]
                private static partial void MissingPlaceholder(Logger logger, int userId);
            }
            """);

        _ = RunGenerator(compilation, out _, out var diagnostics);
        AssertHasDiagnostic(diagnostics, "XLG0004");
    }

    [TestMethod]
    public void GeneratedMethod_ExecutesAtRuntime()
    {
        var compilation = CreateCompilation(
            """
            using XenoAtom.Logging;

            namespace Demo;

            public static partial class GeneratedLogs
            {
                [LogMethod(LogLevel.Info, "User {userId} connected")]
                public static partial void UserConnected(Logger logger, int userId);
            }

            public static class RuntimeInvoker
            {
                public static void Invoke(Logger logger, int userId)
                {
                    GeneratedLogs.UserConnected(logger, userId);
                }
            }
            """);

        var driver = RunGenerator(compilation, out var outputCompilation, out var diagnostics);
        AssertNoErrors(diagnostics, "Generator diagnostics");
        AssertCompilationSuccess(outputCompilation);
        Assert.IsNotNull(GetGeneratedSource(driver));

        var runtimeAssembly = EmitAssembly(outputCompilation);
        var invokerType = runtimeAssembly.GetType("Demo.RuntimeInvoker");
        Assert.IsNotNull(invokerType);
        var invokeMethod = invokerType!.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Static);
        Assert.IsNotNull(invokeMethod);

        var writer = new RuntimeCaptureWriter();
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

        LogManager.Shutdown();
        LogManager.Initialize<LogMessageSyncProcessor>(config);
        var logger = LogManager.GetLogger("Tests.Generated.Runtime");

        invokeMethod!.Invoke(null, [logger, 42]);

        LogManager.Shutdown();
        Assert.AreEqual(1, writer.Messages.Count);
        Assert.AreEqual("User 42 connected", writer.Messages[0]);
    }

    private static GeneratorDriver RunGenerator(
        CSharpCompilation compilation,
        out CSharpCompilation outputCompilation,
        out ImmutableArray<Diagnostic> diagnostics)
    {
        var parseOptions = (CSharpParseOptions)compilation.SyntaxTrees[0].Options;
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [new LogMethodGenerator().AsSourceGenerator()],
            parseOptions: parseOptions);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out diagnostics);
        outputCompilation = (CSharpCompilation)updatedCompilation;
        return driver;
    }

    private static string GetGeneratedSource(GeneratorDriver driver)
    {
        var runResult = driver.GetRunResult();
        Assert.AreEqual(1, runResult.Results.Length);
        Assert.AreEqual(1, runResult.Results[0].GeneratedSources.Length);
        return runResult.Results[0].GeneratedSources[0].SourceText.ToString();
    }

    private static CSharpCompilation CreateCompilation(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            source,
            CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview));

        return CSharpCompilation.Create(
            assemblyName: "XenoAtom.Logging.GeneratorTests.Compilation",
            syntaxTrees: [syntaxTree],
            references: GetMetadataReferences(),
            options: new CSharpCompilationOptions(
                outputKind: OutputKind.DynamicallyLinkedLibrary,
                nullableContextOptions: NullableContextOptions.Enable));
    }

    private static IReadOnlyList<MetadataReference> GetMetadataReferences()
    {
        var references = new List<MetadataReference>();
        var trustedAssemblies = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
        if (!string.IsNullOrEmpty(trustedAssemblies))
        {
            foreach (var assemblyPath in trustedAssemblies.Split(Path.PathSeparator))
            {
                references.Add(MetadataReference.CreateFromFile(assemblyPath));
            }
        }

        references.Add(MetadataReference.CreateFromFile(typeof(Logger).Assembly.Location));
        return references;
    }

    private static void AssertNoErrors(IEnumerable<Diagnostic> diagnostics, string name)
    {
        var errors = diagnostics.Where(static x => x.Severity == DiagnosticSeverity.Error).ToArray();
        Assert.AreEqual(0, errors.Length, $"{name}{Environment.NewLine}{string.Join(Environment.NewLine, errors.Select(static x => x.ToString()))}");
    }

    private static void AssertCompilationSuccess(Compilation compilation)
    {
        using var stream = new MemoryStream();
        var emitResult = compilation.Emit(stream);
        if (!emitResult.Success)
        {
            var text = string.Join(Environment.NewLine, emitResult.Diagnostics.Select(static x => x.ToString()));
            Assert.Fail(text);
        }
    }

    private static Assembly EmitAssembly(Compilation compilation)
    {
        using var stream = new MemoryStream();
        var emitResult = compilation.Emit(stream);
        if (!emitResult.Success)
        {
            var text = string.Join(Environment.NewLine, emitResult.Diagnostics.Select(static x => x.ToString()));
            Assert.Fail(text);
        }

        return Assembly.Load(stream.ToArray());
    }

    private static void AssertHasDiagnostic(IEnumerable<Diagnostic> diagnostics, string id)
    {
        if (!diagnostics.Any(diagnostic => diagnostic.Id == id))
        {
            var text = string.Join(Environment.NewLine, diagnostics.Select(static x => x.ToString()));
            Assert.Fail($"Expected diagnostic {id}.{Environment.NewLine}{text}");
        }
    }

    private sealed class RuntimeCaptureWriter : LogWriter
    {
        public List<string> Messages { get; } = [];

        protected override void Log(in LogMessage logMessage)
        {
            Messages.Add(logMessage.Text.ToString());
        }
    }
}
