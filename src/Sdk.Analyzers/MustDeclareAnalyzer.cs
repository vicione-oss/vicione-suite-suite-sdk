using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Sdk.Analyzers;

/// <summary>
/// Analyzer that enforces <c>[MustDeclare(typeof(TAttribute))]</c>: when a concrete type implements
/// an interface (or derives from a base class) carrying that constraint, the type must itself be
/// decorated with the required attribute. For <c>IActivityArgument</c> this guards the
/// <c>MessageEndpointAttribute</c> whose absence makes <c>MessagingHelper.GetActivityEndpointName</c>
/// fail at runtime.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MustDeclareAnalyzer : DiagnosticAnalyzer
{
    private const string MustDeclareAttributeFullName = "Sdk.Backend.Diagnostics.MustDeclareAttribute";

    private static readonly DiagnosticDescriptor s_missingRequiredAttributeDiagnostic = new(
        id: "VOSDK001",
        title: "Required attribute is missing",
        messageFormat: "Type '{0}' implements '{1}' and must be decorated with the [{2}] attribute",
        category: "Sdk.Analyzers",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
        = [s_missingRequiredAttributeDiagnostic];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static startContext =>
        {
            var mustDeclareAttributeType = startContext.Compilation.GetTypeByMetadataName(MustDeclareAttributeFullName);
            if (mustDeclareAttributeType is null)
                return;

            startContext.RegisterSymbolAction(
                symbolContext => AnalyzeNamedType(symbolContext, mustDeclareAttributeType),
                SymbolKind.NamedType);
        });
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context, INamedTypeSymbol mustDeclareAttributeType)
    {
        var type = (INamedTypeSymbol)context.Symbol;

        // Abstract classes and interfaces only carry the constraint; only concrete implementations
        // end up being instantiated and must actually satisfy it.
        if (type.IsAbstract || (type.TypeKind != TypeKind.Class && type.TypeKind != TypeKind.Struct))
            return;

        foreach (var (requiredAttributeType, constraintType) in GetRequiredAttributes(type, mustDeclareAttributeType))
        {
            if (DeclaresAttribute(type, requiredAttributeType))
                continue;

            context.ReportDiagnostic(Diagnostic.Create(
                s_missingRequiredAttributeDiagnostic,
                type.Locations[0],
                type.Name,
                constraintType.Name,
                StripAttributeSuffix(requiredAttributeType.Name)));
        }
    }

    /// <summary>
    /// Collects the attribute types required by every constraint in the type's hierarchy, keeping
    /// the first constraint that introduced each requirement so duplicates are reported only once.
    /// </summary>
    private static IEnumerable<(INamedTypeSymbol RequiredAttributeType, INamedTypeSymbol ConstraintType)> GetRequiredAttributes(
        INamedTypeSymbol type,
        INamedTypeSymbol mustDeclareAttributeType)
    {
        var seen = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

        foreach (var constraintType in EnumerateHierarchy(type))
        {
            foreach (var attribute in constraintType.GetAttributes())
            {
                if (!SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, mustDeclareAttributeType))
                    continue;

                if (attribute.ConstructorArguments.Length == 0
                    || attribute.ConstructorArguments[0].Value is not INamedTypeSymbol requiredAttributeType)
                {
                    continue;
                }

                if (seen.Add(requiredAttributeType))
                    yield return (requiredAttributeType, constraintType);
            }
        }
    }

    private static IEnumerable<INamedTypeSymbol> EnumerateHierarchy(INamedTypeSymbol type)
    {
        for (var current = type; current is not null; current = current.BaseType)
            yield return current;

        foreach (var implementedInterface in type.AllInterfaces)
            yield return implementedInterface;
    }

    /// <summary>
    /// The runtime lookup (MassTransit's <c>GetAttribute</c>) resolves attributes on the type itself
    /// and its base classes, but not on implemented interfaces — mirror that exactly. A subtype of
    /// the required attribute satisfies the requirement.
    /// </summary>
    private static bool DeclaresAttribute(INamedTypeSymbol type, INamedTypeSymbol requiredAttributeType)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            foreach (var attribute in current.GetAttributes())
            {
                if (IsOrDerivesFrom(attribute.AttributeClass, requiredAttributeType))
                    return true;
            }
        }

        return false;
    }

    private static bool IsOrDerivesFrom(INamedTypeSymbol? type, INamedTypeSymbol baseType)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
                return true;
        }

        return false;
    }

    private static string StripAttributeSuffix(string name)
        => name.EndsWith("Attribute", StringComparison.Ordinal)
            ? name.Substring(0, name.Length - "Attribute".Length)
            : name;
}
