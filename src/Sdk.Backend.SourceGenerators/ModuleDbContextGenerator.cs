using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Sdk.Backend.SourceGenerators;

/// <summary>
/// Incremental source generator that produces SQLite/PostgreSQL DbContext subclasses
/// and design-time factories for classes annotated with <c>[ModuleDbContext]</c>.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class ModuleDbContextGenerator : IIncrementalGenerator
{
    private const string AttributeFullName = "Sdk.Backend.Persistence.ModuleDbContextAttribute";
    private const string ModuleDbContextFullName = "Sdk.Backend.Persistence.ModuleDbContext";
    private const string IModuleDbContextFullName = "Sdk.Backend.Persistence.IModuleDbContext";

    private static readonly DiagnosticDescriptor s_mustBePartialDiagnostic = new(
        id: "SDKDBCTX001",
        title: "ModuleDbContext class must be partial",
        messageFormat: "Class '{0}' is annotated with [ModuleDbContext] but is not declared as partial",
        category: "Sdk.Backend.SourceGenerators",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor s_mustDeriveFromModuleDbContextDiagnostic = new(
        id: "SDKDBCTX002",
        title: "ModuleDbContext class must derive from ModuleDbContext",
        messageFormat: "Class '{0}' is annotated with [ModuleDbContext] but does not derive from ModuleDbContext",
        category: "Sdk.Backend.SourceGenerators",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                AttributeFullName,
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (ctx, _) => GetGenerationModel(ctx))
            .Where(static m => m is not null)
            .Select(static (m, _) => m!);

        context.RegisterSourceOutput(classDeclarations, static (spc, model) => Execute(spc, model));
    }

    private static GenerationResult? GetGenerationModel(GeneratorAttributeSyntaxContext context)
    {
        if (context.TargetSymbol is not INamedTypeSymbol classSymbol)
            return null;

        var syntax = (ClassDeclarationSyntax)context.TargetNode;
        var isPartial = syntax.Modifiers.Any(SyntaxKind.PartialKeyword);
        var derivesFromBase = DerivesFrom(classSymbol, ModuleDbContextFullName);

        if (!isPartial)
        {
            return new GenerationResult(
                ClassName: classSymbol.Name,
                NamespaceName: classSymbol.ContainingNamespace.ToDisplayString(),
                AccessModifier: null,
                DefaultSchemaName: null,
                GenerateConstructor: false,
                DbSetProperties: default,
                ErrorId: ErrorKind.NotPartial);
        }

        if (!derivesFromBase)
        {
            return new GenerationResult(
                ClassName: classSymbol.Name,
                NamespaceName: classSymbol.ContainingNamespace.ToDisplayString(),
                AccessModifier: null,
                DefaultSchemaName: null,
                GenerateConstructor: false,
                DbSetProperties: default,
                ErrorId: ErrorKind.DoesNotDeriveFromBase);
        }

        // Read DefaultSchemaName from the attribute
        string? defaultSchemaName = null;
        foreach (var attr in context.Attributes)
        {
            foreach (var namedArg in attr.NamedArguments)
            {
                if (namedArg.Key == "DefaultSchemaName" && namedArg.Value.Value is string schema)
                    defaultSchemaName = schema;
            }
        }

        // Generate constructor only if the user hasn't declared one
        var hasUserConstructor = false;
        foreach (var ctor in classSymbol.Constructors)
        {
            if (!ctor.IsImplicitlyDeclared)
            {
                hasUserConstructor = true;
                break;
            }
        }

        // Collect DbSet<T> properties from the IModuleDbContext-derived interface
        // that the user hasn't already implemented on the class
        var dbSetProperties = GetDbSetPropertiesToGenerate(classSymbol);

        var accessibility = classSymbol.DeclaredAccessibility;
        var accessModifier = accessibility == Accessibility.Internal ? "internal" : "public";

        return new GenerationResult(
            ClassName: classSymbol.Name,
            NamespaceName: classSymbol.ContainingNamespace.ToDisplayString(),
            AccessModifier: accessModifier,
            DefaultSchemaName: defaultSchemaName,
            GenerateConstructor: !hasUserConstructor,
            DbSetProperties: dbSetProperties,
            ErrorId: null);
    }

    private static EquatableArray<DbSetPropertyInfo> GetDbSetPropertiesToGenerate(INamedTypeSymbol classSymbol)
    {
        // Find the interface that directly extends IModuleDbContext (not IModuleDbContext itself)
        INamedTypeSymbol? moduleInterface = null;
        foreach (var iface in classSymbol.AllInterfaces)
        {
            if (iface.ToDisplayString() == IModuleDbContextFullName)
                continue;

            foreach (var baseIface in iface.AllInterfaces)
            {
                if (baseIface.ToDisplayString() == IModuleDbContextFullName)
                {
                    moduleInterface = iface;
                    break;
                }
            }

            if (moduleInterface is not null)
                break;
        }

        if (moduleInterface is null)
            return new EquatableArray<DbSetPropertyInfo>(Array.Empty<DbSetPropertyInfo>());

        // Collect existing property names declared directly on the class (across all partial declarations)
        var existingProperties = new HashSet<string>(StringComparer.Ordinal);
        foreach (var member in classSymbol.GetMembers())
        {
            if (member is IPropertySymbol prop)
                existingProperties.Add(prop.Name);
        }

        // Collect DbSet<T> properties from the interface that aren't already on the class
        var result = new List<DbSetPropertyInfo>();
        foreach (var member in moduleInterface.GetMembers())
        {
            if (member is not IPropertySymbol property)
                continue;

            if (property.Type is not INamedTypeSymbol propertyType || !propertyType.IsGenericType)
                continue;

            if (propertyType.ConstructedFrom.Name != "DbSet" ||
                propertyType.ConstructedFrom.ContainingNamespace.ToDisplayString() != "Microsoft.EntityFrameworkCore")
                continue;

            if (existingProperties.Contains(property.Name))
                continue;

            var entityType = propertyType.TypeArguments[0];
            var fullyQualifiedEntityType = entityType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            result.Add(new DbSetPropertyInfo(property.Name, fullyQualifiedEntityType));
        }

        return new EquatableArray<DbSetPropertyInfo>([.. result]);
    }

    private static bool DerivesFrom(INamedTypeSymbol symbol, string baseTypeFullName)
    {
        var current = symbol.BaseType;
        while (current is not null)
        {
            if (current.ToDisplayString() == baseTypeFullName)
                return true;
            current = current.BaseType;
        }

        return false;
    }

    private static void Execute(SourceProductionContext context, GenerationResult result)
    {
        if (result.ErrorId is not null)
        {
            var descriptor = result.ErrorId == ErrorKind.NotPartial
                ? s_mustBePartialDiagnostic
                : s_mustDeriveFromModuleDbContextDiagnostic;

            context.ReportDiagnostic(Diagnostic.Create(descriptor, Location.None, result.ClassName));
            return;
        }

        var source = GenerateSource(result);
        context.AddSource($"{result.ClassName}.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static string GenerateSource(GenerationResult model)
    {
        var ns = model.NamespaceName;
        var name = model.ClassName;
        var access = model.AccessModifier;

        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine("using Microsoft.EntityFrameworkCore.Design;");
        sb.AppendLine("using Sdk.Backend.Persistence;");
        sb.AppendLine();
        sb.AppendLine($"namespace {ns};");

        // Partial class members (constructor, DefaultSchemaName, DbSet properties)
        if (model.GenerateConstructor || model.DefaultSchemaName is not null || model.DbSetProperties.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine($"{access} partial class {name}");
            sb.AppendLine("{");

            if (model.GenerateConstructor)
                sb.AppendLine($"    protected {name}(DbContextOptions options) : base(options) {{ }}");

            if (model.DefaultSchemaName is not null)
            {
                if (model.GenerateConstructor)
                    sb.AppendLine();
                sb.AppendLine($"    public override string DefaultSchemaName => \"{model.DefaultSchemaName}\";");
            }

            foreach (var dbSet in model.DbSetProperties)
            {
                if (model.GenerateConstructor || model.DefaultSchemaName is not null || dbSet != model.DbSetProperties[0])
                    sb.AppendLine();
                sb.AppendLine($"    public DbSet<{dbSet.EntityTypeFullyQualified}> {dbSet.PropertyName} => Set<{dbSet.EntityTypeFullyQualified}>();");
            }

            sb.AppendLine("}");
        }

        // Sqlite / Postgres subclasses
        sb.AppendLine();
        sb.AppendLine($"{access} sealed class {name}Sqlite(DbContextOptions<{name}Sqlite> options) : {name}(options), ISqliteDbContext;");
        sb.AppendLine();
        sb.AppendLine($"{access} sealed class {name}Postgres(DbContextOptions<{name}Postgres> options) : {name}(options), IPostgresDbContext;");

        // Design-time factories
        sb.AppendLine();
        sb.AppendLine($"{access} sealed class {name}SqliteFactory : IDesignTimeDbContextFactory<{name}Sqlite>");
        sb.AppendLine("{");
        sb.AppendLine($"    public {name}Sqlite CreateDbContext(string[] args)");
        sb.AppendLine("    {");
        sb.AppendLine($"        var optionsBuilder = new DbContextOptionsBuilder<{name}Sqlite>();");
        sb.AppendLine("        optionsBuilder.UseSqlite();");
        sb.AppendLine($"        return new {name}Sqlite(optionsBuilder.Options);");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        sb.AppendLine();
        sb.AppendLine($"{access} sealed class {name}PostgresFactory : IDesignTimeDbContextFactory<{name}Postgres>");
        sb.AppendLine("{");
        sb.AppendLine($"    public {name}Postgres CreateDbContext(string[] args)");
        sb.AppendLine("    {");
        sb.AppendLine($"        var optionsBuilder = new DbContextOptionsBuilder<{name}Postgres>();");
        sb.AppendLine("        optionsBuilder.UseNpgsql();");
        sb.AppendLine($"        return new {name}Postgres(optionsBuilder.Options);");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private enum ErrorKind
    {
        NotPartial,
        DoesNotDeriveFromBase
    }

    private sealed record GenerationResult(
        string ClassName,
        string NamespaceName,
        string? AccessModifier,
        string? DefaultSchemaName,
        bool GenerateConstructor,
        EquatableArray<DbSetPropertyInfo> DbSetProperties,
        ErrorKind? ErrorId);

    private sealed record DbSetPropertyInfo(
        string PropertyName,
        string EntityTypeFullyQualified);
}
