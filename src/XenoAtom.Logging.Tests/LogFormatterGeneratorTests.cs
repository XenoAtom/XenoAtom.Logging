// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using XenoAtom.Logging.Generators;

namespace XenoAtom.Logging.Tests;

[TestClass]
public class LogFormatterGeneratorTests
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
    public void Generator_EmitsImplementation_ForFormatterType()
    {
        var compilation = CreateCompilation(
            """
            using System;
            using XenoAtom.Logging;

            namespace XenoAtom.Logging.Demo;

            [LogFormatter("{Timestamp:HH:mm:ss} {Level,-5} {Text}")]
            public sealed partial record DemoFormatter : LogFormatter;
            """);

        var driver = RunGenerator(compilation, out var outputCompilation, out var diagnostics);
        AssertNoErrors(diagnostics, "Generator diagnostics");
        AssertCompilationSuccess(outputCompilation);

        var generated = GetSingleGeneratedSource(driver);
        Assert.IsTrue(generated.Contains("public static DemoFormatter Instance { get; } = new();", StringComparison.Ordinal));
        Assert.IsTrue(generated.Contains("public DemoFormatter() : base(global::XenoAtom.Logging.LogLevelFormat.Tri, \"HH:mm:ss\")", StringComparison.Ordinal));
        Assert.IsTrue(generated.Contains("public override bool TryFormat(", StringComparison.Ordinal));
    }

    [TestMethod]
    public void Generator_EmitsImplementation_ForFormatterProperty()
    {
        var compilation = CreateCompilation(
            """
            using System;
            using XenoAtom.Logging;

            namespace XenoAtom.Logging.Demo;

            public static partial class FormatterCatalog
            {
                [LogFormatter("{Timestamp:HH:mm:ss} {Level} {Text}")]
                public static partial LogFormatter Compact { get; }
            }
            """);

        var driver = RunGenerator(compilation, out var outputCompilation, out var diagnostics);
        AssertNoErrors(diagnostics, "Generator diagnostics");
        AssertCompilationSuccess(outputCompilation);

        var generated = GetSingleGeneratedSource(driver);
        Assert.IsTrue(generated.Contains("private sealed partial record __CompactGeneratedFormatter", StringComparison.Ordinal));
        Assert.IsTrue(generated.Contains("static partial global::XenoAtom.Logging.LogFormatter Compact => __CompactGeneratedFormatter.Instance;", StringComparison.Ordinal));
    }

    [TestMethod]
    public void Generator_ReportsUnknownFieldDiagnostic()
    {
        var compilation = CreateCompilation(
            """
            using System;
            using XenoAtom.Logging;

            namespace XenoAtom.Logging.Demo;

            [LogFormatter("{Unknown}")]
            public sealed partial record BadFormatter : LogFormatter;
            """);

        _ = RunGenerator(compilation, out _, out var diagnostics);
        AssertHasDiagnostic(diagnostics, "XLF0001");
    }

    [TestMethod]
    public void Generator_ReportsMalformedTemplateDiagnostic()
    {
        var compilation = CreateCompilation(
            """
            using System;
            using XenoAtom.Logging;

            namespace XenoAtom.Logging.Demo;

            [LogFormatter("{Timestamp")]
            public sealed partial record BadFormatter : LogFormatter;
            """);

        _ = RunGenerator(compilation, out _, out var diagnostics);
        AssertHasDiagnostic(diagnostics, "XLF0002");
    }

    [TestMethod]
    public void Generator_ReportsInvalidFieldFormatDiagnostic()
    {
        var compilation = CreateCompilation(
            """
            using System;
            using XenoAtom.Logging;

            namespace XenoAtom.Logging.Demo;

            [LogFormatter("{Level:invalid}")]
            public sealed partial record BadFormatter : LogFormatter;
            """);

        _ = RunGenerator(compilation, out _, out var diagnostics);
        AssertHasDiagnostic(diagnostics, "XLF0003");
    }

    [TestMethod]
    public void Generator_ReportsInvalidTypeUsageDiagnostic()
    {
        var compilation = CreateCompilation(
            """
            using System;
            using XenoAtom.Logging;

            namespace XenoAtom.Logging.Demo;

            [LogFormatter("{Text}")]
            public partial class NotAFormatter
            {
            }
            """);

        _ = RunGenerator(compilation, out _, out var diagnostics);
        AssertHasDiagnostic(diagnostics, "XLF0004");
    }

    [TestMethod]
    public void Generator_ReportsInvalidPropertyUsageDiagnostic()
    {
        var compilation = CreateCompilation(
            """
            using System;
            using XenoAtom.Logging;

            namespace XenoAtom.Logging.Demo;

            public static class FormatterCatalog
            {
                [LogFormatter("{Text}")]
                public static LogFormatter Bad { get; } = null!;
            }
            """);

        _ = RunGenerator(compilation, out _, out var diagnostics);
        AssertHasDiagnostic(diagnostics, "XLF0005");
    }

    [TestMethod]
    public void Generator_ReportsConditionalAlwaysEmittedWarning()
    {
        var compilation = CreateCompilation(
            """
            using System;
            using XenoAtom.Logging;

            namespace XenoAtom.Logging.Demo;

            [LogFormatter("{? [{Level}] ?}{Text}")]
            public sealed partial record DemoFormatter : LogFormatter;
            """);

        _ = RunGenerator(compilation, out _, out var diagnostics);
        AssertHasDiagnostic(diagnostics, "XLF0006", DiagnosticSeverity.Warning);
    }

    [TestMethod]
    public void GeneratedFormatter_FormatsAtRuntime()
    {
        var compilation = CreateCompilation(
            """
            using System;
            using XenoAtom.Logging;

            namespace XenoAtom.Logging.Demo;

            [LogFormatter("{Timestamp:HH:mm:ss} {Level,-5} {Text}")]
            public sealed partial record DemoFormatter : LogFormatter;

            public sealed class CaptureWriter : LogWriter
            {
                public string? LastMessage { get; private set; }

                public LogFormatter Formatter { get; } =
                    DemoFormatter.Instance with { LevelFormat = LogLevelFormat.Short, TimestampFormat = "HH:mm:ss" };

                protected override void Log(LogMessage logMessage)
                {
                    var segments = new LogMessageFormatSegments(false);
                    try
                    {
                        var buffer = new char[256];
                        if (!Formatter.TryFormat(logMessage, buffer, out var written, ref segments))
                        {
                            throw new InvalidOperationException("Formatting failed.");
                        }

                        LastMessage = new string(buffer[..written]);
                    }
                    finally
                    {
                        segments.Dispose();
                    }
                }
            }

            public static class RuntimeInvoker
            {
                public static string Invoke()
                {
                    var writer = new CaptureWriter();
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

                    LogManager.Initialize(config);
                    var logger = LogManager.GetLogger("Demo.Runtime");
                    logger.Info("hello");
                    LogManager.Shutdown();
                    return writer.LastMessage ?? string.Empty;
                }
            }
            """);

        var driver = RunGenerator(compilation, out var outputCompilation, out var diagnostics);
        AssertNoErrors(diagnostics, "Generator diagnostics");
        AssertCompilationSuccess(outputCompilation);
            Assert.AreEqual(1, GetGeneratedSources(driver).Count);

            var runtimeAssembly = EmitAssembly(outputCompilation);
            var invokerType = runtimeAssembly.GetType("XenoAtom.Logging.Demo.RuntimeInvoker");
        Assert.IsNotNull(invokerType);
        var invokeMethod = invokerType!.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Static);
        Assert.IsNotNull(invokeMethod);

        var text = (string?)invokeMethod!.Invoke(null, null);
        Assert.IsFalse(string.IsNullOrEmpty(text));
        Assert.IsTrue(text!.Contains("INFO", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("hello", StringComparison.Ordinal));
    }

    private static GeneratorDriver RunGenerator(
        CSharpCompilation compilation,
        out CSharpCompilation outputCompilation,
        out ImmutableArray<Diagnostic> diagnostics)
    {
        var parseOptions = (CSharpParseOptions)compilation.SyntaxTrees[0].Options;
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [new LogFormatterGenerator().AsSourceGenerator()],
            parseOptions: parseOptions);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out diagnostics);
        outputCompilation = (CSharpCompilation)updatedCompilation;
        return driver;
    }

    private static string GetSingleGeneratedSource(GeneratorDriver driver)
    {
        var generatedSources = GetGeneratedSources(driver);
        Assert.AreEqual(1, generatedSources.Count);
        return generatedSources[0];
    }

    private static IReadOnlyList<string> GetGeneratedSources(GeneratorDriver driver)
    {
        var runResult = driver.GetRunResult();
        var result = new List<string>();
        foreach (var generatorResult in runResult.Results)
        {
            foreach (var source in generatorResult.GeneratedSources)
            {
                result.Add(source.SourceText.ToString());
            }
        }

        return result;
    }

    private static CSharpCompilation CreateCompilation(string source)
    {
        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
        var syntaxTree = CSharpSyntaxTree.ParseText(
            source,
            parseOptions);
        var globalUsingsTree = CSharpSyntaxTree.ParseText(
            """
            global using System;
            global using XenoAtom.Logging;
            """,
            parseOptions);

        return CSharpCompilation.Create(
            assemblyName: "XenoAtom.Logging.FormatterGeneratorTests.Compilation",
            syntaxTrees: [globalUsingsTree, syntaxTree],
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

    private static void AssertHasDiagnostic(IEnumerable<Diagnostic> diagnostics, string id, DiagnosticSeverity? severity = null)
    {
        foreach (var diagnostic in diagnostics)
        {
            if (diagnostic.Id == id && (severity is null || diagnostic.Severity == severity.Value))
            {
                return;
            }
        }

        var text = string.Join(Environment.NewLine, diagnostics.Select(static x => x.ToString()));
        Assert.Fail($"Expected diagnostic {id}.{Environment.NewLine}{text}");
    }
}
