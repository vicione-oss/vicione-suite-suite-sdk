using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Sdk.Backend.Persistence;

/// <summary>
/// The part of a module's EF Core context that the host needs to migrate, replicate and save it; a module derives its
/// own interface from this one and adds its <c>DbSet</c> properties.
/// </summary>
public interface IModuleDbContext : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the context's <see cref="Microsoft.EntityFrameworkCore.DbContext.ChangeTracker"/>.
    /// </summary>
    ChangeTracker ChangeTracker { get; }

    /// <summary>
    /// Gets the context's <see cref="Microsoft.EntityFrameworkCore.DbContext.Database"/> facade, e.g. for transactions or raw SQL.
    /// </summary>
    DatabaseFacade Database { get; }

    /// <summary>
    /// Gets the schema the module's tables live in; <see cref="ModuleDbContext"/> applies it as the model's default schema.
    /// </summary>
    string DefaultSchemaName { get; }

    /// <summary>
    /// Gets the entity types whose data is not replicated between the master and slave databases.
    /// </summary>
    IEnumerable<Type> NotSynchronizedEntityTypes { get; }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    int SaveChanges();

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies any pending migrations for this context to the database.
    /// </summary>
    Task MigrateAsync(CancellationToken cancellationToken = default);
}
