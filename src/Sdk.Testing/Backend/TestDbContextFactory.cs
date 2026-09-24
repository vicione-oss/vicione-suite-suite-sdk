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
        // The returned context owns this connection and disposes it when the context is disposed
        var connection = new SqliteConnection($"Data Source={dataSource ?? DataSourceInMemory};");
#pragma warning restore CA2000 // Dispose objects before losing scope
        try
        {
            connection.Open();
            return CreateSqliteContext<TDbContext>(connection, contextOwnsConnection: true, init, optionsAction: null);
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Creates a SQLite DbContext for testing using an existing <see cref="SqliteConnection"/>.
    /// </summary>
    public static TDbContext CreateSqliteContext<TDbContext>(SqliteConnection connection, bool init = true,
        Action<DbContextOptionsBuilder>? optionsAction = null)
        where TDbContext : DbContext => CreateSqliteContext<TDbContext>(connection, contextOwnsConnection: false, init, optionsAction);

    /// <summary>
    /// Creates a SQLite DbContext for testing using an existing <see cref="SqliteConnection"/>.
    /// When <paramref name="contextOwnsConnection"/> is <see langword="true"/>, disposing the context disposes the connection.
    /// </summary>
    internal static TDbContext CreateSqliteContext<TDbContext>(SqliteConnection connection, bool contextOwnsConnection,
        bool init, Action<DbContextOptionsBuilder>? optionsAction)
        where TDbContext : DbContext
    {
        var optionsBuilder = new DbContextOptionsBuilder<TDbContext>()
            .UseSqlite(connection, contextOwnsConnection)
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
