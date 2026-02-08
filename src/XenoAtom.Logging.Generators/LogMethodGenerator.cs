// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XenoAtom.Logging.Generators;

/// <summary>
/// Generates implementations for methods annotated with <c>[LogMethod]</c>.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class LogMethodGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var generatedMethods = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                LogMethodUtilities.LogMethodAttributeMetadataName,
                static (node, _) => node is MethodDeclarationSyntax { AttributeLists.Count: > 0 },
                static (ctx, ct) => TryCreateGenerationResult(ctx, ct))
            .Where(static result => result is not null)
            .Select(static (result, _) => result!.Value)
            .WithComparer(LogMethodGenerationResultComparer.Instance);

        context.RegisterSourceOutput(
            generatedMethods,
            static (spc, result) => Emit(spc, result));
    }

    private static LogMethodGenerationResult? TryCreateGenerationResult(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (context.TargetSymbol is not IMethodSymbol methodSymbol)
        {
            return null;
        }

        if (methodSymbol.PartialImplementationPart is not null)
        {
            return null;
        }

        if (!LogMethodUtilities.TryGetLogMethodAttribute(methodSymbol, out var attribute) || attribute is null)
        {
            return null;
        }

        return GenerateMethod(context.SemanticModel.Compilation, methodSymbol, attribute);
    }

    private static void Emit(SourceProductionContext context, LogMethodGenerationResult result)
    {
        foreach (var diagnostic in result.Diagnostics)
        {
            context.ReportDiagnostic(diagnostic);
        }

        if (result.Source is not null && result.HintName is not null)
        {
            context.AddSource(result.HintName, result.Source);
        }
    }

    private static LogMethodGenerationResult GenerateMethod(Compilation compilation, IMethodSymbol methodSymbol, AttributeData attribute)
    {
        var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

        if (!TryValidateMethod(methodSymbol, diagnostics, out var loggerParameter, out var exceptionParameter, out var propertiesParameter))
        {
            return new LogMethodGenerationResult(null, null, diagnostics.ToImmutable());
        }

        if (!TryGetLogLevel(methodSymbol, attribute, diagnostics, out var logLevelValue, out var logMethodName))
        {
            return new LogMethodGenerationResult(null, null, diagnostics.ToImmutable());
        }

        var messageTemplate = attribute.ConstructorArguments[1].Value as string;
        if (string.IsNullOrEmpty(messageTemplate))
        {
            Report(
                diagnostics,
                LogMethodDiagnostics.InvalidMethodSignature,
                methodSymbol,
                methodSymbol.Name,
                "The message template cannot be null or empty.");
            return new LogMethodGenerationResult(null, null, diagnostics.ToImmutable());
        }

        var nonNullTemplate = messageTemplate!;
        if (!LogMethodUtilities.TryParseTemplate(nonNullTemplate, out var tokens, out var parseError))
        {
            Report(
                diagnostics,
                LogMethodDiagnostics.TemplateParseError,
                methodSymbol,
                methodSymbol.Name,
                parseError);
            return new LogMethodGenerationResult(null, null, diagnostics.ToImmutable());
        }

        var templateParameters = BuildTemplateParameterMap(methodSymbol, loggerParameter, propertiesParameter);
        if (!ValidateTemplateParameters(methodSymbol, tokens, templateParameters, diagnostics))
        {
            return new LogMethodGenerationResult(null, null, diagnostics.ToImmutable());
        }

        var source = GenerateSource(
            compilation,
            methodSymbol,
            tokens,
            templateParameters,
            loggerParameter,
            exceptionParameter,
            propertiesParameter,
            logLevelValue,
            logMethodName!,
            GetEventIdData(attribute, methodSymbol.Name));

        var hintName = CreateHintName(methodSymbol);
        return new LogMethodGenerationResult(hintName, source, diagnostics.ToImmutable());
    }

    private static bool TryValidateMethod(
        IMethodSymbol methodSymbol,
        ImmutableArray<Diagnostic>.Builder diagnostics,
        out IParameterSymbol loggerParameter,
        out IParameterSymbol? exceptionParameter,
        out IParameterSymbol? propertiesParameter)
    {
        loggerParameter = null!;
        exceptionParameter = null;
        propertiesParameter = null;

        if (!methodSymbol.IsStatic ||
            !methodSymbol.IsPartialDefinition ||
            !methodSymbol.ReturnsVoid ||
            methodSymbol.IsGenericMethod)
        {
            Report(
                diagnostics,
                LogMethodDiagnostics.InvalidMethodSignature,
                methodSymbol,
                methodSymbol.Name,
                "The method must be static partial void and cannot be generic.");
            return false;
        }

        if (methodSymbol.Parameters.Length == 0)
        {
            Report(
                diagnostics,
                LogMethodDiagnostics.InvalidMethodSignature,
                methodSymbol,
                methodSymbol.Name,
                "The first parameter must be a Logger.");
            return false;
        }

        var firstParameter = methodSymbol.Parameters[0];
        if (!LogMethodUtilities.IsLoggerParameter(firstParameter))
        {
            Report(
                diagnostics,
                LogMethodDiagnostics.InvalidMethodSignature,
                methodSymbol,
                methodSymbol.Name,
                "The first parameter must be a XenoAtom.Logging.Logger.");
            return false;
        }

        if (HasGenericContainingType(methodSymbol.ContainingType))
        {
            Report(
                diagnostics,
                LogMethodDiagnostics.InvalidMethodSignature,
                methodSymbol,
                methodSymbol.Name,
                "Containing types with generic parameters are not supported.");
            return false;
        }

        loggerParameter = firstParameter;

        foreach (var parameterSymbol in methodSymbol.Parameters)
        {
            if (parameterSymbol.RefKind != RefKind.None || parameterSymbol.IsParams || parameterSymbol.HasExplicitDefaultValue)
            {
                Report(
                    diagnostics,
                    LogMethodDiagnostics.InvalidMethodSignature,
                    methodSymbol,
                    methodSymbol.Name,
                    "ref/out/in/params/default parameters are not supported.");
                return false;
            }

            if (LogMethodUtilities.IsLogPropertiesParameter(parameterSymbol))
            {
                if (propertiesParameter is not null)
                {
                    Report(
                        diagnostics,
                        LogMethodDiagnostics.InvalidMethodSignature,
                        methodSymbol,
                        methodSymbol.Name,
                        "Only a single LogProperties parameter is supported.");
                    return false;
                }

                propertiesParameter = parameterSymbol;
                continue;
            }

            if (LogMethodUtilities.IsExceptionType(parameterSymbol.Type))
            {
                if (exceptionParameter is not null)
                {
                    Report(
                        diagnostics,
                        LogMethodDiagnostics.InvalidMethodSignature,
                        methodSymbol,
                        methodSymbol.Name,
                        "Only a single Exception parameter is supported.");
                    return false;
                }

                exceptionParameter = parameterSymbol;
            }
        }

        return true;
    }

    private static Dictionary<string, IParameterSymbol> BuildTemplateParameterMap(
        IMethodSymbol methodSymbol,
        IParameterSymbol loggerParameter,
        IParameterSymbol? propertiesParameter)
    {
        var result = new Dictionary<string, IParameterSymbol>(StringComparer.OrdinalIgnoreCase);
        foreach (var parameterSymbol in methodSymbol.Parameters)
        {
            if (SymbolEqualityComparer.Default.Equals(parameterSymbol, loggerParameter))
            {
                continue;
            }

            if (propertiesParameter is not null && SymbolEqualityComparer.Default.Equals(parameterSymbol, propertiesParameter))
            {
                continue;
            }

            result[parameterSymbol.Name] = parameterSymbol;
        }

        return result;
    }

    private static bool ValidateTemplateParameters(
        IMethodSymbol methodSymbol,
        IEnumerable<TemplateToken> tokens,
        IReadOnlyDictionary<string, IParameterSymbol> templateParameters,
        ImmutableArray<Diagnostic>.Builder diagnostics)
    {
        foreach (var token in tokens)
        {
            if (!token.IsPlaceholder)
            {
                continue;
            }

            if (!templateParameters.ContainsKey(token.Text))
            {
                Report(
                    diagnostics,
                    LogMethodDiagnostics.TemplateParameterNotFound,
                    methodSymbol,
                    methodSymbol.Name,
                    token.Text);
                return false;
            }
        }

        return true;
    }

    private static bool TryGetLogLevel(
        IMethodSymbol methodSymbol,
        AttributeData attribute,
        ImmutableArray<Diagnostic>.Builder diagnostics,
        out int logLevelValue,
        out string? logMethodName)
    {
        logLevelValue = (int)attribute.ConstructorArguments[0].Value!;
        logMethodName = LogMethodUtilities.GetLogMethodName(logLevelValue);
        if (logMethodName is null)
        {
            Report(
                diagnostics,
                LogMethodDiagnostics.InvalidLogLevel,
                methodSymbol,
                methodSymbol.Name,
                LogMethodUtilities.GetLogLevelName(logLevelValue));
            return false;
        }

        return true;
    }

    private static (bool HasEventId, int EventId, string EventName) GetEventIdData(AttributeData attribute, string methodName)
    {
        var eventId = 0;
        var eventName = methodName;
        var hasEventId = false;

        foreach (var namedArgument in attribute.NamedArguments)
        {
            if (namedArgument.Key == "EventId")
            {
                eventId = (int)(namedArgument.Value.Value ?? 0);
                hasEventId = hasEventId || eventId != 0;
            }
            else if (namedArgument.Key == "EventName")
            {
                if (namedArgument.Value.Value is string name && name.Length > 0)
                {
                    eventName = name;
                }

                hasEventId = true;
            }
        }

        return (hasEventId, eventId, eventName);
    }

    private static string GenerateSource(
        Compilation compilation,
        IMethodSymbol methodSymbol,
        ImmutableArray<TemplateToken> tokens,
        IReadOnlyDictionary<string, IParameterSymbol> templateParameters,
        IParameterSymbol loggerParameter,
        IParameterSymbol? exceptionParameter,
        IParameterSymbol? propertiesParameter,
        int logLevelValue,
        string logMethodName,
        (bool HasEventId, int EventId, string EventName) eventIdData)
    {
        var source = new StringBuilder();
        source.AppendLine("// <auto-generated/>");
        source.AppendLine("#nullable enable");

        var namespaceSymbol = methodSymbol.ContainingNamespace;
        if (!namespaceSymbol.IsGlobalNamespace)
        {
            source.Append("namespace ").Append(namespaceSymbol.ToDisplayString()).AppendLine();
            source.AppendLine("{");
        }

        var indent = namespaceSymbol.IsGlobalNamespace ? 0 : 4;

        var containingTypes = GetContainingTypes(methodSymbol.ContainingType);
        foreach (var type in containingTypes)
        {
            AppendIndent(source, indent);
            source.Append(GetAccessibilityKeyword(type.DeclaredAccessibility));
            if (type.IsStatic)
            {
                source.Append(" static");
            }

            source.Append(" partial ");
            source.Append(GetTypeKeyword(type));
            source.Append(' ');
            source.Append(LogMethodUtilities.EscapeIdentifier(type.Name));
            source.AppendLine();

            AppendIndent(source, indent);
            source.AppendLine("{");
            indent += 4;
        }

        AppendIndent(source, indent);
        var methodAccessibility = GetAccessibilityKeyword(methodSymbol.DeclaredAccessibility).Trim();
        if (methodAccessibility.Length > 0)
        {
            source.Append(methodAccessibility).Append(' ');
        }

        source.Append("static partial void ");
        source.Append(LogMethodUtilities.EscapeIdentifier(methodSymbol.Name));
        source.Append('(');
        source.Append(BuildParameterList(methodSymbol));
        source.AppendLine(")");

        AppendIndent(source, indent);
        source.AppendLine("{");
        indent += 4;

        var loggerIdentifier = LogMethodUtilities.EscapeIdentifier(loggerParameter.Name);
        AppendIndent(source, indent);
        source.Append("if (!");
        source.Append(loggerIdentifier);
        source.Append(".IsEnabled(global::XenoAtom.Logging.LogLevel.");
        source.Append(LogMethodUtilities.GetLogLevelName(logLevelValue));
        source.AppendLine("))");

        AppendIndent(source, indent);
        source.AppendLine("{");
        AppendIndent(source, indent + 4);
        source.AppendLine("return;");
        AppendIndent(source, indent);
        source.AppendLine("}");

        var arguments = new List<string>();
        if (eventIdData.HasEventId)
        {
            arguments.Add(
                $"new global::XenoAtom.Logging.LogEventId({eventIdData.EventId.ToString(CultureInfo.InvariantCulture)}, {LogMethodUtilities.ToStringLiteral(eventIdData.EventName)})");
        }

        if (exceptionParameter is not null)
        {
            arguments.Add(LogMethodUtilities.EscapeIdentifier(exceptionParameter.Name));
        }

        if (propertiesParameter is not null)
        {
            arguments.Add(LogMethodUtilities.EscapeIdentifier(propertiesParameter.Name));
        }

        arguments.Add(BuildInterpolatedMessage(compilation, tokens, templateParameters));

        AppendIndent(source, indent);
        source.Append("global::XenoAtom.Logging.LoggerExtensions.");
        source.Append(logMethodName);
        source.Append('(');
        source.Append(loggerIdentifier);
        if (arguments.Count > 0)
        {
            source.Append(", ");
            source.Append(string.Join(", ", arguments));
        }
        source.AppendLine(");");

        indent -= 4;
        AppendIndent(source, indent);
        source.AppendLine("}");

        foreach (var _ in containingTypes)
        {
            indent -= 4;
            AppendIndent(source, indent);
            source.AppendLine("}");
        }

        if (!namespaceSymbol.IsGlobalNamespace)
        {
            source.AppendLine("}");
        }

        return source.ToString();
    }

    private static string BuildInterpolatedMessage(
        Compilation compilation,
        IEnumerable<TemplateToken> tokens,
        IReadOnlyDictionary<string, IParameterSymbol> templateParameters)
    {
        var message = new StringBuilder();
        message.Append('$');
        message.Append('"');

        foreach (var token in tokens)
        {
            if (!token.IsPlaceholder)
            {
                message.Append(LogMethodUtilities.EscapeInterpolatedLiteral(token.Text));
                continue;
            }

            var parameter = templateParameters[token.Text];
            var parameterIdentifier = LogMethodUtilities.EscapeIdentifier(parameter.Name);
            var expression = BuildInterpolationExpression(compilation, parameter, parameterIdentifier);
            message.Append('{');
            message.Append(expression);
            if (token.Alignment.HasValue)
            {
                message.Append(',');
                message.Append(token.Alignment.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (!string.IsNullOrEmpty(token.Format))
            {
                message.Append(':');
                message.Append(token.Format);
            }

            message.Append('}');
        }

        message.Append('"');
        return message.ToString();
    }

    private static string BuildInterpolationExpression(Compilation compilation, IParameterSymbol parameter, string identifier)
    {
        if (LogMethodUtilities.IsAllocationFriendlyType(parameter.Type, compilation))
        {
            return identifier;
        }

        if (parameter.Type.IsReferenceType || parameter.NullableAnnotation == NullableAnnotation.Annotated)
        {
            return $"({identifier} is null ? null : {identifier}.ToString())";
        }

        return $"global::System.Convert.ToString({identifier}, global::System.Globalization.CultureInfo.InvariantCulture)";
    }

    private static string BuildParameterList(IMethodSymbol methodSymbol)
    {
        var builder = new StringBuilder();
        for (var index = 0; index < methodSymbol.Parameters.Length; index++)
        {
            var parameterSymbol = methodSymbol.Parameters[index];
            if (index > 0)
            {
                builder.Append(", ");
            }

            builder.Append(LogMethodUtilities.FormatType(parameterSymbol.Type));
            builder.Append(' ');
            builder.Append(LogMethodUtilities.EscapeIdentifier(parameterSymbol.Name));
        }

        return builder.ToString();
    }

    private static string GetAccessibilityKeyword(Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            Accessibility.Private => "private",
            Accessibility.Protected => "protected",
            Accessibility.ProtectedAndInternal => "private protected",
            Accessibility.ProtectedOrInternal => "protected internal",
            _ => string.Empty,
        };
    }

    private static string GetTypeKeyword(INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol.IsRecord)
        {
            return typeSymbol.TypeKind == TypeKind.Struct ? "record struct" : "record class";
        }

        return typeSymbol.TypeKind == TypeKind.Struct ? "struct" : "class";
    }

    private static List<INamedTypeSymbol> GetContainingTypes(INamedTypeSymbol containingType)
    {
        var result = new List<INamedTypeSymbol>();
        var current = containingType;
        while (current is not null)
        {
            result.Add(current);
            current = current.ContainingType;
        }

        result.Reverse();
        return result;
    }

    private static bool HasGenericContainingType(INamedTypeSymbol? containingType)
    {
        var current = containingType;
        while (current is not null)
        {
            if (current.TypeParameters.Length > 0)
            {
                return true;
            }

            current = current.ContainingType;
        }

        return false;
    }

    private static string CreateHintName(IMethodSymbol methodSymbol)
    {
        var fullyQualifiedName = methodSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var sanitized = fullyQualifiedName
            .Replace("global::", string.Empty)
            .Replace('<', '_')
            .Replace('>', '_')
            .Replace(':', '_')
            .Replace('.', '_')
            .Replace('(', '_')
            .Replace(')', '_')
            .Replace(',', '_')
            .Replace(' ', '_');
        return $"{sanitized}.LogMethod.g.cs";
    }

    private static void Report(
        ImmutableArray<Diagnostic>.Builder diagnostics,
        DiagnosticDescriptor descriptor,
        IMethodSymbol methodSymbol,
        params object[] arguments)
    {
        var location = methodSymbol.Locations.Length > 0 ? methodSymbol.Locations[0] : Location.None;
        diagnostics.Add(Diagnostic.Create(descriptor, location, arguments));
    }

    private static void AppendIndent(StringBuilder builder, int indent)
    {
        if (indent > 0)
        {
            builder.Append(' ', indent);
        }
    }

    private readonly struct LogMethodGenerationResult
    {
        public LogMethodGenerationResult(string? hintName, string? source, ImmutableArray<Diagnostic> diagnostics)
        {
            HintName = hintName;
            Source = source;
            Diagnostics = diagnostics;
        }

        public string? HintName { get; }

        public string? Source { get; }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
    }

    private sealed class LogMethodGenerationResultComparer : IEqualityComparer<LogMethodGenerationResult>
    {
        public static readonly LogMethodGenerationResultComparer Instance = new();

        public bool Equals(LogMethodGenerationResult x, LogMethodGenerationResult y)
        {
            if (!string.Equals(x.HintName, y.HintName, StringComparison.Ordinal) ||
                !string.Equals(x.Source, y.Source, StringComparison.Ordinal))
            {
                return false;
            }

            var xDiagnostics = x.Diagnostics;
            var yDiagnostics = y.Diagnostics;
            if (xDiagnostics.Length != yDiagnostics.Length)
            {
                return false;
            }

            for (var i = 0; i < xDiagnostics.Length; i++)
            {
                var left = xDiagnostics[i];
                var right = yDiagnostics[i];
                if (!string.Equals(left.Id, right.Id, StringComparison.Ordinal) ||
                    left.Severity != right.Severity ||
                    !string.Equals(left.GetMessage(CultureInfo.InvariantCulture), right.GetMessage(CultureInfo.InvariantCulture), StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(LogMethodGenerationResult obj)
        {
            unchecked
            {
                var hash = 17;
                hash = (hash * 31) + (obj.HintName is null ? 0 : StringComparer.Ordinal.GetHashCode(obj.HintName));
                hash = (hash * 31) + (obj.Source is null ? 0 : StringComparer.Ordinal.GetHashCode(obj.Source));

                foreach (var diagnostic in obj.Diagnostics)
                {
                    hash = (hash * 31) + StringComparer.Ordinal.GetHashCode(diagnostic.Id);
                    hash = (hash * 31) + (int)diagnostic.Severity;
                    hash = (hash * 31) + StringComparer.Ordinal.GetHashCode(diagnostic.GetMessage(CultureInfo.InvariantCulture));
                }

                return hash;
            }
        }
    }
}
