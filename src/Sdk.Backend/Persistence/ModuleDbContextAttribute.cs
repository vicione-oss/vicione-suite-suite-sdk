namespace Sdk.Backend.Persistence;

/// <summary>
/// Marks a <see cref="ModuleDbContext"/> subclass for source generation of the
/// SQLite/PostgreSQL implementation classes and their design-time factories.
/// </summary>
/// <remarks>
/// <para>The annotated class must be <c>partial</c> and derive from <see cref="ModuleDbContext"/>.</para>
/// <para>The generator produces:</para>
/// <list type="bullet">
///   <item>A <c>protected</c> constructor taking <see cref="Microsoft.EntityFrameworkCore.DbContextOptions"/>, unless declared</item>
///   <item>An override of <see cref="ModuleDbContext.DefaultSchemaName"/>, if <see cref="DefaultSchemaName"/> is set</item>
///   <item>A <c>DbSet</c> property for each one on an <see cref="IModuleDbContext"/>-derived interface, unless declared</item>
///   <item><c>{ClassName}Sqlite</c> — sealed subclass implementing <see cref="ISqliteDbContext"/></item>
///   <item><c>{ClassName}Postgres</c> — sealed subclass implementing <see cref="IPostgresDbContext"/></item>
///   <item><c>{ClassName}SqliteFactory</c> — design-time factory (<c>IDesignTimeDbContextFactory</c>) for the SQLite context</item>
///   <item><c>{ClassName}PostgresFactory</c> — design-time factory (<c>IDesignTimeDbContextFactory</c>) for the PostgreSQL context</item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ModuleDbContextAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the schema the generator returns from <see cref="ModuleDbContext.DefaultSchemaName"/>;
    /// <see langword="null"/> leaves that override to the class.
    /// </summary>
    public string? DefaultSchemaName { get; set; }
}
