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
    /// SQLite data source for a private in-memory database that lives as long as its connection.
    /// </summary>
    public const string DataSourceInMemory = ":memory:";

    /// <summary>
    /// Creates a context over a new in-memory SQLite database.
    /// </summary>
    /// <param name="init">Drops and recreates the schema from the model; <see langword="false"/> leaves the database untouched.</param>
    public static TDbContext CreateSqliteContext<TDbContext>(bool init = true)
        where TDbContext : ModuleDbContext, ISqliteDbContext => CreateSqliteContext<TDbContext>(DataSourceInMemory, init);

    /// <summary>
    /// Creates a context over an opened connection to <paramref name="dataSource"/>.
    /// </summary>
    /// <param name="dataSource">A SQLite data source, e.g. a file name; <see langword="null"/> means in-memory.</param>
    /// <param name="init">Drops and recreates the schema from the model; <see langword="false"/> leaves the database untouched.</param>
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
    /// Creates a context over <paramref name="connection"/>, which stays owned by the caller.
    /// </summary>
    /// <param name="connection">An open connection; the context does not dispose it.</param>
    /// <param name="init">Drops and recreates the schema from the model; <see langword="false"/> leaves the database untouched.</param>
    /// <param name="optionsAction">Adjusts the options after SQLite is configured.</param>
    /// <exception cref="MissingMethodException">Thrown if <typeparamref name="TDbContext"/> has no public options constructor.</exception>
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
