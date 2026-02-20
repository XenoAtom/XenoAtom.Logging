// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XenoAtom.Logging.Generators;

/// <summary>
/// Generates formatter implementations from <c>[LogFormatter]</c> templates.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class LogFormatterGenerator : IIncrementalGenerator
{
    private const string DefaultTimestampFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
    private const string DefaultSeparator = ", ";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var typeResults = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                LogFormatterUtilities.LogFormatterAttributeMetadataName,
                static (node, _) => node is TypeDeclarationSyntax { AttributeLists.Count: > 0 },
                static (ctx, ct) => TryCreateTypeGenerationResult(ctx, ct))
            .Where(static result => result is not null)
            .Select(static (result, _) => result!.Value)
            .WithComparer(LogFormatterGenerationResultComparer.Instance);

        context.RegisterSourceOutput(
            typeResults,
            static (spc, result) => Emit(spc, result));

        var propertyResults = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                LogFormatterUtilities.LogFormatterAttributeMetadataName,
                static (node, _) => node is PropertyDeclarationSyntax { AttributeLists.Count: > 0 },
                static (ctx, ct) => TryCreatePropertyGenerationResult(ctx, ct))
            .Where(static result => result is not null)
            .Select(static (result, _) => result!.Value)
            .WithComparer(LogFormatterGenerationResultComparer.Instance);

        context.RegisterSourceOutput(
            propertyResults,
            static (spc, result) => Emit(spc, result));
    }

    private static LogFormatterGenerationResult? TryCreateTypeGenerationResult(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (context.TargetSymbol is not INamedTypeSymbol typeSymbol)
        {
            return null;
        }

        if (!LogFormatterUtilities.TryGetLogFormatterAttribute(typeSymbol, out var attribute) || attribute is null)
        {
            return null;
        }

        return GenerateForType(typeSymbol, attribute);
    }

    private static LogFormatterGenerationResult? TryCreatePropertyGenerationResult(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (context.TargetSymbol is not IPropertySymbol propertySymbol)
        {
            return null;
        }

        if (!LogFormatterUtilities.TryGetLogFormatterAttribute(propertySymbol, out var attribute) || attribute is null)
        {
            return null;
        }

        return GenerateForProperty(propertySymbol, attribute);
    }

    private static void Emit(SourceProductionContext context, LogFormatterGenerationResult result)
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

    private static LogFormatterGenerationResult GenerateForType(INamedTypeSymbol typeSymbol, AttributeData attribute)
    {
        var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
        if (!TryValidateFormatterTypeUsage(diagnostics, typeSymbol))
        {
            return new LogFormatterGenerationResult(null, null, diagnostics.ToImmutable());
        }

        if (!TryGetTemplate(attribute, out var template))
        {
            Report(diagnostics, LogFormatterDiagnostics.MalformedTemplate, typeSymbol, "Template argument is missing.");
            return new LogFormatterGenerationResult(null, null, diagnostics.ToImmutable());
        }

        if (!TryBuildTemplateModel(diagnostics, typeSymbol, template, out var model))
        {
            return new LogFormatterGenerationResult(null, null, diagnostics.ToImmutable());
        }

        var source = GenerateForType(typeSymbol, model);
        return new LogFormatterGenerationResult($"{CreateHintName(typeSymbol)}.LogFormatter.g.cs", source, diagnostics.ToImmutable());
    }

    private static LogFormatterGenerationResult GenerateForProperty(IPropertySymbol propertySymbol, AttributeData attribute)
    {
        var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
        if (!TryValidateFormatterPropertyUsage(diagnostics, propertySymbol))
        {
            return new LogFormatterGenerationResult(null, null, diagnostics.ToImmutable());
        }

        if (!TryGetTemplate(attribute, out var template))
        {
            Report(diagnostics, LogFormatterDiagnostics.MalformedTemplate, propertySymbol, "Template argument is missing.");
            return new LogFormatterGenerationResult(null, null, diagnostics.ToImmutable());
        }

        if (!TryBuildTemplateModel(diagnostics, propertySymbol, template, out var model))
        {
            return new LogFormatterGenerationResult(null, null, diagnostics.ToImmutable());
        }

        var source = GenerateForProperty(propertySymbol, model);
        return new LogFormatterGenerationResult($"{CreateHintName(propertySymbol)}.LogFormatter.g.cs", source, diagnostics.ToImmutable());
    }

    private static bool TryValidateFormatterTypeUsage(ImmutableArray<Diagnostic>.Builder diagnostics, INamedTypeSymbol typeSymbol)
    {
        if (!IsPartial(typeSymbol))
        {
            Report(diagnostics, LogFormatterDiagnostics.InvalidTypeUsage, typeSymbol, $"Type '{typeSymbol.Name}' must be partial.");
            return false;
        }

        if (!InheritsLogFormatter(typeSymbol))
        {
            Report(diagnostics, LogFormatterDiagnostics.InvalidTypeUsage, typeSymbol, $"Type '{typeSymbol.Name}' must inherit XenoAtom.Logging.LogFormatter.");
            return false;
        }

        return true;
    }

    private static bool TryValidateFormatterPropertyUsage(ImmutableArray<Diagnostic>.Builder diagnostics, IPropertySymbol propertySymbol)
    {
        if (!propertySymbol.IsStatic || !IsPartialProperty(propertySymbol))
        {
            Report(diagnostics, LogFormatterDiagnostics.InvalidPropertyUsage, propertySymbol, $"Property '{propertySymbol.Name}' must be static partial.");
            return false;
        }

        if (propertySymbol.ContainingType is null || !IsPartial(propertySymbol.ContainingType))
        {
            Report(diagnostics, LogFormatterDiagnostics.InvalidPropertyUsage, propertySymbol, $"Containing type for property '{propertySymbol.Name}' must be partial.");
            return false;
        }

        if (!InheritsLogFormatter(propertySymbol.Type))
        {
            Report(diagnostics, LogFormatterDiagnostics.InvalidPropertyUsage, propertySymbol, $"Property '{propertySymbol.Name}' must return XenoAtom.Logging.LogFormatter or a subtype.");
            return false;
        }

        return true;
    }

    private static bool TryGetTemplate(AttributeData attribute, out string template)
    {
        if (attribute.ConstructorArguments.Length == 0 || attribute.ConstructorArguments[0].Value is not string text)
        {
            template = string.Empty;
            return false;
        }

        template = text;
        return true;
    }

    private static bool TryBuildTemplateModel(
        ImmutableArray<Diagnostic>.Builder diagnostics,
        ISymbol targetSymbol,
        string template,
        out FormatterTemplateModel model)
    {
        model = default!;
        if (!LogFormatterUtilities.TryParseTemplate(template, out var nodes, out var parseError))
        {
            Report(diagnostics, LogFormatterDiagnostics.MalformedTemplate, targetSymbol, parseError);
            return false;
        }

        if (!TryNormalizeNodes(diagnostics, targetSymbol, nodes, out var normalizedItems, out var usesTimestamp, out var usesLevel, out var defaultTimestampFormat, out var defaultLevelFormat))
        {
            return false;
        }

        model = new FormatterTemplateModel(normalizedItems, usesTimestamp, usesLevel, defaultTimestampFormat, defaultLevelFormat);
        return true;
    }

    private static bool TryNormalizeNodes(
        ImmutableArray<Diagnostic>.Builder diagnostics,
        ISymbol targetSymbol,
        ImmutableArray<FormatterTemplateNode> nodes,
        out ImmutableArray<TemplateItem> items,
        out bool usesTimestamp,
        out bool usesLevel,
        out string defaultTimestampFormat,
        out FormatterLevelFormat defaultLevelFormat)
    {
        var builder = ImmutableArray.CreateBuilder<TemplateItem>();
        usesTimestamp = false;
        usesLevel = false;
        defaultTimestampFormat = DefaultTimestampFormat;
        defaultLevelFormat = FormatterLevelFormat.Tri;

        foreach (var node in nodes)
        {
            if (!TryNormalizeNode(diagnostics, targetSymbol, node, builder, ref usesTimestamp, ref usesLevel, ref defaultTimestampFormat, ref defaultLevelFormat))
            {
                items = default;
                return false;
            }
        }

        items = builder.ToImmutable();
        return true;
    }

    private static bool TryNormalizeNode(
        ImmutableArray<Diagnostic>.Builder diagnostics,
        ISymbol targetSymbol,
        FormatterTemplateNode node,
        ImmutableArray<TemplateItem>.Builder builder,
        ref bool usesTimestamp,
        ref bool usesLevel,
        ref string defaultTimestampFormat,
        ref FormatterLevelFormat defaultLevelFormat)
    {
        switch (node.Kind)
        {
            case FormatterTemplateNodeKind.Literal:
                if (!string.IsNullOrEmpty(node.Literal))
                {
                    builder.Add(new LiteralTemplateItem(node.Literal));
                }
                return true;
            case FormatterTemplateNodeKind.Field:
                if (!TryNormalizeField(diagnostics, targetSymbol, node.Field, out var fieldItem, ref usesTimestamp, ref usesLevel, ref defaultTimestampFormat, ref defaultLevelFormat))
                {
                    return false;
                }

                builder.Add(fieldItem);
                return true;
            case FormatterTemplateNodeKind.Conditional:
            {
                var conditionalBuilder = ImmutableArray.CreateBuilder<TemplateItem>();
                foreach (var child in node.Children)
                {
                    if (child.Kind == FormatterTemplateNodeKind.Conditional)
                    {
                        Report(diagnostics, LogFormatterDiagnostics.MalformedTemplate, targetSymbol, "Malformed template: nested conditional sections are not supported.");
                        return false;
                    }

                    if (!TryNormalizeNode(diagnostics, targetSymbol, child, conditionalBuilder, ref usesTimestamp, ref usesLevel, ref defaultTimestampFormat, ref defaultLevelFormat))
                    {
                        return false;
                    }
                }

                var conditionalItems = conditionalBuilder.ToImmutable();
                var fields = GetFields(conditionalItems);
                if (fields.Length == 0)
                {
                    Report(diagnostics, LogFormatterDiagnostics.MalformedTemplate, targetSymbol, "Malformed template: conditional section must contain at least one field.");
                    return false;
                }

                var hasEmptiableField = fields.Any(IsEmptiableField);
                if (!hasEmptiableField)
                {
                    Report(diagnostics, LogFormatterDiagnostics.ConditionalAlwaysEmitted, targetSymbol);
                }

                builder.Add(new ConditionalTemplateItem(conditionalItems, fields, hasEmptiableField));
                return true;
            }
            default:
                Report(diagnostics, LogFormatterDiagnostics.MalformedTemplate, targetSymbol, "Malformed template node.");
                return false;
        }
    }

    private static bool TryNormalizeField(
        ImmutableArray<Diagnostic>.Builder diagnostics,
        ISymbol targetSymbol,
        FormatterFieldToken fieldToken,
        out FieldTemplateItem item,
        ref bool usesTimestamp,
        ref bool usesLevel,
        ref string defaultTimestampFormat,
        ref FormatterLevelFormat defaultLevelFormat)
    {
        item = default!;
        if (!TryResolveFieldKind(fieldToken.Name, out var fieldKind))
        {
            Report(diagnostics, LogFormatterDiagnostics.UnknownField, targetSymbol, fieldToken.Name);
            return false;
        }

        var format = fieldToken.Format;
        var mode = 0;
        var separator = DefaultSeparator;

        switch (fieldKind)
        {
            case FormatterFieldKind.Timestamp:
                usesTimestamp = true;
                var timestampFormat = string.IsNullOrEmpty(format) ? DefaultTimestampFormat : format!;
                if (!string.Equals(defaultTimestampFormat, timestampFormat, StringComparison.Ordinal))
                {
                    if (!string.Equals(defaultTimestampFormat, DefaultTimestampFormat, StringComparison.Ordinal))
                    {
                        Report(diagnostics, LogFormatterDiagnostics.InvalidFieldFormat, targetSymbol, timestampFormat, fieldToken.Name);
                        return false;
                    }
                }

                defaultTimestampFormat = timestampFormat;
                format = timestampFormat;
                break;

            case FormatterFieldKind.Level:
                usesLevel = true;
                if (!TryParseLevelFormat(format, out var parsedLevelFormat))
                {
                    Report(diagnostics, LogFormatterDiagnostics.InvalidFieldFormat, targetSymbol, format ?? string.Empty, fieldToken.Name);
                    return false;
                }

                if (defaultLevelFormat != parsedLevelFormat && defaultLevelFormat != FormatterLevelFormat.Tri)
                {
                    Report(diagnostics, LogFormatterDiagnostics.InvalidFieldFormat, targetSymbol, format ?? string.Empty, fieldToken.Name);
                    return false;
                }

                defaultLevelFormat = parsedLevelFormat;
                format = null;
                break;

            case FormatterFieldKind.EventId:
            case FormatterFieldKind.Exception:
            case FormatterFieldKind.Thread:
                if (!TryParseNamedMode(format, fieldToken.Name, out mode))
                {
                    Report(diagnostics, LogFormatterDiagnostics.InvalidFieldFormat, targetSymbol, format ?? string.Empty, fieldToken.Name);
                    return false;
                }
                format = null;
                break;

            case FormatterFieldKind.Scope:
            case FormatterFieldKind.Properties:
                if (!TryParseSeparator(format, out separator))
                {
                    Report(diagnostics, LogFormatterDiagnostics.InvalidFieldFormat, targetSymbol, format ?? string.Empty, fieldToken.Name);
                    return false;
                }
                format = null;
                break;

            case FormatterFieldKind.LoggerName:
            case FormatterFieldKind.Text:
            case FormatterFieldKind.NewLine:
                if (!string.IsNullOrEmpty(format))
                {
                    Report(diagnostics, LogFormatterDiagnostics.InvalidFieldFormat, targetSymbol, format ?? string.Empty, fieldToken.Name);
                    return false;
                }
                format = null;
                break;
        }

        item = new FieldTemplateItem(fieldKind, fieldToken.Alignment, format, mode, separator);
        return true;
    }

    private static ImmutableArray<FormatterFieldKind> GetFields(ImmutableArray<TemplateItem> items)
    {
        var builder = ImmutableArray.CreateBuilder<FormatterFieldKind>();
        foreach (var item in items)
        {
            if (item is FieldTemplateItem field)
            {
                builder.Add(field.Kind);
            }
        }

        return builder.ToImmutable();
    }

    private static bool IsEmptiableField(FormatterFieldKind fieldKind)
        => fieldKind is FormatterFieldKind.EventId
            or FormatterFieldKind.Exception
            or FormatterFieldKind.Scope
            or FormatterFieldKind.Properties
            or FormatterFieldKind.Text;

    private static bool TryResolveFieldKind(string fieldName, out FormatterFieldKind fieldKind)
    {
        switch (fieldName.ToLowerInvariant())
        {
            case "timestamp": fieldKind = FormatterFieldKind.Timestamp; return true;
            case "level": fieldKind = FormatterFieldKind.Level; return true;
            case "loggername": fieldKind = FormatterFieldKind.LoggerName; return true;
            case "eventid": fieldKind = FormatterFieldKind.EventId; return true;
            case "text": fieldKind = FormatterFieldKind.Text; return true;
            case "exception": fieldKind = FormatterFieldKind.Exception; return true;
            case "thread": fieldKind = FormatterFieldKind.Thread; return true;
            case "sequenceid": fieldKind = FormatterFieldKind.SequenceId; return true;
            case "scope": fieldKind = FormatterFieldKind.Scope; return true;
            case "properties": fieldKind = FormatterFieldKind.Properties; return true;
            case "newline": fieldKind = FormatterFieldKind.NewLine; return true;
            default:
                fieldKind = default;
                return false;
        }
    }

    private static bool TryParseLevelFormat(string? format, out FormatterLevelFormat levelFormat)
    {
        if (format is not { Length: > 0 } formatValue)
        {
            levelFormat = FormatterLevelFormat.Tri;
            return true;
        }

        switch (formatValue.ToLowerInvariant())
        {
            case "short": levelFormat = FormatterLevelFormat.Short; return true;
            case "long": levelFormat = FormatterLevelFormat.Long; return true;
            case "tri": levelFormat = FormatterLevelFormat.Tri; return true;
            case "char": levelFormat = FormatterLevelFormat.Char; return true;
            default:
                levelFormat = default;
                return false;
        }
    }

    private static bool TryParseNamedMode(string? format, string fieldName, out int mode)
    {
        mode = 0;
        if (format is not { Length: > 0 } formatValue)
        {
            return true;
        }

        switch (fieldName.ToLowerInvariant())
        {
            case "eventid":
                if (formatValue.Equals("id", StringComparison.OrdinalIgnoreCase)) { mode = 1; return true; }
                if (formatValue.Equals("name", StringComparison.OrdinalIgnoreCase)) { mode = 2; return true; }
                return false;
            case "exception":
                if (formatValue.Equals("message", StringComparison.OrdinalIgnoreCase)) { mode = 1; return true; }
                if (formatValue.Equals("type", StringComparison.OrdinalIgnoreCase)) { mode = 2; return true; }
                return false;
            case "thread":
                if (formatValue.Equals("id", StringComparison.OrdinalIgnoreCase)) { mode = 0; return true; }
                if (formatValue.Equals("name", StringComparison.OrdinalIgnoreCase)) { mode = 1; return true; }
                return false;
            default:
                return false;
        }
    }

    private static bool TryParseSeparator(string? format, out string separator)
    {
        separator = DefaultSeparator;
        if (format is not { Length: > 0 } formatValue)
        {
            return true;
        }

        const string prefix = "separator=";
        if (!formatValue.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        separator = formatValue.Substring(prefix.Length);
        return true;
    }

    private static string GenerateForType(INamedTypeSymbol typeSymbol, FormatterTemplateModel model)
    {
        var source = new StringBuilder();
        AppendFilePreamble(source);

        var namespaceSymbol = typeSymbol.ContainingNamespace;
        if (!namespaceSymbol.IsGlobalNamespace)
        {
            source.Append("namespace ").Append(namespaceSymbol.ToDisplayString()).AppendLine();
            source.AppendLine("{");
        }

        var indent = namespaceSymbol.IsGlobalNamespace ? 0 : 4;
        var containingTypes = GetContainingTypes(typeSymbol.ContainingType);
        foreach (var containingType in containingTypes)
        {
            AppendTypeHeader(source, containingType, indent);
            AppendIndent(source, indent);
            source.AppendLine("{");
            indent += 4;
        }

        AppendIndent(source, indent);
        source.Append("partial ").Append(GetTypeKeyword(typeSymbol)).Append(' ').Append(LogFormatterUtilities.EscapeIdentifier(typeSymbol.Name)).AppendLine();
        AppendIndent(source, indent);
        source.AppendLine("{");
        indent += 4;

        EmitFormatterBody(source, indent, typeSymbol.Name, model);

        indent -= 4;
        AppendIndent(source, indent);
        source.AppendLine("}");

        for (var i = containingTypes.Count - 1; i >= 0; i--)
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

    private static string GenerateForProperty(IPropertySymbol propertySymbol, FormatterTemplateModel model)
    {
        var source = new StringBuilder();
        AppendFilePreamble(source);

        var containingType = propertySymbol.ContainingType;
        var namespaceSymbol = containingType.ContainingNamespace;
        if (!namespaceSymbol.IsGlobalNamespace)
        {
            source.Append("namespace ").Append(namespaceSymbol.ToDisplayString()).AppendLine();
            source.AppendLine("{");
        }

        var indent = namespaceSymbol.IsGlobalNamespace ? 0 : 4;
        var outerTypes = GetContainingTypes(containingType);
        foreach (var type in outerTypes)
        {
            AppendTypeHeader(source, type, indent);
            AppendIndent(source, indent);
            source.AppendLine("{");
            indent += 4;
        }

        var generatedTypeName = $"__{propertySymbol.Name}GeneratedFormatter";
        AppendIndent(source, indent);
        source.Append("private sealed partial record ").Append(generatedTypeName).AppendLine(" : global::XenoAtom.Logging.LogFormatter");
        AppendIndent(source, indent);
        source.AppendLine("{");
        indent += 4;

        EmitFormatterBody(source, indent, generatedTypeName, model);

        indent -= 4;
        AppendIndent(source, indent);
        source.AppendLine("}");
        source.AppendLine();

        AppendIndent(source, indent);
        var accessibility = GetAccessibilityKeyword(propertySymbol.DeclaredAccessibility);
        if (!string.IsNullOrEmpty(accessibility))
        {
            source.Append(accessibility).Append(' ');
        }

        source.Append("static partial ").Append(LogFormatterUtilities.FormatType(propertySymbol.Type)).Append(' ').Append(LogFormatterUtilities.EscapeIdentifier(propertySymbol.Name)).Append(" => ").Append(generatedTypeName).AppendLine(".Instance;");

        for (var index = outerTypes.Count - 1; index >= 0; index--)
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

    private static void EmitFormatterBody(StringBuilder source, int indent, string formatterTypeName, FormatterTemplateModel model)
    {
        AppendIndent(source, indent);
        source.AppendLine("/// <summary>");
        AppendIndent(source, indent);
        source.AppendLine("/// Gets a shared formatter instance.");
        AppendIndent(source, indent);
        source.AppendLine("/// </summary>");
        AppendIndent(source, indent);
        source.Append("public static ").Append(formatterTypeName).AppendLine(" Instance { get; } = new();");
        source.AppendLine();

        AppendIndent(source, indent);
        source.AppendLine("/// <summary>");
        AppendIndent(source, indent);
        source.AppendLine("/// Initializes a new formatter instance with template defaults.");
        AppendIndent(source, indent);
        source.AppendLine("/// </summary>");
        AppendIndent(source, indent);
        source.Append("public ").Append(formatterTypeName)
            .Append("() : base(global::XenoAtom.Logging.LogLevelFormat.")
            .Append(model.DefaultLevelFormat)
            .Append(", ")
            .Append(LogFormatterUtilities.ToStringLiteral(model.DefaultTimestampFormat))
            .AppendLine(")");
        AppendIndent(source, indent);
        source.AppendLine("{");
        AppendIndent(source, indent);
        source.AppendLine("}");
        source.AppendLine();

        AppendIndent(source, indent);
        source.AppendLine("/// <inheritdoc />");
        AppendIndent(source, indent);
        source.AppendLine("public override bool TryFormat(global::XenoAtom.Logging.LogMessage logMessage, global::System.Span<char> destination, out int charsWritten, ref global::XenoAtom.Logging.LogMessageFormatSegments segments)");
        AppendIndent(source, indent);
        source.AppendLine("{");
        indent += 4;

        AppendIndent(source, indent);
        source.AppendLine("charsWritten = 0;");
        AppendIndent(source, indent);
        source.AppendLine("var position = 0;");
        if (model.UsesTimestamp)
        {
            AppendIndent(source, indent);
            source.AppendLine("var timestampFormat = TimestampFormat;");
        }

        if (model.UsesLevel)
        {
            AppendIndent(source, indent);
            source.AppendLine("var levelFormat = LevelFormat;");
        }

        var operationIndex = 0;
        foreach (var item in model.Items)
        {
            EmitTemplateItem(source, indent, item, ref operationIndex);
        }

        AppendIndent(source, indent);
        source.AppendLine("charsWritten = position;");
        AppendIndent(source, indent);
        source.AppendLine("return true;");

        indent -= 4;
        AppendIndent(source, indent);
        source.AppendLine("}");

        EmitHelpers(source, indent);
    }

    private static void AppendFilePreamble(StringBuilder source)
    {
        source.AppendLine("// <auto-generated/>");
        source.AppendLine("#nullable enable");
        source.AppendLine("using System;");
        source.AppendLine("using XenoAtom.Logging;");
        source.AppendLine();
    }

    private static void EmitTemplateItem(StringBuilder source, int indent, TemplateItem item, ref int operationIndex)
    {
        if (item is LiteralTemplateItem literalItem)
        {
            if (literalItem.Text.Length == 0)
            {
                return;
            }

            AppendIndent(source, indent);
            source.Append("if (!TryAppend(").Append(LogFormatterUtilities.ToStringLiteral(literalItem.Text)).AppendLine(".AsSpan(), destination, ref position)) return false;");
            return;
        }

        if (item is FieldTemplateItem fieldItem)
        {
            EmitField(source, indent, fieldItem, ref operationIndex);
            return;
        }

        if (item is ConditionalTemplateItem conditionalItem)
        {
            if (conditionalItem.HasEmptiableField)
            {
                AppendIndent(source, indent);
                source.Append("if (");
                EmitConditionalExpression(source, conditionalItem.Fields);
                source.AppendLine(")");
                AppendIndent(source, indent);
                source.AppendLine("{");
                indent += 4;
            }

            foreach (var child in conditionalItem.Items)
            {
                EmitTemplateItem(source, indent, child, ref operationIndex);
            }

            if (conditionalItem.HasEmptiableField)
            {
                indent -= 4;
                AppendIndent(source, indent);
                source.AppendLine("}");
            }
        }
    }

    private static void EmitConditionalExpression(StringBuilder source, ImmutableArray<FormatterFieldKind> fields)
    {
        for (var i = 0; i < fields.Length; i++)
        {
            if (i > 0)
            {
                source.Append(" && ");
            }

            source.Append(fields[i] switch
            {
                FormatterFieldKind.EventId => "!logMessage.EventId.IsEmpty",
                FormatterFieldKind.Exception => "logMessage.Exception is not null",
                FormatterFieldKind.Scope => "logMessage.Scope.Count > 0",
                FormatterFieldKind.Properties => "logMessage.Properties.Count > 0",
                FormatterFieldKind.Text => "!logMessage.Text.IsEmpty",
                _ => "true"
            });
        }
    }

    private static void EmitField(StringBuilder source, int indent, FieldTemplateItem fieldItem, ref int operationIndex)
    {
        var fieldStartVar = $"fieldStart{operationIndex}";
        operationIndex++;

        var hasSegment = fieldItem.Kind != FormatterFieldKind.NewLine;
        if (hasSegment)
        {
            AppendIndent(source, indent);
            source.Append("var ").Append(fieldStartVar).AppendLine(" = position;");
        }

        switch (fieldItem.Kind)
        {
            case FormatterFieldKind.Timestamp:
                AppendIndent(source, indent);
                source.AppendLine("if (!TryWriteTimestamp(logMessage.Timestamp, timestampFormat, logMessage.FormatProvider, destination, ref position)) return false;");
                break;
            case FormatterFieldKind.Level:
                AppendIndent(source, indent);
                source.AppendLine("if (!TryWriteLevel(logMessage.Level, levelFormat, destination, ref position)) return false;");
                break;
            case FormatterFieldKind.LoggerName:
                AppendIndent(source, indent);
                source.AppendLine("if (!TryAppend(logMessage.Logger.Name.AsSpan(), destination, ref position)) return false;");
                break;
            case FormatterFieldKind.EventId:
                AppendIndent(source, indent);
                source.Append("if (!TryWriteEventId(logMessage.EventId, ").Append(fieldItem.Mode.ToString()).AppendLine(", destination, ref position)) return false;");
                break;
            case FormatterFieldKind.Text:
                AppendIndent(source, indent);
                source.AppendLine("if (!TryAppend(logMessage.Text, destination, ref position)) return false;");
                break;
            case FormatterFieldKind.Exception:
                AppendIndent(source, indent);
                source.Append("if (!TryWriteException(logMessage.Exception, ").Append(fieldItem.Mode.ToString()).AppendLine(", destination, ref position)) return false;");
                break;
            case FormatterFieldKind.Thread:
                AppendIndent(source, indent);
                source.Append("if (!TryWriteThread(logMessage.Thread, ").Append(fieldItem.Mode.ToString()).AppendLine(", destination, ref position)) return false;");
                break;
            case FormatterFieldKind.SequenceId:
                AppendIndent(source, indent);
                source.Append("if (!TryWriteSequenceId(logMessage.SequenceId, ").Append(fieldItem.Format is null ? "null" : LogFormatterUtilities.ToStringLiteral(fieldItem.Format)).AppendLine(", logMessage.FormatProvider, destination, ref position)) return false;");
                break;
            case FormatterFieldKind.Scope:
                AppendIndent(source, indent);
                source.Append("if (!TryWriteScope(logMessage.Scope, ").Append(LogFormatterUtilities.ToStringLiteral(fieldItem.Separator)).AppendLine(", destination, ref position)) return false;");
                break;
            case FormatterFieldKind.Properties:
                AppendIndent(source, indent);
                source.Append("if (!TryWriteProperties(logMessage.Properties, ").Append(LogFormatterUtilities.ToStringLiteral(fieldItem.Separator)).AppendLine(", destination, ref position)) return false;");
                break;
            case FormatterFieldKind.NewLine:
                AppendIndent(source, indent);
                source.AppendLine("if (!TryAppend(global::System.Environment.NewLine.AsSpan(), destination, ref position)) return false;");
                break;
        }

        if (fieldItem.Alignment.HasValue)
        {
            AppendIndent(source, indent);
            source.Append("if (!TryApplyAlignment(").Append(fieldItem.Alignment.Value.ToString()).Append(", ").Append(hasSegment ? fieldStartVar : "position").AppendLine(", destination, ref position)) return false;");
        }

        if (hasSegment)
        {
            AppendIndent(source, indent);
            source.Append("segments.Add(").Append(fieldStartVar).Append(", position, global::XenoAtom.Logging.LogMessageFormatSegmentKind.").Append(GetSegmentKind(fieldItem.Kind)).AppendLine(");");
        }
    }

    private static string GetSegmentKind(FormatterFieldKind fieldKind)
    {
        return fieldKind switch
        {
            FormatterFieldKind.Timestamp => "Timestamp",
            FormatterFieldKind.Level => "Level",
            FormatterFieldKind.LoggerName => "LoggerName",
            FormatterFieldKind.EventId => "EventId",
            FormatterFieldKind.Text => "Text",
            FormatterFieldKind.Exception => "Exception",
            FormatterFieldKind.Thread => "ThreadId",
            FormatterFieldKind.SequenceId => "SequenceId",
            FormatterFieldKind.Scope => "SecondaryText",
            FormatterFieldKind.Properties => "SecondaryText",
            _ => "Text"
        };
    }

    private static void EmitHelpers(StringBuilder source, int indent)
    {
        source.AppendLine();
        AppendIndent(source, indent);
        source.AppendLine("private static bool TryAppend(global::System.ReadOnlySpan<char> text, global::System.Span<char> destination, ref int position)");
        AppendIndent(source, indent);
        source.AppendLine("{");
        AppendIndent(source, indent + 4);
        source.AppendLine("if (text.Length > destination.Length - position) return false;");
        AppendIndent(source, indent + 4);
        source.AppendLine("text.CopyTo(destination.Slice(position));");
        AppendIndent(source, indent + 4);
        source.AppendLine("position += text.Length;");
        AppendIndent(source, indent + 4);
        source.AppendLine("return true;");
        AppendIndent(source, indent);
        source.AppendLine("}");

        source.AppendLine();
        AppendIndent(source, indent);
        source.AppendLine("private static bool TryAppend(char value, global::System.Span<char> destination, ref int position)");
        AppendIndent(source, indent);
        source.AppendLine("{");
        AppendIndent(source, indent + 4);
        source.AppendLine("if ((uint)position >= (uint)destination.Length) return false;");
        AppendIndent(source, indent + 4);
        source.AppendLine("destination[position++] = value;");
        AppendIndent(source, indent + 4);
        source.AppendLine("return true;");
        AppendIndent(source, indent);
        source.AppendLine("}");

        source.AppendLine();
        AppendIndent(source, indent);
        source.AppendLine("private static bool TryAppendString(string? text, global::System.Span<char> destination, ref int position)");
        AppendIndent(source, indent);
        source.AppendLine("{");
        AppendIndent(source, indent + 4);
        source.AppendLine("return string.IsNullOrEmpty(text) || TryAppend(text.AsSpan(), destination, ref position);");
        AppendIndent(source, indent);
        source.AppendLine("}");

        source.AppendLine();
        AppendIndent(source, indent);
        source.AppendLine("private static bool TryApplyAlignment(int alignment, int fieldStart, global::System.Span<char> destination, ref int position)");
        AppendIndent(source, indent);
        source.AppendLine("{");
        AppendIndent(source, indent + 4);
        source.AppendLine("if (alignment == 0) return true;");
        AppendIndent(source, indent + 4);
        source.AppendLine("var width = global::System.Math.Abs(alignment);");
        AppendIndent(source, indent + 4);
        source.AppendLine("var currentLength = position - fieldStart;");
        AppendIndent(source, indent + 4);
        source.AppendLine("var padding = width - currentLength;");
        AppendIndent(source, indent + 4);
        source.AppendLine("if (padding <= 0) return true;");
        AppendIndent(source, indent + 4);
        source.AppendLine("if (padding > destination.Length - position) return false;");
        AppendIndent(source, indent + 4);
        source.AppendLine("if (alignment < 0)");
        AppendIndent(source, indent + 4);
        source.AppendLine("{");
        AppendIndent(source, indent + 8);
        source.AppendLine("destination.Slice(position, padding).Fill(' ');");
        AppendIndent(source, indent + 8);
        source.AppendLine("position += padding;");
        AppendIndent(source, indent + 8);
        source.AppendLine("return true;");
        AppendIndent(source, indent + 4);
        source.AppendLine("}");
        AppendIndent(source, indent + 4);
        source.AppendLine("var tail = destination.Slice(fieldStart, currentLength);");
        AppendIndent(source, indent + 4);
        source.AppendLine("tail.CopyTo(destination.Slice(fieldStart + padding));");
        AppendIndent(source, indent + 4);
        source.AppendLine("destination.Slice(fieldStart, padding).Fill(' ');");
        AppendIndent(source, indent + 4);
        source.AppendLine("position += padding;");
        AppendIndent(source, indent + 4);
        source.AppendLine("return true;");
        AppendIndent(source, indent);
        source.AppendLine("}");

        source.AppendLine();
        AppendIndent(source, indent);
        source.AppendLine("private static bool TryWriteTimestamp(global::System.DateTime value, string format, global::System.IFormatProvider provider, global::System.Span<char> destination, ref int position)");
        AppendIndent(source, indent);
        source.AppendLine("{");
        AppendIndent(source, indent + 4);
        source.AppendLine("if (!value.TryFormat(destination.Slice(position), out var charsWritten, format.AsSpan(), provider)) return false;");
        AppendIndent(source, indent + 4);
        source.AppendLine("position += charsWritten;");
        AppendIndent(source, indent + 4);
        source.AppendLine("return true;");
        AppendIndent(source, indent);
        source.AppendLine("}");

        source.AppendLine();
        AppendIndent(source, indent);
        source.AppendLine("private static bool TryWriteLevel(global::XenoAtom.Logging.LogLevel level, global::XenoAtom.Logging.LogLevelFormat levelFormat, global::System.Span<char> destination, ref int position)");
        AppendIndent(source, indent);
        source.AppendLine("{");
        AppendIndent(source, indent + 4);
        source.AppendLine("switch (levelFormat)");
        AppendIndent(source, indent + 4);
        source.AppendLine("{");
        AppendIndent(source, indent + 8);
        source.AppendLine("case global::XenoAtom.Logging.LogLevelFormat.Short: return TryAppend(level.ToShortString().AsSpan(), destination, ref position);");
        AppendIndent(source, indent + 8);
        source.AppendLine("case global::XenoAtom.Logging.LogLevelFormat.Long: return TryAppend(level.ToLongString().AsSpan(), destination, ref position);");
        AppendIndent(source, indent + 8);
        source.AppendLine("case global::XenoAtom.Logging.LogLevelFormat.Tri: return TryAppend(level.ToTriString().AsSpan(), destination, ref position);");
        AppendIndent(source, indent + 8);
        source.AppendLine("case global::XenoAtom.Logging.LogLevelFormat.Char:");
        AppendIndent(source, indent + 12);
        source.AppendLine("var shortText = level.ToShortString();");
        AppendIndent(source, indent + 12);
        source.AppendLine("return shortText.Length > 0 && TryAppend(shortText[0], destination, ref position);");
        AppendIndent(source, indent + 8);
        source.AppendLine("default: return TryAppend(level.ToShortString().AsSpan(), destination, ref position);");
        AppendIndent(source, indent + 4);
        source.AppendLine("}");
        AppendIndent(source, indent);
        source.AppendLine("}");

        source.AppendLine();
        AppendIndent(source, indent);
        source.AppendLine("private static bool TryWriteEventId(global::XenoAtom.Logging.LogEventId eventId, int mode, global::System.Span<char> destination, ref int position)");
        AppendIndent(source, indent);
        source.AppendLine("{");
        AppendIndent(source, indent + 4);
        source.AppendLine("switch (mode)");
        AppendIndent(source, indent + 4);
        source.AppendLine("{");
        AppendIndent(source, indent + 8);
        source.AppendLine("case 1:");
        AppendIndent(source, indent + 12);
        source.AppendLine("if (!eventId.Id.TryFormat(destination.Slice(position), out var idChars)) return false;");
        AppendIndent(source, indent + 12);
        source.AppendLine("position += idChars;");
        AppendIndent(source, indent + 12);
        source.AppendLine("return true;");
        AppendIndent(source, indent + 8);
        source.AppendLine("case 2:");
        AppendIndent(source, indent + 12);
        source.AppendLine("return TryAppendString(eventId.Name, destination, ref position);");
        AppendIndent(source, indent + 8);
        source.AppendLine("default:");
        AppendIndent(source, indent + 12);
        source.AppendLine("if (!eventId.Id.TryFormat(destination.Slice(position), out var charsWritten)) return false;");
        AppendIndent(source, indent + 12);
        source.AppendLine("position += charsWritten;");
        AppendIndent(source, indent + 12);
        source.AppendLine("if (!string.IsNullOrWhiteSpace(eventId.Name))");
        AppendIndent(source, indent + 12);
        source.AppendLine("{");
        AppendIndent(source, indent + 16);
        source.AppendLine("if (!TryAppend(':', destination, ref position)) return false;");
        AppendIndent(source, indent + 16);
        source.AppendLine("if (!TryAppend(eventId.Name.AsSpan(), destination, ref position)) return false;");
        AppendIndent(source, indent + 12);
        source.AppendLine("}");
        AppendIndent(source, indent + 12);
        source.AppendLine("return true;");
        AppendIndent(source, indent + 4);
        source.AppendLine("}");
        AppendIndent(source, indent);
        source.AppendLine("}");

        source.AppendLine();
        AppendIndent(source, indent);
        source.AppendLine("private static bool TryWriteException(global::System.Exception? exception, int mode, global::System.Span<char> destination, ref int position)");
        AppendIndent(source, indent);
        source.AppendLine("{");
        AppendIndent(source, indent + 4);
        source.AppendLine("if (exception is null) return true;");
        AppendIndent(source, indent + 4);
        source.AppendLine("switch (mode)");
        AppendIndent(source, indent + 4);
        source.AppendLine("{");
        AppendIndent(source, indent + 8);
        source.AppendLine("case 1: return TryAppend(exception.Message.AsSpan(), destination, ref position);");
        AppendIndent(source, indent + 8);
        source.AppendLine("case 2: return TryAppendString(exception.GetType().FullName, destination, ref position);");
        AppendIndent(source, indent + 8);
        source.AppendLine("default: return TryAppend(exception.ToString().AsSpan(), destination, ref position);");
        AppendIndent(source, indent + 4);
        source.AppendLine("}");
        AppendIndent(source, indent);
        source.AppendLine("}");

        source.AppendLine();
        AppendIndent(source, indent);
        source.AppendLine("private static bool TryWriteThread(global::System.Threading.Thread thread, int mode, global::System.Span<char> destination, ref int position)");
        AppendIndent(source, indent);
        source.AppendLine("{");
        AppendIndent(source, indent + 4);
        source.AppendLine("if (mode == 1) return TryAppendString(thread.Name, destination, ref position);");
        AppendIndent(source, indent + 4);
        source.AppendLine("if (!thread.ManagedThreadId.TryFormat(destination.Slice(position), out var charsWritten)) return false;");
        AppendIndent(source, indent + 4);
        source.AppendLine("position += charsWritten;");
        AppendIndent(source, indent + 4);
        source.AppendLine("return true;");
        AppendIndent(source, indent);
        source.AppendLine("}");

        source.AppendLine();
        AppendIndent(source, indent);
        source.AppendLine("private static bool TryWriteSequenceId(long sequenceId, string? format, global::System.IFormatProvider provider, global::System.Span<char> destination, ref int position)");
        AppendIndent(source, indent);
        source.AppendLine("{");
        AppendIndent(source, indent + 4);
        source.AppendLine("if (string.IsNullOrEmpty(format))");
        AppendIndent(source, indent + 4);
        source.AppendLine("{");
        AppendIndent(source, indent + 8);
        source.AppendLine("if (!sequenceId.TryFormat(destination.Slice(position), out var charsWritten, default, provider)) return false;");
        AppendIndent(source, indent + 8);
        source.AppendLine("position += charsWritten;");
        AppendIndent(source, indent + 8);
        source.AppendLine("return true;");
        AppendIndent(source, indent + 4);
        source.AppendLine("}");
        AppendIndent(source, indent + 4);
        source.AppendLine("if (!sequenceId.TryFormat(destination.Slice(position), out var formattedCharsWritten, format.AsSpan(), provider)) return false;");
        AppendIndent(source, indent + 4);
        source.AppendLine("position += formattedCharsWritten;");
        AppendIndent(source, indent + 4);
        source.AppendLine("return true;");
        AppendIndent(source, indent);
        source.AppendLine("}");

        source.AppendLine();
        AppendIndent(source, indent);
        source.AppendLine("private static bool TryWriteScope(global::XenoAtom.Logging.LogScope scope, string separator, global::System.Span<char> destination, ref int position)");
        AppendIndent(source, indent);
        source.AppendLine("{");
        AppendIndent(source, indent + 4);
        source.AppendLine("var hasEntry = false;");
        AppendIndent(source, indent + 4);
        source.AppendLine("for (var scopeIndex = 0; scopeIndex < scope.Count; scopeIndex++)");
        AppendIndent(source, indent + 4);
        source.AppendLine("{");
        AppendIndent(source, indent + 8);
        source.AppendLine("if (!TryWritePropertiesCore(scope[scopeIndex], separator, ref hasEntry, destination, ref position)) return false;");
        AppendIndent(source, indent + 4);
        source.AppendLine("}");
        AppendIndent(source, indent + 4);
        source.AppendLine("return true;");
        AppendIndent(source, indent);
        source.AppendLine("}");

        source.AppendLine();
        AppendIndent(source, indent);
        source.AppendLine("private static bool TryWriteProperties(global::XenoAtom.Logging.LogPropertiesReader properties, string separator, global::System.Span<char> destination, ref int position)");
        AppendIndent(source, indent);
        source.AppendLine("{");
        AppendIndent(source, indent + 4);
        source.AppendLine("var hasEntry = false;");
        AppendIndent(source, indent + 4);
        source.AppendLine("return TryWritePropertiesCore(properties, separator, ref hasEntry, destination, ref position);");
        AppendIndent(source, indent);
        source.AppendLine("}");

        source.AppendLine();
        AppendIndent(source, indent);
        source.AppendLine("private static bool TryWritePropertiesCore(global::XenoAtom.Logging.LogPropertiesReader properties, string separator, ref bool hasEntry, global::System.Span<char> destination, ref int position)");
        AppendIndent(source, indent);
        source.AppendLine("{");
        AppendIndent(source, indent + 4);
        source.AppendLine("foreach (var property in properties)");
        AppendIndent(source, indent + 4);
        source.AppendLine("{");
        AppendIndent(source, indent + 8);
        source.AppendLine("if (hasEntry && !TryAppend(separator.AsSpan(), destination, ref position)) return false;");
        AppendIndent(source, indent + 8);
        source.AppendLine("if (!TryAppend(property.Name, destination, ref position)) return false;");
        AppendIndent(source, indent + 8);
        source.AppendLine("if (!TryAppend('=', destination, ref position)) return false;");
        AppendIndent(source, indent + 8);
        source.AppendLine("if (!TryAppend(property.Value, destination, ref position)) return false;");
        AppendIndent(source, indent + 8);
        source.AppendLine("hasEntry = true;");
        AppendIndent(source, indent + 4);
        source.AppendLine("}");
        AppendIndent(source, indent + 4);
        source.AppendLine("return true;");
        AppendIndent(source, indent);
        source.AppendLine("}");
    }

    private static bool IsPartial(INamedTypeSymbol typeSymbol)
    {
        foreach (var syntaxReference in typeSymbol.DeclaringSyntaxReferences)
        {
            if (syntaxReference.GetSyntax() is TypeDeclarationSyntax declaration &&
                declaration.Modifiers.Any(static modifier => modifier.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword)))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsPartialProperty(IPropertySymbol propertySymbol)
    {
        foreach (var syntaxReference in propertySymbol.DeclaringSyntaxReferences)
        {
            if (syntaxReference.GetSyntax() is PropertyDeclarationSyntax declaration &&
                declaration.Modifiers.Any(static modifier => modifier.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword)))
            {
                return true;
            }
        }

        return false;
    }

    private static bool InheritsLogFormatter(ITypeSymbol typeSymbol)
    {
        var current = typeSymbol;
        while (current is not null)
        {
            if (current.ToDisplayString() == LogFormatterUtilities.LogFormatterMetadataName)
            {
                return true;
            }

            current = current.BaseType;
        }

        return false;
    }

    private static List<INamedTypeSymbol> GetContainingTypes(INamedTypeSymbol? containingType)
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

    private static void AppendTypeHeader(StringBuilder builder, INamedTypeSymbol typeSymbol, int indent)
    {
        AppendIndent(builder, indent);
        var accessibility = GetAccessibilityKeyword(typeSymbol.DeclaredAccessibility);
        if (!string.IsNullOrEmpty(accessibility))
        {
            builder.Append(accessibility).Append(' ');
        }

        if (typeSymbol.IsStatic)
        {
            builder.Append("static ");
        }

        builder.Append("partial ");
        builder.Append(GetTypeKeyword(typeSymbol));
        builder.Append(' ');
        builder.Append(LogFormatterUtilities.EscapeIdentifier(typeSymbol.Name));
        builder.AppendLine();
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
            _ => string.Empty
        };
    }

    private static string GetTypeKeyword(INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol.IsRecord)
        {
            return typeSymbol.TypeKind == TypeKind.Struct ? "record struct" : "record";
        }

        return typeSymbol.TypeKind == TypeKind.Struct ? "struct" : "class";
    }

    private static string CreateHintName(ISymbol symbol)
    {
        var fullyQualifiedName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        return fullyQualifiedName
            .Replace("global::", string.Empty)
            .Replace('<', '_')
            .Replace('>', '_')
            .Replace(':', '_')
            .Replace('.', '_')
            .Replace('(', '_')
            .Replace(')', '_')
            .Replace(',', '_')
            .Replace(' ', '_');
    }

    private static void AppendIndent(StringBuilder builder, int indent)
    {
        if (indent > 0)
        {
            builder.Append(' ', indent);
        }
    }

    private static void Report(ImmutableArray<Diagnostic>.Builder diagnostics, DiagnosticDescriptor descriptor, ISymbol symbol, params object[] arguments)
    {
        var location = symbol.Locations.Length > 0 ? symbol.Locations[0] : Location.None;
        diagnostics.Add(Diagnostic.Create(descriptor, location, arguments));
    }

    private readonly struct LogFormatterGenerationResult
    {
        public LogFormatterGenerationResult(string? hintName, string? source, ImmutableArray<Diagnostic> diagnostics)
        {
            HintName = hintName;
            Source = source;
            Diagnostics = diagnostics;
        }

        public string? HintName { get; }

        public string? Source { get; }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
    }

    private sealed class LogFormatterGenerationResultComparer : IEqualityComparer<LogFormatterGenerationResult>
    {
        public static readonly LogFormatterGenerationResultComparer Instance = new();

        public bool Equals(LogFormatterGenerationResult x, LogFormatterGenerationResult y)
        {
            if (!string.Equals(x.HintName, y.HintName, StringComparison.Ordinal) ||
                !string.Equals(x.Source, y.Source, StringComparison.Ordinal))
            {
                return false;
            }

            var leftDiagnostics = x.Diagnostics;
            var rightDiagnostics = y.Diagnostics;
            if (leftDiagnostics.Length != rightDiagnostics.Length)
            {
                return false;
            }

            for (var index = 0; index < leftDiagnostics.Length; index++)
            {
                var left = leftDiagnostics[index];
                var right = rightDiagnostics[index];
                if (!string.Equals(left.Id, right.Id, StringComparison.Ordinal) ||
                    left.Severity != right.Severity ||
                    !string.Equals(left.GetMessage(CultureInfo.InvariantCulture), right.GetMessage(CultureInfo.InvariantCulture), StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(LogFormatterGenerationResult obj)
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

    private enum FormatterFieldKind
    {
        Timestamp,
        Level,
        LoggerName,
        EventId,
        Text,
        Exception,
        Thread,
        SequenceId,
        Scope,
        Properties,
        NewLine
    }

    private abstract class TemplateItem
    {
    }

    private sealed class LiteralTemplateItem : TemplateItem
    {
        public LiteralTemplateItem(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }

    private sealed class FieldTemplateItem : TemplateItem
    {
        public FieldTemplateItem(FormatterFieldKind kind, int? alignment, string? format, int mode, string separator)
        {
            Kind = kind;
            Alignment = alignment;
            Format = format;
            Mode = mode;
            Separator = separator;
        }

        public FormatterFieldKind Kind { get; }

        public int? Alignment { get; }

        public string? Format { get; }

        public int Mode { get; }

        public string Separator { get; }
    }

    private sealed class ConditionalTemplateItem : TemplateItem
    {
        public ConditionalTemplateItem(ImmutableArray<TemplateItem> items, ImmutableArray<FormatterFieldKind> fields, bool hasEmptiableField)
        {
            Items = items;
            Fields = fields;
            HasEmptiableField = hasEmptiableField;
        }

        public ImmutableArray<TemplateItem> Items { get; }

        public ImmutableArray<FormatterFieldKind> Fields { get; }

        public bool HasEmptiableField { get; }
    }

    private sealed class FormatterTemplateModel
    {
        public FormatterTemplateModel(
            ImmutableArray<TemplateItem> items,
            bool usesTimestamp,
            bool usesLevel,
            string defaultTimestampFormat,
            FormatterLevelFormat defaultLevelFormat)
        {
            Items = items;
            UsesTimestamp = usesTimestamp;
            UsesLevel = usesLevel;
            DefaultTimestampFormat = defaultTimestampFormat;
            DefaultLevelFormat = defaultLevelFormat;
        }

        public ImmutableArray<TemplateItem> Items { get; }

        public bool UsesTimestamp { get; }

        public bool UsesLevel { get; }

        public string DefaultTimestampFormat { get; }

        public FormatterLevelFormat DefaultLevelFormat { get; }
    }

    private enum FormatterLevelFormat
    {
        Short,
        Long,
        Tri,
        Char
    }
}
