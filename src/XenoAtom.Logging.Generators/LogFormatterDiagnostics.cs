// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace XenoAtom.Logging.Generators;

internal static class LogFormatterDiagnostics
{
    public static readonly DiagnosticDescriptor UnknownField = new(
        id: "XLF0001",
        title: "Unknown formatter field",
        messageFormat: "Unknown formatter field '{0}'.",
        category: "XenoAtom.Logging.Generators",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MalformedTemplate = new(
        id: "XLF0002",
        title: "Malformed formatter template",
        messageFormat: "Malformed formatter template: {0}",
        category: "XenoAtom.Logging.Generators",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidFieldFormat = new(
        id: "XLF0003",
        title: "Invalid formatter field format specifier",
        messageFormat: "Invalid format specifier '{0}' for field '{1}'.",
        category: "XenoAtom.Logging.Generators",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidTypeUsage = new(
        id: "XLF0004",
        title: "Invalid formatter type usage",
        messageFormat: "{0}",
        category: "XenoAtom.Logging.Generators",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidPropertyUsage = new(
        id: "XLF0005",
        title: "Invalid formatter property usage",
        messageFormat: "{0}",
        category: "XenoAtom.Logging.Generators",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ConditionalAlwaysEmitted = new(
        id: "XLF0006",
        title: "Conditional section always emitted",
        messageFormat: "Conditional section has no emptyable fields and is always emitted.",
        category: "XenoAtom.Logging.Generators",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
