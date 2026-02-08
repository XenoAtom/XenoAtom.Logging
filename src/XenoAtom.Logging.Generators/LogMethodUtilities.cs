// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace XenoAtom.Logging.Generators;

internal static class LogMethodUtilities
{
    public const string LogMethodAttributeMetadataName = "XenoAtom.Logging.LogMethodAttribute";
    public const string LoggerMetadataName = "XenoAtom.Logging.Logger";
    public const string LogPropertiesMetadataName = "XenoAtom.Logging.LogProperties";

    private static readonly SymbolDisplayFormat TypeDisplayFormat =
        SymbolDisplayFormat.FullyQualifiedFormat.WithMiscellaneousOptions(
            SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier |
            SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

    public static bool TryGetLogMethodAttribute(IMethodSymbol methodSymbol, out AttributeData? attribute)
    {
        foreach (var candidate in methodSymbol.GetAttributes())
        {
            if (candidate.AttributeClass?.ToDisplayString() == LogMethodAttributeMetadataName)
            {
                attribute = candidate;
                return true;
            }
        }

        attribute = null;
        return false;
    }

    public static string FormatType(ITypeSymbol typeSymbol) => typeSymbol.ToDisplayString(TypeDisplayFormat);

    public static bool IsLoggerParameter(IParameterSymbol parameterSymbol)
        => parameterSymbol.Type.ToDisplayString() == LoggerMetadataName;

    public static bool IsLogPropertiesParameter(IParameterSymbol parameterSymbol)
        => parameterSymbol.Type.ToDisplayString() == LogPropertiesMetadataName;

    public static bool IsExceptionType(ITypeSymbol typeSymbol)
    {
        var current = typeSymbol;
        while (current is not null)
        {
            if (current.ToDisplayString() == "System.Exception")
            {
                return true;
            }

            current = current.BaseType;
        }

        return false;
    }

    public static bool IsAllocationFriendlyType(ITypeSymbol typeSymbol, Compilation compilation)
    {
        if (typeSymbol.SpecialType is SpecialType.System_String or SpecialType.System_Boolean)
        {
            return true;
        }

        if (typeSymbol is INamedTypeSymbol namedType &&
            namedType.IsGenericType &&
            namedType.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
        {
            var underlyingType = namedType.TypeArguments[0];
            return IsUnmanagedSpanFormattable(underlyingType, compilation);
        }

        return IsUnmanagedSpanFormattable(typeSymbol, compilation);
    }

    public static string EscapeIdentifier(string identifier)
        => SyntaxFacts.GetKeywordKind(identifier) != SyntaxKind.None ? $"@{identifier}" : identifier;

    public static string EscapeInterpolatedLiteral(string text)
    {
        var builder = new StringBuilder(text.Length);
        foreach (var character in text)
        {
            switch (character)
            {
                case '{':
                    builder.Append("{{");
                    break;
                case '}':
                    builder.Append("}}");
                    break;
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

        return builder.ToString();
    }

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

    public static string? GetLogMethodName(int logLevelValue)
    {
        return logLevelValue switch
        {
            1 => "Trace",
            2 => "Debug",
            3 => "Info",
            4 => "Warn",
            5 => "Error",
            6 => "Fatal",
            _ => null,
        };
    }

    public static string GetLogLevelName(int logLevelValue)
    {
        return logLevelValue switch
        {
            0 => "All",
            1 => "Trace",
            2 => "Debug",
            3 => "Info",
            4 => "Warn",
            5 => "Error",
            6 => "Fatal",
            7 => "None",
            _ => logLevelValue.ToString(CultureInfo.InvariantCulture),
        };
    }

    public static bool TryParseTemplate(string template, out ImmutableArray<TemplateToken> tokens, out string error)
    {
        var builder = ImmutableArray.CreateBuilder<TemplateToken>();
        var literal = new StringBuilder();

        for (var index = 0; index < template.Length; index++)
        {
            var character = template[index];

            if (character == '{')
            {
                if (index + 1 < template.Length && template[index + 1] == '{')
                {
                    literal.Append('{');
                    index++;
                    continue;
                }

                if (literal.Length > 0)
                {
                    builder.Add(TemplateToken.Literal(literal.ToString()));
                    literal.Clear();
                }

                var start = index + 1;
                var end = start;
                while (end < template.Length && template[end] != '}')
                {
                    if (template[end] == '{')
                    {
                        tokens = default;
                        error = "Nested placeholders are not supported.";
                        return false;
                    }

                    end++;
                }

                if (end >= template.Length)
                {
                    tokens = default;
                    error = "Missing closing '}' in the message template.";
                    return false;
                }

                var placeholderText = template.Substring(start, end - start);
                if (!TryParsePlaceholder(placeholderText, out var placeholderName, out var alignment, out var format, out error))
                {
                    tokens = default;
                    return false;
                }

                builder.Add(TemplateToken.Placeholder(placeholderName, alignment, format));
                index = end;
                continue;
            }

            if (character == '}')
            {
                if (index + 1 < template.Length && template[index + 1] == '}')
                {
                    literal.Append('}');
                    index++;
                    continue;
                }

                tokens = default;
                error = "Unexpected '}' in the message template.";
                return false;
            }

            literal.Append(character);
        }

        if (literal.Length > 0)
        {
            builder.Add(TemplateToken.Literal(literal.ToString()));
        }

        tokens = builder.ToImmutable();
        error = string.Empty;
        return true;
    }

    private static bool TryParsePlaceholder(
        string placeholderText,
        out string name,
        out int? alignment,
        out string? format,
        out string error)
    {
        var text = placeholderText.AsSpan().Trim();
        if (text.IsEmpty)
        {
            name = string.Empty;
            alignment = null;
            format = null;
            error = "An empty placeholder was found in the message template.";
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

        name = text.Slice(0, endOfName).Trim().ToString();
        if (name.Length == 0)
        {
            alignment = null;
            format = null;
            error = "A placeholder is missing its parameter name.";
            return false;
        }

        alignment = null;
        format = null;

        var cursor = endOfName;
        if (cursor < text.Length && text[cursor] == ',')
        {
            cursor++;
            var alignmentStart = cursor;
            while (cursor < text.Length && text[cursor] != ':')
            {
                cursor++;
            }

            var alignmentText = text.Slice(alignmentStart, cursor - alignmentStart).Trim();
            var alignmentString = alignmentText.ToString();
            if (!int.TryParse(alignmentString, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedAlignment))
            {
                error = $"Invalid alignment '{alignmentString}' for placeholder '{name}'.";
                return false;
            }

            alignment = parsedAlignment;
        }

        if (cursor < text.Length && text[cursor] == ':')
        {
            cursor++;
            format = text.Slice(cursor, text.Length - cursor).ToString();
        }

        error = string.Empty;
        return true;
    }

    private static bool IsUnmanagedSpanFormattable(ITypeSymbol typeSymbol, Compilation compilation)
    {
        if (!typeSymbol.IsValueType || !typeSymbol.IsUnmanagedType)
        {
            return false;
        }

        var spanFormattable = compilation.GetTypeByMetadataName("System.ISpanFormattable");
        if (spanFormattable is null)
        {
            return false;
        }

        foreach (var item in typeSymbol.AllInterfaces)
        {
            if (SymbolEqualityComparer.Default.Equals(item, spanFormattable))
            {
                return true;
            }
        }

        return false;
    }
}

internal readonly struct TemplateToken
{
    private TemplateToken(bool isPlaceholder, string text, int? alignment, string? format)
    {
        IsPlaceholder = isPlaceholder;
        Text = text;
        Alignment = alignment;
        Format = format;
    }

    public bool IsPlaceholder { get; }

    public string Text { get; }

    public int? Alignment { get; }

    public string? Format { get; }

    public static TemplateToken Literal(string text) => new(false, text, null, null);

    public static TemplateToken Placeholder(string name, int? alignment, string? format) => new(true, name, alignment, format);
}
