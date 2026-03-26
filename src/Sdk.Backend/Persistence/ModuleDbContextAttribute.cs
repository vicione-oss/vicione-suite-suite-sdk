namespace Sdk.Backend.Persistence;

/// <summary>
/// Marks a <see cref="ModuleDbContext"/> subclass for source generation of the
/// SQLite/PostgreSQL implementation classes and their design-time factories.
/// </summary>
/// <remarks>
/// <para>The annotated class must be <c>partial</c> and derive from <see cref="ModuleDbContext"/>.</para>
/// <para>The generator will produce:</para>
/// <list type="bullet">
///   <item>A <c>protected</c> constructor accepting <see cref="Microsoft.EntityFrameworkCore.DbContextOptions"/> (unless one is already declared)</item>
///   <item>An override of <see cref="ModuleDbContext.DefaultSchemaName"/> (if <see cref="DefaultSchemaName"/> is set)</item>
///   <item><c>{ClassName}Sqlite</c> — sealed subclass implementing <see cref="ISqliteDbContext"/></item>
///   <item><c>{ClassName}Postgres</c> — sealed subclass implementing <see cref="IPostgresDbContext"/></item>
///   <item><c>{ClassName}SqliteFactory</c> — <see cref="Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory{T}"/> for the SQLite context</item>
///   <item><c>{ClassName}PostgresFactory</c> — <see cref="Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory{T}"/> for the PostgreSQL context</item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ModuleDbContextAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the default database schema name for this module context.
    /// When set, the generator emits <c>public override string DefaultSchemaName =&gt; "…";</c>.
    /// </summary>
    public string? DefaultSchemaName { get; set; }
}
