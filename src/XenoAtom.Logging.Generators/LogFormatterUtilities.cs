// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace XenoAtom.Logging.Generators;

internal static class LogFormatterUtilities
{
    public const string LogFormatterAttributeMetadataName = "XenoAtom.Logging.LogFormatterAttribute";
    public const string LogFormatterMetadataName = "XenoAtom.Logging.LogFormatter";

    private static readonly SymbolDisplayFormat TypeDisplayFormat =
        SymbolDisplayFormat.FullyQualifiedFormat.WithMiscellaneousOptions(
            SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier |
            SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

    public static bool TryGetLogFormatterAttribute(ISymbol symbol, out AttributeData? attribute)
    {
        foreach (var candidate in symbol.GetAttributes())
        {
            if (candidate.AttributeClass?.ToDisplayString() == LogFormatterAttributeMetadataName)
            {
                attribute = candidate;
                return true;
            }
        }

        attribute = null;
        return false;
    }

    public static string FormatType(ITypeSymbol typeSymbol) => typeSymbol.ToDisplayString(TypeDisplayFormat);

    public static string EscapeIdentifier(string identifier)
        => SyntaxFacts.GetKeywordKind(identifier) != SyntaxKind.None ? $"@{identifier}" : identifier;

    public static string ToStringLiteral(string text)
    {
        var builder = new StringBuilder(text.Length + 2);
        builder.Append('"');
        foreach (var character in text)
        {
            switch (character)
            {
                case '"':
                    builder.Append("\\\"");
                    break;
                case '\\':
                    builder.Append("\\\\");
                    break;
                case '\r':
                    builder.Append("\\r");
                    break;
                case '\n':
                    builder.Append("\\n");
                    break;
                case '\t':
                    builder.Append("\\t");
                    break;
                default:
                    builder.Append(character);
                    break;
            }
        }

        builder.Append('"');
        return builder.ToString();
    }

    public static bool TryParseTemplate(string template, out ImmutableArray<FormatterTemplateNode> nodes, out string error)
    {
        var index = 0;
        var success = TryParseNodes(template, ref index, insideConditional: false, out nodes, out error);
        if (!success)
        {
            nodes = default;
            return false;
        }

        if (index != template.Length)
        {
            nodes = default;
            error = "Malformed template: unexpected trailing content.";
            return false;
        }

        return true;
    }

    private static bool TryParseNodes(
        string template,
        ref int index,
        bool insideConditional,
        out ImmutableArray<FormatterTemplateNode> nodes,
        out string error)
    {
        var builder = ImmutableArray.CreateBuilder<FormatterTemplateNode>();
        var literalBuilder = new StringBuilder();

        while (index < template.Length)
        {
            if (insideConditional &&
                index + 1 < template.Length &&
                template[index] == '?' &&
                template[index + 1] == '}')
            {
                FlushLiteral(literalBuilder, builder);
                index += 2;
                nodes = builder.ToImmutable();
                error = string.Empty;
                return true;
            }

            var character = template[index];
            if (character == '{')
            {
                if (index + 1 < template.Length && template[index + 1] == '{')
                {
                    literalBuilder.Append('{');
                    index += 2;
                    continue;
                }

                if (index + 1 < template.Length && template[index + 1] == '?')
                {
                    if (insideConditional)
                    {
                        nodes = default;
                        error = "Malformed template: nested conditional sections are not supported.";
                        return false;
                    }

                    FlushLiteral(literalBuilder, builder);
                    index += 2;

                    if (!TryParseNodes(template, ref index, insideConditional: true, out var conditionalNodes, out error))
                    {
                        nodes = default;
                        return false;
                    }

                    if (!ContainsField(conditionalNodes))
                    {
                        nodes = default;
                        error = "Malformed template: conditional section must contain at least one field.";
                        return false;
                    }

                    builder.Add(FormatterTemplateNode.CreateConditional(conditionalNodes));
                    continue;
                }

                FlushLiteral(literalBuilder, builder);

                index++;
                var placeholderStart = index;
                while (index < template.Length && template[index] != '}')
                {
                    if (template[index] == '{')
                    {
                        nodes = default;
                        error = "Malformed template: nested '{' in field placeholder.";
                        return false;
                    }

                    index++;
                }

                if (index >= template.Length)
                {
                    nodes = default;
                    error = "Malformed template: missing closing '}' in field placeholder.";
                    return false;
                }

                var placeholderText = template.Substring(placeholderStart, index - placeholderStart);
                if (!TryParseField(placeholderText, out var field, out error))
                {
                    nodes = default;
                    return false;
                }

                builder.Add(FormatterTemplateNode.CreateField(field));
                index++;
                continue;
            }

            if (character == '}')
            {
                if (index + 1 < template.Length && template[index + 1] == '}')
                {
                    literalBuilder.Append('}');
                    index += 2;
                    continue;
                }

                nodes = default;
                error = "Malformed template: unexpected '}'.";
                return false;
            }

            literalBuilder.Append(character);
            index++;
        }

        if (insideConditional)
        {
            nodes = default;
            error = "Malformed template: missing closing '?}' for conditional section.";
            return false;
        }

        FlushLiteral(literalBuilder, builder);
        nodes = builder.ToImmutable();
        error = string.Empty;
        return true;
    }

    private static bool ContainsField(ImmutableArray<FormatterTemplateNode> nodes)
    {
        foreach (var node in nodes)
        {
            if (node.Kind == FormatterTemplateNodeKind.Field)
            {
                return true;
            }
        }

        return false;
    }

    private static void FlushLiteral(StringBuilder literalBuilder, ImmutableArray<FormatterTemplateNode>.Builder builder)
    {
        if (literalBuilder.Length == 0)
        {
            return;
        }

        builder.Add(FormatterTemplateNode.CreateLiteral(literalBuilder.ToString()));
        literalBuilder.Clear();
    }

    private static bool TryParseField(string placeholderText, out FormatterFieldToken field, out string error)
    {
        var text = placeholderText.AsSpan().Trim();
        if (text.IsEmpty)
        {
            field = default;
            error = "Malformed template: empty field placeholder.";
            return false;
        }

        var commaIndex = text.IndexOf(',');
        var colonIndex = text.IndexOf(':');
        var endOfName = text.Length;
        if (commaIndex >= 0 && commaIndex < endOfName)
        {
            endOfName = commaIndex;
        }

        if (colonIndex >= 0 && colonIndex < endOfName)
        {
            endOfName = colonIndex;
        }

        var name = text.Slice(0, endOfName).Trim().ToString();
        if (name.Length == 0)
        {
            field = default;
            error = "Malformed template: field name cannot be empty.";
            return false;
        }

        int? alignment = null;
        string? format = null;

        var cursor = endOfName;
        if (cursor < text.Length && text[cursor] == ',')
        {
            cursor++;
            var alignmentStart = cursor;
            while (cursor < text.Length && text[cursor] != ':')
            {
                cursor++;
            }

            var alignmentText = text.Slice(alignmentStart, cursor - alignmentStart).Trim().ToString();
            if (!int.TryParse(alignmentText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedAlignment))
            {
                field = default;
                error = $"Malformed template: invalid alignment '{alignmentText}' for field '{name}'.";
                return false;
            }

            alignment = parsedAlignment;
        }

        if (cursor < text.Length && text[cursor] == ':')
        {
            cursor++;
            format = text.Slice(cursor).ToString();
        }

        field = new FormatterFieldToken(name, alignment, format);
        error = string.Empty;
        return true;
    }
}

internal enum FormatterTemplateNodeKind
{
    Literal,
    Field,
    Conditional
}

internal readonly struct FormatterTemplateNode
{
    private FormatterTemplateNode(FormatterTemplateNodeKind kind, string literal, FormatterFieldToken field, ImmutableArray<FormatterTemplateNode> children)
    {
        Kind = kind;
        Literal = literal;
        Field = field;
        Children = children;
    }

    public FormatterTemplateNodeKind Kind { get; }

    public string Literal { get; }

    public FormatterFieldToken Field { get; }

    public ImmutableArray<FormatterTemplateNode> Children { get; }

    public static FormatterTemplateNode CreateLiteral(string text) => new(FormatterTemplateNodeKind.Literal, text, default, default);

    public static FormatterTemplateNode CreateField(FormatterFieldToken field) => new(FormatterTemplateNodeKind.Field, string.Empty, field, default);

    public static FormatterTemplateNode CreateConditional(ImmutableArray<FormatterTemplateNode> children) => new(FormatterTemplateNodeKind.Conditional, string.Empty, default, children);
}

internal readonly struct FormatterFieldToken
{
    public FormatterFieldToken(string name, int? alignment, string? format)
    {
        Name = name;
        Alignment = alignment;
        Format = format;
    }

    public string Name { get; }

    public int? Alignment { get; }

    public string? Format { get; }
}
