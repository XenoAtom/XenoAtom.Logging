// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace XenoAtom.Logging.Generators;

internal static class LogMethodDiagnostics
{
    public static readonly DiagnosticDescriptor InvalidMethodSignature = new(
        id: "XLG0001",
        title: "Invalid [LogMethod] or [LogMethodMarkup] signature",
        messageFormat: "Method '{0}' cannot be source-generated: {1}",
        category: "XenoAtom.Logging.Generators",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidLogLevel = new(
        id: "XLG0002",
        title: "Invalid log level",
        messageFormat: "Method '{0}' uses unsupported log level '{1}'. Supported values are Trace, Debug, Info, Warn, Error, and Fatal.",
        category: "XenoAtom.Logging.Generators",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor TemplateParseError = new(
        id: "XLG0003",
        title: "Invalid message template",
        messageFormat: "Method '{0}' has an invalid message template: {1}",
        category: "XenoAtom.Logging.Generators",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor TemplateParameterNotFound = new(
        id: "XLG0004",
        title: "Template parameter not found",
        messageFormat: "Method '{0}' references template parameter '{1}' that does not match any method parameter.",
        category: "XenoAtom.Logging.Generators",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MissingMarkupSupport = new(
        id: "XLG0005",
        title: "Missing markup logging support",
        messageFormat: "Method '{0}' uses [LogMethodMarkup], but the XenoAtom.Logging.Terminal package is not referenced.",
        category: "XenoAtom.Logging.Generators",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor AllocationRiskParameter = new(
        id: "XLG0100",
        title: "Potential logging allocation",
        messageFormat: "Parameter '{0}' (type '{1}') is not allocation-friendly for generated logging. Prefer string, bool, or unmanaged ISpanFormattable values.",
        category: "XenoAtom.Logging.Performance",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
