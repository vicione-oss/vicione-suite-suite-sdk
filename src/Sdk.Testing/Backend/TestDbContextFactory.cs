using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Sdk.Backend.Persistence;

namespace Sdk.Testing.Backend;

/// <summary>
/// Provides a factory for creating <see cref="ModuleDbContext"/> instances for testing purposes.
/// </summary>
public static class TestDbContextFactory
{
    /// <summary>
    /// The connection string identifier for creating an in-memory SQLite database.
    /// </summary>
    public const string DataSourceInMemory = ":memory:";

    /// <summary>
    /// Creates an in-memory SQLite DbContext for testing.
    /// </summary>
    public static TDbContext CreateSqliteContext<TDbContext>(bool init = true)
        where TDbContext : ModuleDbContext, ISqliteDbContext => CreateSqliteContext<TDbContext>(DataSourceInMemory, init);

    /// <summary>
    /// Creates a SQLite DbContext for testing using a specified data source.
    /// </summary>
    public static TDbContext CreateSqliteContext<TDbContext>(string? dataSource, bool init = true)
        where TDbContext : ModuleDbContext, ISqliteDbContext
    {
#pragma warning disable CA2000 // Dispose objects before losing scope
        // Connection gets disposed by DbContext
        var connection = new SqliteConnection($"Data Source={dataSource ?? DataSourceInMemory};");
#pragma warning restore CA2000 // Dispose objects before losing scope
        connection.Open();

        return CreateSqliteContext<TDbContext>(connection, init);
    }

    /// <summary>
    /// Creates a SQLite DbContext for testing using an existing <see cref="SqliteConnection"/>.
    /// </summary>
    public static TDbContext CreateSqliteContext<TDbContext>(SqliteConnection connection, bool init = true,
        Action<DbContextOptionsBuilder>? optionsAction = null)
        where TDbContext : DbContext
    {
        var optionsBuilder = new DbContextOptionsBuilder<TDbContext>()
            .UseSqlite(connection)
#if DEBUG            
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
#endif            
            ;
        optionsAction?.Invoke(optionsBuilder);

        var moduleDbContext = Activator.CreateInstance(typeof(TDbContext), optionsBuilder.Options) as TDbContext
            ?? throw new InvalidOperationException($"Failed to create {typeof(TDbContext).Name}");

        if (init)
        {
            moduleDbContext.Database.EnsureDeleted();
            moduleDbContext.Database.EnsureCreated();
        }

        return moduleDbContext;
    }
}
