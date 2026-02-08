// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace XenoAtom.Logging.Generators;

/// <summary>
/// Reports placeholders that map to parameter types likely to allocate during generated logging.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LogMethodAllocationAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(LogMethodDiagnostics.AllocationRiskParameter);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
    }

    private static void AnalyzeMethod(SymbolAnalysisContext context)
    {
        if (context.Symbol is not IMethodSymbol methodSymbol)
        {
            return;
        }

        if (!LogMethodUtilities.TryGetLogMethodAttribute(methodSymbol, out var attribute))
        {
            return;
        }

        if (attribute is null)
        {
            return;
        }

        if (attribute.ConstructorArguments.Length < 2)
        {
            return;
        }

        var messageTemplate = attribute.ConstructorArguments[1].Value as string;
        if (string.IsNullOrEmpty(messageTemplate))
        {
            return;
        }

        var nonNullTemplate = messageTemplate!;
        if (!LogMethodUtilities.TryParseTemplate(nonNullTemplate, out var tokens, out _))
        {
            return;
        }

        if (methodSymbol.Parameters.Length == 0 || !LogMethodUtilities.IsLoggerParameter(methodSymbol.Parameters[0]))
        {
            return;
        }

        IParameterSymbol? propertiesParameter = null;
        foreach (var parameter in methodSymbol.Parameters)
        {
            if (LogMethodUtilities.IsLogPropertiesParameter(parameter))
            {
                propertiesParameter = parameter;
                break;
            }
        }

        var templateParameters = new Dictionary<string, IParameterSymbol>(StringComparer.OrdinalIgnoreCase);
        foreach (var parameter in methodSymbol.Parameters)
        {
            if (SymbolEqualityComparer.Default.Equals(parameter, methodSymbol.Parameters[0]))
            {
                continue;
            }

            if (propertiesParameter is not null && SymbolEqualityComparer.Default.Equals(parameter, propertiesParameter))
            {
                continue;
            }

            templateParameters[parameter.Name] = parameter;
        }

        var warned = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var token in tokens)
        {
            if (!token.IsPlaceholder)
            {
                continue;
            }

            if (!templateParameters.TryGetValue(token.Text, out var parameterSymbol))
            {
                continue;
            }

            if (!warned.Add(parameterSymbol.Name))
            {
                continue;
            }

            if (LogMethodUtilities.IsAllocationFriendlyType(parameterSymbol.Type, context.Compilation))
            {
                continue;
            }

            var location = parameterSymbol.Locations.Length > 0 ? parameterSymbol.Locations[0] : methodSymbol.Locations[0];
            context.ReportDiagnostic(
                Diagnostic.Create(
                    LogMethodDiagnostics.AllocationRiskParameter,
                    location,
                    parameterSymbol.Name,
                    parameterSymbol.Type.ToDisplayString()));
        }
    }
}
